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

    public async Task<string> GenerateSummaryAsync(IEnumerable<LogEntry> logs)
    {
        var lines = logs.Select(l => $"• [{l.Project}] {l.Description} ({l.Hours}h)");
        var prompt = $"Write a brief, professional end-of-day summary (3-4 sentences) from these work log entries:\n\n" +
                     string.Join('\n', lines);
        return await CallOllamaAsync(prompt, timeout: 60);
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
