namespace Game.Shared.Services.Localization
{
    public interface ILocalizationService
    {
        string GetLocalizedString(string table, string key);
        void SetLanguage(string localeCode);
    }
}
