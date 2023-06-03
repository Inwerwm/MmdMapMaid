using System.Text.Json;
using Microsoft.Extensions.Options;

using MmdMapMaid.Contracts.Services;
using MmdMapMaid.Core.Contracts.Services;
using MmdMapMaid.Core.Helpers;
using MmdMapMaid.Helpers;
using MmdMapMaid.Models;
using Windows.Storage;

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

    private IDictionary<string, object> _settings;

    private bool _isInitialized;

    public LocalSettingsService(IFileService fileService, IOptions<LocalSettingsOptions> options)
    {
        _fileService = fileService;
        _options = options.Value;

        _applicationDataFolder = Path.Combine(_localApplicationData, _options.ApplicationDataFolder ?? _defaultApplicationDataFolder);
        _localsettingsFile = _options.LocalSettingsFile ?? _defaultLocalSettingsFile;

        _settings = new Dictionary<string, object>();
    }

    private async Task InitializeAsync()
    {
        if (!_isInitialized)
        {
            _settings = await Task.Run(() => _fileService.Read<IDictionary<string, object>>(_applicationDataFolder, _localsettingsFile)) ?? new Dictionary<string, object>();

            _isInitialized = true;
        }
    }

    public async Task<T?> ReadSettingAsync<T>(string key, JsonSerializerOptions? options = null)
    {
        if (RuntimeHelper.IsMSIX)
        {
            if (ApplicationData.Current.LocalSettings.Values.TryGetValue(key, out var obj))
            {
                return await Json.ToObjectAsync<T>((string)obj, options);
            }
        }
        else
        {
            await InitializeAsync();

            if (_settings != null && _settings.TryGetValue(key, out var obj))
            {
                return await Json.ToObjectAsync<T>((string)obj, options);
            }
        }

        return default;
    }

    public async Task SaveSettingAsync<T>(string key, T value, JsonSerializerOptions? options = null)
    {
        if (RuntimeHelper.IsMSIX)
        {
            ApplicationData.Current.LocalSettings.Values[key] = await Json.StringifyAsync(value, options);
        }
        else
        {
            await InitializeAsync();

            _settings[key] = await Json.StringifyAsync(value, options);

            await Task.Run(() => _fileService.Save(_applicationDataFolder, _localsettingsFile, _settings));
        }
    }

    private void Initialize()
    {
        if (!_isInitialized)
        {
            _settings = _fileService.Read<IDictionary<string, object>>(_applicationDataFolder, _localsettingsFile) ?? new Dictionary<string, object>();

            _isInitialized = true;
        }
    }

    public T? ReadSetting<T>(string key, JsonSerializerOptions? options = null)
    {
        if (RuntimeHelper.IsMSIX)
        {
            if (ApplicationData.Current.LocalSettings.Values.TryGetValue(key, out var obj))
            {
                return Json.ToObject<T>((string)obj, options);
            }
        }
        else
        {
            Initialize();

            if (_settings != null && _settings.TryGetValue(key, out var obj))
            {
                return Json.ToObject<T>((string)obj, options);
            }
        }

        return default;
    }

    public void SaveSetting<T>(string key, T value, JsonSerializerOptions? options = null)
    {
        if (RuntimeHelper.IsMSIX)
        {
            ApplicationData.Current.LocalSettings.Values[key] = Json.Stringify(value, options);
        }
        else
        {
            Initialize();

            _settings[key] = Json.Stringify(value, options);

            _fileService.Save(_applicationDataFolder, _localsettingsFile, _settings);
        }
    }

    public JsonSerializerOptions CreateOptionWithDictionaryConverter<TKey, TValue>() where TKey : notnull
    {
        var opt = new JsonSerializerOptions();
        opt.Converters.Add(new NonStringKeyDictionaryConverter<TKey, TValue>());

        return opt;
    }
}
