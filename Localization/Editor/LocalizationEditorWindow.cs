using System.Collections;
using System.Collections.Generic;
using UnityEditor.SceneManagement;
using UnityEditorInternal;
using UnityEngine;
using UnityEngine.UI;

namespace UnityEditor
{
    //TODO find missing Keys
    public class LocalizationEditorWindow : EditorWindow
    {
        [MenuItem("Localization/Localization Editor")]
        public static LocalizationEditorWindow Init()
        {
            LocalizationEditorWindow window = (LocalizationEditorWindow)GetWindow(typeof(LocalizationEditorWindow));

            window.minSize = new Vector2(450, 355);

            Texture icon = EditorGUIUtility.IconContent(EditorGUIUtility.isProSkin ? "d_AlphabeticalSorting" : "AlphabeticalSorting").image;
            window.titleContent = new GUIContent("Localization", icon);
            window.Show();

            return window;
        }

        private LocalizationData targetData = null;
        private int selectedLanguageIndex = 0;
        private List<LocalizationData> datas = null;

        private Vector2 scroll;
        private bool stringFoldOut = false, spriteFoldOut = false;

        private ReorderableList stringReorderableList = null, spriteReorderableList = null;
        List<int> stringKeyPairMultipleKeyIndex = new List<int>();
        List<int> spriteKeyPairMultipleKeyIndex = new List<int>();

        private void OnEnable()
        {
            UpdateLocalizationData();
        }

        private void OnGUI()
        {
            GUILayout.Label("Localization Editor", EditorStyles.boldLabel);

            EditorGUILayout.BeginHorizontal();
            {
                targetData = EditorGUILayoutExtension.DisabledObjectField("Language", targetData, typeof(LocalizationData), false) as LocalizationData;

                #region Draw current exist language dropdown
                PreloadAllLocalizationData();

                string[] languages = new string[datas.Count + 1];
                for (int i = 0; i < datas.Count; i++)
                    languages[i] = datas[i].name;
                languages[datas.Count] = "Add new Language";

                if (selectedLanguageIndex >= languages.Length)
                    selectedLanguageIndex = languages.Length - 1;

                EditorGUI.BeginChangeCheck();
                int prevSelectedLanguageIndex = selectedLanguageIndex == languages.Length ? 0 : selectedLanguageIndex;
                selectedLanguageIndex = EditorGUILayout.Popup(selectedLanguageIndex, languages);
                if (EditorGUI.EndChangeCheck())
                {
                    //On Add new Language
                    if (selectedLanguageIndex == datas.Count)
                    {
                        selectedLanguageIndex = prevSelectedLanguageIndex;
                        CreateNewLocalizationEditorWindow.Init();
                    }
                    else //Language selected
                    {
                        var selectedLanguage = languages[selectedLanguageIndex].ToLower();
                        if (targetData == null || targetData.name.ToLower() != selectedLanguage)
                            LoadLocalizationData(selectedLanguage);
                    }
                }
                #endregion Draw current exist language dropdown
            }
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.Space();
            scroll = EditorGUILayout.BeginScrollView(scroll);
            {
                EditorGUILayout.LabelField("Localization Data", EditorStyles.boldLabel);
                stringFoldOut = EditorGUILayout.Foldout(stringFoldOut, "Strings", true);
                if (stringFoldOut)
                {
                    EditorGUI.BeginChangeCheck();
                    {
                        if (stringReorderableList != null)
                            stringReorderableList.DoLayoutList();
                    }
                    if (EditorGUI.EndChangeCheck())
                        StringReorderableListChanged();
                }

                spriteFoldOut = EditorGUILayout.Foldout(spriteFoldOut, "Sprites", true);
                if (spriteFoldOut)
                {
                    EditorGUI.BeginChangeCheck();
                    {
                        if (spriteReorderableList != null)
                            spriteReorderableList.DoLayoutList();
                    }
                    if (EditorGUI.EndChangeCheck())
                        SpriteReorderableListChanged();
                }

                GUI.enabled = false;
                GUILayout.Button("Export to .csv");
                GUILayout.Button("Import from .csv");
                GUI.enabled = true;
            }
            EditorGUILayout.EndScrollView();
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

        [System.Serializable]
        private class LocalizedStringKeyPair
        {
            [SerializeField]
            public string localizationKey = "";
            [SerializeField]
            public string text = "";

            public LocalizedStringKeyPair(string localizationKey, string text)
            {
                this.localizationKey = localizationKey;
                this.text = text;
            }

            public LocalizedStringKeyPair()
            {

            }
        }

        [System.Serializable]
        private class LocalizedSpriteKeyPair
        {
            [SerializeField]
            public string localizationKey = "";
            [SerializeField]
            public Sprite sprite = null;

            public LocalizedSpriteKeyPair(string localizationKey, Sprite sprite)
            {
                this.localizationKey = localizationKey;
                this.sprite = sprite;
            }

            public LocalizedSpriteKeyPair() { }
        }

        private void UpdateLocalizationData()
        {
            if (targetData == null)
            {
                stringReorderableList = null;
                spriteReorderableList = null;

                stringKeyPairMultipleKeyIndex.Clear();
                spriteKeyPairMultipleKeyIndex.Clear();
            }
            else
            {
                List<LocalizedStringKeyPair> stringKeyPair = new List<LocalizedStringKeyPair>();
                foreach (var key in targetData.strings.Keys)
                    stringKeyPair.Add(new LocalizedStringKeyPair(key, targetData.strings[key]));
                stringReorderableList = new ReorderableList(stringKeyPair, typeof(LocalizedStringKeyPair), true, true, true, true);

                stringReorderableList.elementHeight = 55;
                stringReorderableList.drawElementCallback = (rect, index, isActive, isFocused) =>
                {
                    var element = stringKeyPair[index];
                    rect.height -= 8;
                    rect.y += 4;

                    Rect keyLabelFieldRect = rect; keyLabelFieldRect.width = 40;
                    EditorGUI.LabelField(keyLabelFieldRect, "  Key");

                    Rect keyFieldRect = rect; keyFieldRect.width = 200; keyFieldRect.x += keyLabelFieldRect.width; keyFieldRect.height = 25 - 8;
                    element.localizationKey = EditorGUI.TextField(keyFieldRect, element.localizationKey);

                    Rect alretFieldRect = rect; keyFieldRect.width = 200; alretFieldRect.x += keyLabelFieldRect.width; alretFieldRect.height = 50 - 8; alretFieldRect.y += keyFieldRect.height;
                    GUIStyle style = new GUIStyle(EditorStyles.label);
                    style.normal.textColor = Color.red;
                    if (string.IsNullOrEmpty(element.localizationKey))
                        EditorGUI.LabelField(alretFieldRect, "Localization key can't be empty\r\nIt will be removed", style);
                    else if (stringKeyPairMultipleKeyIndex.Contains(index))
                        EditorGUI.LabelField(alretFieldRect, "This key is already exist\r\nIt will be removed", style);

                    Rect stringLabelFieldRect = rect; stringLabelFieldRect.width = 50; stringLabelFieldRect.x += keyLabelFieldRect.width + keyFieldRect.width;
                    EditorGUI.LabelField(stringLabelFieldRect, "  String");

                    Rect stringFieldRect = rect; stringFieldRect.width -= keyLabelFieldRect.width + keyFieldRect.width + stringLabelFieldRect.width; stringFieldRect.x += keyLabelFieldRect.width + keyFieldRect.width + stringLabelFieldRect.width;
                    element.text = EditorGUI.TextField(stringFieldRect, element.text);
                };

                stringReorderableList.onChangedCallback = (list) =>
                {
                    StringReorderableListChanged();
                };

                List<LocalizedSpriteKeyPair> spriteKeyPair = new List<LocalizedSpriteKeyPair>();
                foreach (var key in targetData.sprites.Keys)
                    spriteKeyPair.Add(new LocalizedSpriteKeyPair(key, targetData.sprites[key]));
                spriteReorderableList = new ReorderableList(spriteKeyPair, typeof(LocalizedSpriteKeyPair), true, true, true, true);

                spriteReorderableList.elementHeight = 100;
                spriteReorderableList.drawElementCallback = (rect, index, isActive, isFocused) =>
                {
                    var element = spriteKeyPair[index];
                    rect.height -= 8;
                    rect.y += 4;

                    Rect keyLabelFieldRect = rect; keyLabelFieldRect.width = 40;
                    EditorGUI.LabelField(keyLabelFieldRect, "  Key");

                    Rect keyFieldRect = rect; keyFieldRect.width = 200; keyFieldRect.x += keyLabelFieldRect.width; keyFieldRect.height = 25 - 8;
                    element.localizationKey = EditorGUI.TextField(keyFieldRect, element.localizationKey);

                    Rect alretFieldRect = rect; keyFieldRect.width = 200; alretFieldRect.x += keyLabelFieldRect.width; alretFieldRect.height = 50 - 8; alretFieldRect.y += keyFieldRect.height;
                    GUIStyle style = new GUIStyle(EditorStyles.label);
                    style.normal.textColor = Color.red;
                    if (string.IsNullOrEmpty(element.localizationKey))
                        EditorGUI.LabelField(alretFieldRect, "Localization key can't be empty\r\nIt will be removed", style);
                    else if (spriteKeyPairMultipleKeyIndex.Contains(index))
                        EditorGUI.LabelField(alretFieldRect, "This key is already exist\r\nIt will be removed", style);

                    Rect spriteFieldRect = rect; spriteFieldRect.width = rect.height; spriteFieldRect.x = rect.width - spriteFieldRect.width;
                    element.sprite = EditorGUI.ObjectField(spriteFieldRect, element.sprite, typeof(Sprite), false) as Sprite;

                    Rect spriteLabelFieldRect = rect; spriteLabelFieldRect.width = 50; spriteLabelFieldRect.x = spriteFieldRect.x - spriteLabelFieldRect.width;
                    EditorGUI.LabelField(spriteLabelFieldRect, "  Sprite");
                };

                spriteReorderableList.onChangedCallback = (list) =>
                {
                    SpriteReorderableListChanged();
                };
            }
        }

        private void StringReorderableListChanged()
        {
            List<LocalizedStringKeyPair> stringKeyPair = stringReorderableList.list as List<LocalizedStringKeyPair>;
            List<string> keys = new List<string>();
            List<int> emptyIndex = new List<int>();
            stringKeyPairMultipleKeyIndex.Clear();

            int count = stringKeyPair.Count;
            for (int i = 0; i < count; i++)
            {
                stringKeyPair[i].localizationKey = stringKeyPair[i].localizationKey.Replace(" ", "").ToLower();

                if (!keys.Contains(stringKeyPair[i].localizationKey))
                    keys.Add(stringKeyPair[i].localizationKey);
                else
                    stringKeyPairMultipleKeyIndex.Add(i);

                if (string.IsNullOrEmpty(stringKeyPair[i].localizationKey))
                    emptyIndex.Add(i);
            }

            targetData.strings.Clear();
            for (int i = 0; i < count; i++)
            {
                if (stringKeyPairMultipleKeyIndex.Contains(i) || emptyIndex.Contains(i))
                    continue;

                var curPair = stringKeyPair[i];
                targetData.strings.Add(curPair.localizationKey, curPair.text);
            }

            EditorUtility.SetDirty(targetData);
            LocalizationSettingsEditorWindow.UpdateLocalizedObjects();
        }

        private void SpriteReorderableListChanged()
        {
            List<LocalizedSpriteKeyPair> spriteKeyPair = spriteReorderableList.list as List<LocalizedSpriteKeyPair>;
            List<string> keys = new List<string>();
            List<int> emptyIndex = new List<int>();
            spriteKeyPairMultipleKeyIndex.Clear();

            int count = spriteKeyPair.Count;
            for (int i = 0; i < count; i++)
            {
                spriteKeyPair[i].localizationKey = spriteKeyPair[i].localizationKey.Replace(" ", "").ToLower();

                if (!keys.Contains(spriteKeyPair[i].localizationKey))
                    keys.Add(spriteKeyPair[i].localizationKey);
                else
                    spriteKeyPairMultipleKeyIndex.Add(i);

                if (string.IsNullOrEmpty(spriteKeyPair[i].localizationKey))
                    emptyIndex.Add(i);
            }

            targetData.sprites.Clear();
            for (int i = 0; i < count; i++)
            {
                if (spriteKeyPairMultipleKeyIndex.Contains(i) || emptyIndex.Contains(i))
                    continue;

                var curPair = spriteKeyPair[i];
                targetData.sprites.Add(curPair.localizationKey, curPair.sprite);
            }

            EditorUtility.SetDirty(targetData);
            LocalizationSettingsEditorWindow.UpdateLocalizedObjects();
        }

        public void LoadLocalizationData(string language)
        {
            targetData = Resources.Load<LocalizationData>("Localization/" + language);

            PreloadAllLocalizationData();
            selectedLanguageIndex = datas.FindIndex((x) => { return x.name.ToLower() == language.ToLower(); });

            UpdateLocalizationData();
        }
    }
}