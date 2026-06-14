using System.Text.Json;
using WorkLogApp.Models;

namespace WorkLogApp.Services;

public class OllamaService(HttpClient http, IConfiguration cfg) : IOllamaService
{
    private readonly string _model = cfg["Ollama:Model"] ?? "llama3.2";

    public async Task<string> ImproveDescriptionAsync(string text)
    {
        var prompt = $"Rewrite this work-log description to be clear, concise, and professional. " +
                     $"Return ONLY the improved text, no quotes or explanations:\n\n{text}";
        return await CallOllamaAsync(prompt);
    }

    public async Task<string> FixEnglishAsync(string text)
    {
        var prompt = $"Fix the spelling, grammar, and clarity of the following text. " +
                     $"Return ONLY the corrected text, no explanation, no quotes:\n\n{text}";
        return await CallOllamaAsync(prompt, timeout: 20);
    }

    public async Task<string> GenerateSummaryAsync(IEnumerable<LogEntry> logs)
    {
        var lines = logs.Select(l => $"• [{l.Project}] {l.Description} ({l.Hours}h)");
        var prompt = $"Write a brief, professional end-of-day summary (3-4 sentences) from these work log entries:\n\n" +
                     string.Join('\n', lines);
        return await CallOllamaAsync(prompt, timeout: 60);
    }

    public async Task<string> GenerateTimesheetSummaryAsync(
        string employeeName, DateOnly start, DateOnly end, IEnumerable<LogEntry> entries)
    {
        var list    = entries.ToList();
        var grouped = list.GroupBy(e => e.Project)
                          .Select(g => $"• {g.Key}: {g.Sum(e => e.Hours)}h — {string.Join("; ", g.Select(e => e.Description))}");

        var prompt =
            $"Write a professional 3-4 sentence timesheet summary for {employeeName} " +
            $"covering {start:MMMM d} to {end:MMMM d, yyyy}. " +
            $"Total hours logged: {list.Sum(e => e.Hours)}h across {list.Select(e => e.Project).Distinct().Count()} projects. " +
            $"Work completed:\n{string.Join('\n', grouped)}\n\n" +
            $"The summary should be suitable for a manager or HR review — clear, factual, professional.";

        return await CallOllamaAsync(prompt, timeout: 90);
    }

    public async Task<ParsedEntry> ParseLogEntryAsync(string text)
    {
        var prompt =
            $"Parse this work log into JSON. Return ONLY a valid JSON object — no explanation, no markdown.\n" +
            $"Keys: \"project\" (string), \"hours\" (decimal or null), \"description\" (clear professional description, string), \"tags\" (comma-separated keywords, string).\n\n" +
            $"Input: {text}\n\nJSON:";

        string raw;
        try { raw = await CallOllamaAsync(prompt, timeout: 20); }
        catch { return new ParsedEntry(null, null, text, ""); }

        var start = raw.IndexOf('{');
        var end   = raw.LastIndexOf('}');
        if (start < 0 || end <= start) return new ParsedEntry(null, null, text, "");

        try
        {
            using var doc  = JsonDocument.Parse(raw[start..(end + 1)]);
            var root       = doc.RootElement;
            var project    = root.TryGetProperty("project",     out var p) ? p.GetString() : null;
            var desc       = root.TryGetProperty("description", out var d) ? d.GetString() ?? text : text;
            var tags       = root.TryGetProperty("tags",        out var t) ? t.GetString() ?? "" : "";
            decimal? hours = null;
            if (root.TryGetProperty("hours", out var h) && h.ValueKind is JsonValueKind.Number)
                hours = h.GetDecimal();
            return new ParsedEntry(project, hours, desc, tags);
        }
        catch { return new ParsedEntry(null, null, text, ""); }
    }

    public async Task<List<ParsedDayEntry>> ParseDayLogAsync(string text)
    {
        var prompt =
            "Parse the following work-day description into a JSON array of individual tasks.\n" +
            "For each task extract exactly these keys:\n" +
            "  description : clear, professional one-line description\n" +
            "  hours       : decimal number (e.g. 2.0, 1.5, 0.5) — estimate if not stated\n" +
            "  category    : one of Development | Meeting | Code Review | Testing | Documentation | DevOps | Research | Design | Other\n" +
            "  project     : project or client name if mentioned, otherwise \"General\"\n\n" +
            "IMPORTANT: Return ONLY a raw JSON array. No markdown fences, no explanation.\n\n" +
            $"Input: {text}\n\nJSON:";

        string raw;
        try { raw = await CallOllamaAsync(prompt, timeout: 35); }
        catch { return [new ParsedDayEntry(text.Trim(), 0, "Other", "General")]; }

        var s = raw.IndexOf('[');
        var e = raw.LastIndexOf(']');
        if (s < 0 || e <= s) return [new ParsedDayEntry(text.Trim(), 0, "Other", "General")];

        try
        {
            var opts = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
            var list = JsonSerializer.Deserialize<List<ParsedDayEntry>>(raw[s..(e + 1)], opts);
            return list?.Where(x => !string.IsNullOrWhiteSpace(x.Description)).ToList()
                ?? [new ParsedDayEntry(text.Trim(), 0, "Other", "General")];
        }
        catch { return [new ParsedDayEntry(text.Trim(), 0, "Other", "General")]; }
    }

    public async Task<string> ChatAsync(string message, string? context = null)
    {
        const string system = "You are WorkLog AI, a focused productivity assistant built into the WorkLog app. " +
            "You help users with: work log summaries, standup updates, time management tips, productivity insights, " +
            "and analysis of their logged work entries.\n\n" +
            "Rules:\n" +
            "1. ONLY answer questions related to work, productivity, time tracking, standup updates, career growth, or the user's work data shown below.\n" +
            "2. If asked about anything unrelated (sports, cooking, general knowledge, etc.), respond: \"I'm WorkLog AI and I focus only on work-related topics. Ask me about your logs, standup, or productivity!\"\n" +
            "3. Keep answers concise, practical, and use bullet points where helpful.\n" +
            "4. If the user shares work data, reference it specifically in your answer.";

        var ctx = string.IsNullOrWhiteSpace(context) ? "" : $"\n\nUser's recent work data:\n{context}";
        var prompt = $"{system}{ctx}\n\nUser: {message}\n\nWorkLog AI:";
        return await CallOllamaAsync(prompt, timeout: 90);
    }

    private async Task<string> CallOllamaAsync(string prompt, int timeout = 40)
    {
        using var cts  = new CancellationTokenSource(TimeSpan.FromSeconds(timeout));
        var payload    = JsonSerializer.Serialize(new { model = _model, prompt, stream = false });
        var content    = new StringContent(payload, System.Text.Encoding.UTF8, "application/json");

        var resp       = await http.PostAsync("/api/generate", content, cts.Token);
        resp.EnsureSuccessStatusCode();

        using var doc  = JsonDocument.Parse(await resp.Content.ReadAsStringAsync(cts.Token));
        return doc.RootElement.GetProperty("response").GetString()?.Trim()
            ?? throw new InvalidOperationException("Empty response from Ollama");
    }
}
