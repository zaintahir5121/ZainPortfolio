using System.Text;
using System.Text.Json;

namespace WorkLogApp.Services;

public class OllamaService(HttpClient http, IConfiguration config)
{
    private readonly string _model   = config["Ollama:Model"] ?? "llama3.2";
    private readonly string _baseUrl = config["Ollama:Url"]   ?? config["Ollama:BaseUrl"] ?? "http://localhost:11434";

    public async Task<string> ChatAsync(
        string system,
        IEnumerable<(string role, string content)> history,
        string userMsg,
        CancellationToken ct = default)
    {
        var messages = new List<object>();
        if (!string.IsNullOrWhiteSpace(system))
            messages.Add(new { role = "system", content = system });
        foreach (var (role, content) in history)
            messages.Add(new { role, content });
        messages.Add(new { role = "user", content = userMsg });

        var payload = JsonSerializer.Serialize(new { model = _model, messages, stream = false });
        using var req = new HttpRequestMessage(HttpMethod.Post, $"{_baseUrl}/api/chat")
        {
            Content = new StringContent(payload, Encoding.UTF8, "application/json")
        };

        using var resp = await http.SendAsync(req, ct);
        resp.EnsureSuccessStatusCode();
        var json = await resp.Content.ReadAsStringAsync(ct);
        using var doc = JsonDocument.Parse(json);
        return doc.RootElement
                  .GetProperty("message")
                  .GetProperty("content")
                  .GetString() ?? "";
    }
}
