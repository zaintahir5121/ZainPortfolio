using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WorkLogApp.Data;
using WorkLogApp.Models.ViewModels;
using WorkLogApp.Services;

namespace WorkLogApp.Controllers;

[Authorize]
public class TimesheetController(AppDbContext db, IOllamaService ollama) : Controller
{
    private int    UserId => int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
    private string Name   => User.FindFirstValue(ClaimTypes.Name) ?? "";
    private string Role   => User.FindFirstValue(ClaimTypes.Role) ?? "";

    /* ── Main view ── */
    [HttpGet]
    public async Task<IActionResult> Index(string period = "mid", int? month = null, int? year = null)
    {
        var vm = await BuildViewModel(period, month, year);
        await SetSidebarStats();
        return View(vm);
    }

    /* ── Printable / PDF view ── */
    [HttpGet]
    public async Task<IActionResult> Print(string period = "mid", int? month = null, int? year = null)
    {
        var vm = await BuildViewModel(period, month, year);
        return View(vm);
    }

    /* ── CSV Export ── */
    [HttpGet]
    public async Task<IActionResult> ExportCsv(
        string period = "mid", int? month = null, int? year = null, string fmt = "standard")
    {
        var vm  = await BuildViewModel(period, month, year);
        var csv = fmt == "bamboohr" ? CsvBambooHr(vm) : CsvStandard(vm);

        var safeName = Name.Replace(" ", "_");
        var filename = $"timesheet_{safeName}_{vm.PeriodStart:yyyy-MM-dd}_{vm.PeriodEnd:yyyy-MM-dd}.csv";

        return File(Encoding.UTF8.GetBytes(csv), "text/csv", filename);
    }

    /* ── AI summary (AJAX) ── */
    [HttpPost]
    public async Task<IActionResult> AiSummary([FromBody] PeriodRequest req)
    {
        var vm = await BuildViewModel(req.Period, req.Month, req.Year);

        if (!vm.Entries.Any())
            return BadRequest(new { error = "No work logs found in this period." });

        try
        {
            var result = await ollama.GenerateTimesheetSummaryAsync(
                Name, vm.PeriodStart, vm.PeriodEnd, vm.Entries);
            return Ok(new { result });
        }
        catch
        {
            return StatusCode(503, new { error = "AI unavailable — make sure Ollama is running." });
        }
    }

    /* ── AI summary for a single day ── */
    [HttpGet]
    public async Task<IActionResult> AiDaySummary(string date)
    {
        if (!DateOnly.TryParse(date, out var d))
            return BadRequest(new { error = "Invalid date" });

        var entries = await db.LogEntries
            .Where(l => l.UserId == UserId && l.Date == d)
            .ToListAsync();

        if (!entries.Any())
            return BadRequest(new { error = "No entries for this day." });

        try
        {
            var result = await ollama.GenerateSummaryAsync(entries);
            return Ok(new { result });
        }
        catch { return StatusCode(503, new { error = "AI unavailable — make sure Ollama is running." }); }
    }

    /* ─────────────── helpers ─────────────── */

    private async Task<TimesheetViewModel> BuildViewModel(string period, int? month, int? year)
    {
        var m = month ?? DateTime.Today.Month;
        var y = year  ?? DateTime.Today.Year;
        var (start, end) = PeriodDates(period, m, y);

        var entries = await db.LogEntries
            .Where(l => l.UserId == UserId && l.Date >= start && l.Date <= end)
            .OrderBy(l => l.Date).ThenBy(l => l.CreatedAt)
            .ToListAsync();

        return new TimesheetViewModel
        {
            PeriodStart  = start,
            PeriodEnd    = end,
            PeriodType   = period,
            Month        = m,
            Year         = y,
            EmployeeName = Name,
            Role         = Role,
            Entries      = entries,
        };
    }

    private async Task SetSidebarStats()
    {
        var today     = DateOnly.FromDateTime(DateTime.Today);
        var weekStart = today.AddDays(-(int)today.DayOfWeek);
        var all       = await db.LogEntries.Where(l => l.UserId == UserId).ToListAsync();

        ViewBag.TodayHours    = all.Where(l => l.Date == today).Sum(l => l.Hours);
        ViewBag.WeekLogsCount = all.Count(l => l.Date >= weekStart);
        ViewBag.ProjectsCount = all.Select(l => l.Project).Distinct().Count();
    }

    private static (DateOnly, DateOnly) PeriodDates(string period, int month, int year) =>
        period switch
        {
            "end"  => (new DateOnly(year, month, 16),
                       new DateOnly(year, month, DateTime.DaysInMonth(year, month))),
            "full" => (new DateOnly(year, month, 1),
                       new DateOnly(year, month, DateTime.DaysInMonth(year, month))),
            _      => (new DateOnly(year, month, 1),   // "mid" is default
                       new DateOnly(year, month, 15)),
        };

    /* ── CSV builders ── */
    private static string CsvStandard(TimesheetViewModel vm)
    {
        var sb = new StringBuilder();
        sb.AppendLine($"Timesheet — {vm.EmployeeName}");
        sb.AppendLine($"Period: {vm.PeriodLabel}");
        sb.AppendLine($"Total Hours: {vm.TotalHours}");
        sb.AppendLine();
        sb.AppendLine("Date,Day,Project,Description,Hours,Tags");

        foreach (var e in vm.Entries)
            sb.AppendLine(
                $"{e.Date:yyyy-MM-dd}," +
                $"{e.Date.DayOfWeek}," +
                $"{CsvEsc(e.Project)}," +
                $"{CsvEsc(e.Description)}," +
                $"{e.Hours}," +
                $"{CsvEsc(e.Tags)}");

        sb.AppendLine();
        sb.AppendLine("Project Summary");
        sb.AppendLine("Project,Total Hours");
        foreach (var g in vm.ByProject)
            sb.AppendLine($"{CsvEsc(g.Key)},{g.Sum(e => e.Hours)}");

        return sb.ToString();
    }

    private static string CsvBambooHr(TimesheetViewModel vm)
    {
        // BambooHR Time Tracking import format
        var sb = new StringBuilder();
        sb.AppendLine("EmployeeName,Date,Hours,ProjectName,Notes");
        foreach (var e in vm.Entries)
            sb.AppendLine(
                $"{CsvEsc(vm.EmployeeName)}," +
                $"{e.Date:MM/dd/yyyy}," +
                $"{e.Hours}," +
                $"{CsvEsc(e.Project)}," +
                $"{CsvEsc(e.Description)}");
        return sb.ToString();
    }

    private static string CsvEsc(string s) =>
        s.Contains(',') || s.Contains('"') || s.Contains('\n')
            ? $"\"{s.Replace("\"", "\"\"")}\""
            : s;

    public record PeriodRequest(string Period, int Month, int Year);
}
