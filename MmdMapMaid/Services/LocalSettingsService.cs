using System.Text.Json;
using Microsoft.Extensions.Options;

using MmdMapMaid.Contracts.Services;
using MmdMapMaid.Core.Contracts.Services;
using MmdMapMaid.Core.Helpers;
using MmdMapMaid.Helpers;
using MmdMapMaid.Models;

namespace MmdMapMaid.Services;

public class LocalSettingsService : ILocalSettingsService
{
    private const string _defaultApplicationDataFolder = "MmdMapMaid/ApplicationData";
    private const string _defaultLocalSettingsFile = "LocalSettings.json";

    private readonly IFileService _fileService;
    private readonly LocalSettingsOptions _options;

    private readonly string _localApplicationData = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
    private readonly string _applicationDataFolder;
    private readonly string _localsettingsFile;

    private IDictionary<string, object?> _settings;

    private bool _isInitialized;

    public LocalSettingsService(IFileService fileService, IOptions<LocalSettingsOptions> options)
    {
        _fileService = fileService;
        _options = options.Value;

        _applicationDataFolder = Path.Combine(_localApplicationData, _options.ApplicationDataFolder ?? _defaultApplicationDataFolder);
        _localsettingsFile = _options.LocalSettingsFile ?? _defaultLocalSettingsFile;

        _settings = new Dictionary<string, object?>();
    }

    private void Initialize(JsonSerializerOptions? options = null)
    {
        if (!_isInitialized)
        {
            _settings = _fileService.Read<IDictionary<string, object?>>(_applicationDataFolder, _localsettingsFile, options ?? Factory.CreateJsonSerializerOptions()) ?? new Dictionary<string, object?>();

            _isInitialized = true;
        }
    }

    private async Task InitializeAsync(JsonSerializerOptions? options = null)
    {
        if (!_isInitialized)
        {
            _settings = await Task.Run(() => _fileService.Read<IDictionary<string, object?>>(_applicationDataFolder, _localsettingsFile, options ?? Factory.CreateJsonSerializerOptions())) ?? new Dictionary<string, object?>();

            _isInitialized = true;
        }
    }

    public T? ReadSetting<T>(string key, JsonSerializerOptions? options = null)
    {
        Initialize(options);

        return ReadSettingInternal<T>(key, options);
    }

    public async Task<T?> ReadSettingAsync<T>(string key, JsonSerializerOptions? options = null)
    {
        await InitializeAsync(options);

        return ReadSettingInternal<T>(key, options);
    }

    private T? ReadSettingInternal<T>(string key, JsonSerializerOptions? options = null)
    {
        if (_settings != null && _settings.TryGetValue(key, out var obj))
        {
            return obj switch
            {
                JsonElement e => Json.ToObject<T>(e.GetRawText(), options ?? Factory.CreateJsonSerializerOptions()),
                T t => t,
                _ => default
            };
        }

        return default;
    }

    public void SaveSetting<T>(string key, T value, JsonSerializerOptions? options = null)
    {
        Initialize(options);

        _settings[key] = value;

        _fileService.Save(_applicationDataFolder, _localsettingsFile, _settings, options ?? Factory.CreateJsonSerializerOptions());
    }

    public async Task SaveSettingAsync<T>(string key, T value, JsonSerializerOptions? options = null)
    {
        await InitializeAsync(options);

        _settings[key] = value;

        await Task.Run(() => _fileService.Save(_applicationDataFolder, _localsettingsFile, _settings, options ?? Factory.CreateJsonSerializerOptions()));
    }
}
