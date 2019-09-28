using System.Collections;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;

public static class LocalizationManager
{
    private static LocalizationSettings settings = null;
    public static string CurrentLanguage { get; private set; }

    private static Dictionary<string, LocalizationData> localizedDataDic = null;

    public static void SetLanguage(string language)
    {
        CurrentLanguage = language;
    }

    private static void LoadLocalizationData(string language)
    {
        if (language == null)
            language = "";

        if (settings == null)
        {
            settings = Resources.Load<LocalizationSettings>("Localization/Dependency/settings");
            CurrentLanguage = settings.currentLanguage;
        }

        language = language.ToLower();
        if (localizedDataDic == null)
            localizedDataDic = new Dictionary<string, LocalizationData>();
        if (localizedDataDic.Keys.Contains(language))
        {
            //Already loaded
        }
        else
        {
            var curLocalizedData = Resources.Load<LocalizationData>("Localization/" + language);
            if (curLocalizedData == null)
            {
                Debug.LogWarning(string.Format("Cannot find \"{0}\" localized datas! use \"Language Instead Missing ({1})\"setting", language, settings.languageInsteadMissing));
                curLocalizedData = Resources.Load<LocalizationData>("Localization/" + settings.languageInsteadMissing);
                if (curLocalizedData == null)
                    Debug.LogWarning(string.Format("Cannot find \"{0}\" localized datas!", settings.languageInsteadMissing));
            }

            localizedDataDic.Add(language, curLocalizedData);
        }
    }

    public static string GetLocalizedText(string localizationKey)
    {
        if (CurrentLanguage == null)
        {
            if (settings == null)
            {
                settings = Resources.Load<LocalizationSettings>("Localization/Dependency/settings");
                CurrentLanguage = settings.currentLanguage;
            }
        }

        return GetLocalizedText(localizationKey, CurrentLanguage);
    }

    public static string GetLocalizedText(string localizationKey, string language)
    {
        LoadLocalizationData(language);

        localizationKey = localizationKey.Replace(" ", "");

        if (string.IsNullOrEmpty(localizationKey))
        {
            if (Application.isPlaying)
                Debug.LogError("Localization Key cannot be empty");
            return null;
        }

        language = language.ToLower();
        if (localizedDataDic[language] == null)
            return null;

        if (localizedDataDic[language].strings.Keys.Contains(localizationKey))
        {
            var data = localizedDataDic[language].strings[localizationKey];
            if (data == null)
                Debug.LogWarning(string.Format("Localized string \"{0}\" on \"{1}\" is not setted", localizationKey, language));
            return data;
        }
        else
        {
            Debug.LogWarning(string.Format("Cannot find localized string \"{0}\" on \"{1}\"", localizationKey, language));
            return localizationKey;
        }
    }

    public static Sprite GetLocalizedSprite(string localizationKey)
    {
        return GetLocalizedSprite(localizationKey, CurrentLanguage);
    }

    public static Sprite GetLocalizedSprite(string localizationKey, string language)
    {
        if (string.IsNullOrEmpty(localizationKey))
        {
            Debug.LogError("Localization Key cannot be empty");
            return null;
        }

        language = language.ToLower();
        LoadLocalizationData(language);
        if (localizedDataDic[language] == null)
            return null;

        if (localizedDataDic[language].sprites.Keys.Contains(localizationKey))
        {
            var data = localizedDataDic[language].sprites[localizationKey];
            if (data == null)
                Debug.LogWarning(string.Format("Localized sprite \"{0}\" on \"{1}\" is not setted", localizationKey, language));

            return data;
        }
        else
        {
            Debug.LogWarning(string.Format("Cannot find localized sprite \"{0}\" on \"{1}\"", localizationKey, language));
            return null;
        }
    }
}