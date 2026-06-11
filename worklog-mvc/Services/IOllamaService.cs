using WorkLogApp.Models;

namespace WorkLogApp.Services;

public interface IOllamaService
{
    Task<string> ImproveDescriptionAsync(string text);
    Task<string> GenerateSummaryAsync(IEnumerable<LogEntry> logs);
    Task<string> GenerateTimesheetSummaryAsync(string employeeName, DateOnly start, DateOnly end, IEnumerable<LogEntry> entries);
}
