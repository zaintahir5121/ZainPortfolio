using WorkLogApp.Models;

namespace WorkLogApp.Services;

public interface IOllamaService
{
    Task<string> ImproveDescriptionAsync(string text);
    Task<string> GenerateSummaryAsync(IEnumerable<LogEntry> logs);
}
