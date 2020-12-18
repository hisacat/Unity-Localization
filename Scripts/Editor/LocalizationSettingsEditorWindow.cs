using System.Collections;
using System.Collections.Generic;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace UnityEditor
{
    public class LocalizationSettingsEditorWindow : EditorWindow
    {
        [MenuItem("Localization/Localization Setting", false, 0)]
        public static LocalizationSettingsEditorWindow Init()
        {
            LocalizationSettingsEditorWindow window = (LocalizationSettingsEditorWindow)GetWindow(typeof(LocalizationSettingsEditorWindow));

            window.minSize = new Vector2(450, 355);
            var settings = Resources.Load<LocalizationSettings>("Localization/Dependency/settings");
            if (settings == null)
            {
                if (!AssetDatabase.IsValidFolder("Assets/Resources"))
                    AssetDatabase.CreateFolder("Assets", "Resources");
                if (!AssetDatabase.IsValidFolder("Assets/Resources/Localization"))
                    AssetDatabase.CreateFolder("Assets/Resources", "Localization");
                if (!AssetDatabase.IsValidFolder("Assets/Resources/Localization/Dependency"))
                    AssetDatabase.CreateFolder("Assets/Resources/Localization", "Dependency");
                settings = CreateInstance<LocalizationSettings>();
                AssetDatabase.CreateAsset(settings, "Assets/Resources/Localization/Dependency/settings.asset");
                AssetDatabase.Refresh();
            }
            window.settings = settings;
            window.FindIndex();

            Texture icon = EditorGUIUtility.IconContent(EditorGUIUtility.isProSkin ? "d_AlphabeticalSorting" : "AlphabeticalSorting").image;
            window.titleContent = new GUIContent("Setting", icon);
            window.Show();

            return window;
        }

        private LocalizationSettings settings = null;
        private List<LocalizationData> datas = null;

        int selectedCurrentLanguageIndex = 0;
        int selectedLanguageInsteadMissingIndex = 0;

        private void FindIndex()
        {
            PreloadAllLocalizationData();
            if (datas.Count <= 0)
                return;

            selectedCurrentLanguageIndex = datas.FindIndex((x) => { return x.name.ToLower() == settings.currentLanguage.ToLower(); });
            selectedLanguageInsteadMissingIndex = datas.FindIndex((x) => { return x.name.ToLower() == settings.languageInsteadMissing.ToLower(); });

            if (selectedCurrentLanguageIndex < 0)
            {
                selectedCurrentLanguageIndex = 0;
                settings.currentLanguage = datas[0].name;
            }

            if (selectedLanguageInsteadMissingIndex < 0)
            {
                selectedLanguageInsteadMissingIndex = 0;
                settings.languageInsteadMissing = datas[0].name;
            }
        }

        private void OnGUI()
        {
            GUILayout.Label("Localization Setting", EditorStyles.boldLabel);

            PreloadAllLocalizationData();

            string[] languages = new string[datas.Count];
            for (int i = 0; i < datas.Count; i++)
                languages[i] = datas[i].name;

            if (selectedCurrentLanguageIndex >= languages.Length)
                selectedCurrentLanguageIndex = languages.Length - 1;
            if (selectedLanguageInsteadMissingIndex >= languages.Length)
                selectedLanguageInsteadMissingIndex = languages.Length - 1;

            EditorGUI.BeginChangeCheck();
            selectedCurrentLanguageIndex = EditorGUILayout.Popup("Current language", selectedCurrentLanguageIndex, languages);
            if (EditorGUI.EndChangeCheck())
            {
                settings.currentLanguage = languages[selectedCurrentLanguageIndex];
                LocalizationManager.SetLanguage(settings.currentLanguage);
                EditorUtility.SetDirty(settings);
                UpdateLocalizedObjects();
            }

            EditorGUI.BeginChangeCheck();
            selectedLanguageInsteadMissingIndex = EditorGUILayout.Popup("Instead missing", selectedLanguageInsteadMissingIndex, languages);
            if (EditorGUI.EndChangeCheck())
            {
                settings.languageInsteadMissing = languages[selectedLanguageInsteadMissingIndex];
                EditorUtility.SetDirty(settings);
                UpdateLocalizedObjects();
            }

            EditorGUI.BeginChangeCheck();
            settings.UsingRuntimeData = EditorGUILayout.Toggle(settings.UsingRuntimeData);
            if (EditorGUI.EndChangeCheck())
            {
                EditorUtility.SetDirty(settings);
                UpdateLocalizedObjects();
            }
        }

        private void PreloadAllLocalizationData()
        {
            if (datas == null)
                datas = new List<LocalizationData>();

            datas.Clear();
            {
                var allAssets = Resources.LoadAll("Localization/");
                foreach (var asset in allAssets)
                {
                    if (asset.GetType() == typeof(LocalizationData))
                        datas.Add(asset as LocalizationData);
                }
            }
        }


        public static void UpdateLocalizedObjects()
        {
            UpdateLocalizedTexts();
            UpdateLocalizedSprite();
        }

        public static void UpdateLocalizedTexts()
        {
            var localizedTexts = Resources.FindObjectsOfTypeAll<LocalizedText>();
            foreach (var localizedText in localizedTexts)
            {
                localizedText.UpdateLocalization();
                EditorUtility.SetDirty(localizedText);
            }
            var localizedTextMeshPros = Resources.FindObjectsOfTypeAll<LocalizationTextMeshProUGUI>();
            foreach (var localizedText in localizedTextMeshPros)
            {
                localizedText.UpdateLocalization();
                EditorUtility.SetDirty(localizedText);
            }
            EditorSceneManager.MarkAllScenesDirty();
        }

        public static void UpdateLocalizedSprite()
        {

        }
    }
}