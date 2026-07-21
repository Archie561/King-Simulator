using UnityEngine;

namespace Game.Shared.Services.Localization
{
    public class LocalizationService : ILocalizationService
    {
        public string GetLocalizedString(string table, string key)
        {
            // Placeholder: Replace with actual Unity Localization implementation
            // e.g., return LocalizationSettings.StringDatabase.GetLocalizedString(table, key);
            return key; 
        }

        public void SetLanguage(string localeCode)
        {
            Debug.Log($"Language set to {localeCode}");
        }
    }
}
