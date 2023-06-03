using MmdMapMaid.Contracts.Services;
using MmdMapMaid.Models;

namespace MmdMapMaid.Services;
internal class FeatureSettingService : IFeatureSettingService
{
    private const string SettingsKeyOfMorphNames = "MorphNames";

    private readonly ILocalSettingsService _localSettingsService;

    public Dictionary<PathInformation, string[]> MorphNames
    {
        get;
        private set;
    }

    public FeatureSettingService(ILocalSettingsService localSettingsService)
    {
        _localSettingsService = localSettingsService;

        MorphNames = new();
    }

    public async Task InitializeAsync()
    {
        MorphNames = await _localSettingsService.ReadSettingAsync<Dictionary<PathInformation, string[]>>(SettingsKeyOfMorphNames) ?? new();
    }

    public async Task SetMorphNamesAsync(Dictionary<PathInformation, string[]> morphNames) => await _localSettingsService.SaveSettingAsync(SettingsKeyOfMorphNames, morphNames);
}
