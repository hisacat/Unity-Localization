using System.Collections;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;

public static class LocalizationManager
{
    private static LocalizationSettings settings = null;
    public static string CurrentLanguage { get; private set; }

    private static Dictionary<string, LocalizationData> localizedDataDic = null;

    private static Dictionary<string, LocalizationData> runtimeLocalizedDataDic = null;

    private static Dictionary<string, LocalizationData> CurrentLocalizedDataDic
    {
        get
        {
            return runtimeLocalizedDataDic;
        }
    }

    public static void SetLanguage(string language, bool forceUpdateLocalizedTexts = true)
    {
        CurrentLanguage = language;

        if (forceUpdateLocalizedTexts)
            ForceUpdateLocalizedTexts();
    }

    public static void ForceUpdateLocalizedTexts()
    {
        var localizedTexts = GameObject.FindObjectsOfType<UnityEngine.UI.LocalizedText>();
        if (localizedTexts != null)
        {
            foreach (var text in localizedTexts)
                text.UpdateLocalization();
        }

        var localizedTMPTexts = GameObject.FindObjectsOfType<TMPro.LocalizationTextMeshProUGUI>();
        if (localizedTMPTexts != null)
        {
            foreach (var text in localizedTMPTexts)
                text.UpdateLocalization();
        }
    }

    public static void LoadRuntimeLocalizationData(Dictionary<string, LocalizationData> data, bool forceUpdateLocalizedTexts = true)
    {
        if (settings == null) settings = Resources.Load<LocalizationSettings>("Localization/Dependency/settings");
        if (settings == null)
        {
            Debug.LogError("Cannot find localization settings");
            return;
        }

        if (!settings.UsingRuntimeData)
        {
            Debug.LogError("LoadRuntimeLocalizationData only works when UsingRuntimeData is on! please check setting");
            return;
        }

        runtimeLocalizedDataDic = data;

        if (forceUpdateLocalizedTexts)
            ForceUpdateLocalizedTexts();
    }

    private static void LoadLocalizationData(string language)
    {
        if (language == null)
            language = "";

        if (settings == null) settings = Resources.Load<LocalizationSettings>("Localization/Dependency/settings");
        if (settings == null)
        {
            Debug.LogError("Cannot find localization settings");
            return;
        }

        if (string.IsNullOrEmpty(language))
        {
            CurrentLanguage = settings.currentLanguage;
        }

        language = language.ToLower();

        if (localizedDataDic == null)
            localizedDataDic = new Dictionary<string, LocalizationData>();
        if (runtimeLocalizedDataDic == null)
            runtimeLocalizedDataDic = new Dictionary<string, LocalizationData>();

        if (settings.UsingRuntimeData)
        {
            return;
        }

        if (localizedDataDic.Keys.Contains(language))
        {
            if (localizedDataDic[language] == null)
                localizedDataDic.Remove(language);
        }
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
                    Debug.LogError(string.Format("Cannot find Instead language \"{0}\" localized datas!", settings.languageInsteadMissing));
            }

            localizedDataDic.Add(language, curLocalizedData);
        }
    }

    public static string GetLocalizedText(string localizationKey, Object obj = null)
    {
        if (string.IsNullOrEmpty(CurrentLanguage))
        {
            if (settings == null)
            {
                settings = Resources.Load<LocalizationSettings>("Localization/Dependency/settings");
                if (settings == null)
                {
                    Debug.LogError("Cannot find localization settings", obj);
                    return null;
                }
                CurrentLanguage = settings.currentLanguage;
            }
        }

        return GetLocalizedText(localizationKey, CurrentLanguage);
    }

    public static string GetLocalizedText(string localizationKey, string language, Object obj = null)
    {
        LoadLocalizationData(language);

        if (string.IsNullOrEmpty(localizationKey))
        {
            if (Application.isPlaying)
                Debug.LogError("Localization Key cannot be empty", obj);
            return null;
        }

        localizationKey = localizationKey.ToLower();
        language = language.ToLower();
        if (CurrentLocalizedDataDic[language] == null)
            return localizationKey;

        if (CurrentLocalizedDataDic[language].strings.Keys.Contains(localizationKey))
        {
            var data = CurrentLocalizedDataDic[language].strings[localizationKey];
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
        if (CurrentLocalizedDataDic[language] == null)
            return null;

        if (CurrentLocalizedDataDic[language].sprites.Keys.Contains(localizationKey))
        {
            var data = CurrentLocalizedDataDic[language].sprites[localizationKey];
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