using System.Text.Json;

namespace MmdMapMaid.Contracts.Services;

public interface ILocalSettingsService
{
    T? ReadSetting<T>(string key, JsonSerializerOptions? options = null);
    Task<T?> ReadSettingAsync<T>(string key, JsonSerializerOptions? options = null);
    void SaveSetting<T>(string key, T value, JsonSerializerOptions? options = null);
    Task SaveSettingAsync<T>(string key, T value, JsonSerializerOptions? options = null);
}
