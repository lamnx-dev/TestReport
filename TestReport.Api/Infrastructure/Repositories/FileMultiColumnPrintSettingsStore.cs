using System.Text.Json;
using TestReport.Api.Application.Interfaces;
using TestReport.Api.Domain.Models;

namespace TestReport.Api.Infrastructure.Repositories;

public sealed class FileMultiColumnPrintSettingsStore : IMultiColumnPrintSettingsStore
{
    private readonly object _gate = new();
    private readonly string _path;
    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web) { WriteIndented = true };

    public FileMultiColumnPrintSettingsStore(IWebHostEnvironment environment)
    {
        _path = Path.Combine(environment.ContentRootPath, "App_Data", "multi-column-print-settings.json");
    }

    public MultiColumnPrintSettings Get()
    {
        lock (_gate)
        {
            if (!File.Exists(_path)) return MultiColumnPrintSettings.Default();
            var settings = JsonSerializer.Deserialize<MultiColumnPrintSettings>(File.ReadAllText(_path), JsonOptions);
            if (MultiColumnPrintSettings.Validate(settings) is { } error)
                throw new InvalidDataException($"Cấu hình in đã lưu không hợp lệ: {error}");
            return settings!;
        }
    }

    public MultiColumnPrintSettings Save(MultiColumnPrintSettings settings)
    {
        lock (_gate)
        {
            Directory.CreateDirectory(Path.GetDirectoryName(_path)!);
            var tempPath = _path + ".tmp";
            File.WriteAllText(tempPath, JsonSerializer.Serialize(settings, JsonOptions));
            File.Move(tempPath, _path, true);
            return settings;
        }
    }

    public MultiColumnPrintSettings Reset()
    {
        lock (_gate)
        {
            if (File.Exists(_path)) File.Delete(_path);
            return MultiColumnPrintSettings.Default();
        }
    }
}
