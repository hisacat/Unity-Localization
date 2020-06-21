using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
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

                if (targetData == null)
                {
                    if (selectedLanguageIndex >= 0)
                    {
                        if (languages != null && languages.Length > selectedLanguageIndex)
                        {
                            var selectedLanguage = languages[selectedLanguageIndex].ToLower();
                            LoadLocalizationData(selectedLanguage);
                        }
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

                //GUI.enabled = targetData != null;
                if (GUILayout.Button("Export to .csv"))
                    ExportToCSV();
                if (GUILayout.Button("CSV to JSON (Open link)"))
                    Application.OpenURL(@"https://csvjson.com/csv2json");
                if (GUILayout.Button("Import from .json"))
                    ImportFromJSON();
                //if (GUILayout.Button("Import from .csv"))
                //    ImportFromCSV();
                //GUI.enabled = true;
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
                    element.text = EditorGUI.TextArea(stringFieldRect, element.text);
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
                stringKeyPair[i].localizationKey = stringKeyPair[i].localizationKey.ToLower();

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

            foreach (var data in datas)
            {
                if (data == targetData)
                    continue;

                var oldDic = new LocalizationData.LocalizedStringDictionary();
                oldDic.CopyFrom(data.strings);

                data.strings.Clear();
                foreach (var key in keys)
                    data.strings.Add(key, oldDic.ContainsKey(key) ? oldDic[key] : null);

                EditorUtility.SetDirty(data);
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
                spriteKeyPair[i].localizationKey = spriteKeyPair[i].localizationKey.ToLower();

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

            foreach (var data in datas)
            {
                if (data == targetData)
                    continue;

                var oldDic = new LocalizationData.LocalizedSpriteDictionary();
                oldDic.CopyFrom(data.sprites);

                data.sprites.Clear();
                foreach (var key in keys)
                    data.sprites.Add(key, oldDic.ContainsKey(key) ? oldDic[key] : null);

                EditorUtility.SetDirty(data);
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

        private void ExportToCSV()
        {
            var path = EditorUtility.SaveFilePanel("Export to .csv", "", "localization data.csv", "csv");

            if (string.IsNullOrEmpty(path))
                return;

            var langueges = new List<string>();
            foreach (var data in datas)
                langueges.Add(data.name);

            string fileData = string.Format("key,{0}\r\n", string.Join(",", langueges));
            var keys = datas != null && datas.Count > 0 ? datas[0].strings.Keys : null;
            if (keys != null)
            {
                foreach (var key in keys)
                {
                    var values = new List<string>();
                    foreach (var data in datas)
                        values.Add(string.Format("\"{0}\"", data.strings[key].Replace("\"", "\"\"")));
                    fileData += string.Format("\"{0}\",{1}\r\n", key, string.Join(",", values));
                }
            }

            System.IO.File.WriteAllText(path, fileData, System.Text.Encoding.UTF8);
        }

        private void ImportFromJSON()
        {
            var path = EditorUtility.OpenFilePanel("Import from .json", "", "json");

            if (string.IsNullOrEmpty(path))
                return;

            var fileData = System.IO.File.ReadAllText(path, System.Text.Encoding.UTF8);
            var jsonData = SimpleJSON.JSON.Parse(fileData).AsArray;

            targetData = null;

            foreach (var data in datas)
                AssetDatabase.DeleteAsset(string.Format("Assets/Resources/Localization/{0}.asset", data.name));

            if (!AssetDatabase.IsValidFolder("Assets/Resources"))
                AssetDatabase.CreateFolder("Assets", "Resources");
            if (!AssetDatabase.IsValidFolder("Assets/Resources/Localization"))
                AssetDatabase.CreateFolder("Assets/Resources", "Localization");
            int count = jsonData.Count;
            var keys = new List<string>();
            var langueges = new List<string>();
            foreach (var key in jsonData[0].Keys)
            {
                if (key != "key")
                    langueges.Add(key);
            }
            for (int i = 0; i < count; i++)
            {
                var data = jsonData[i];
                keys.Add(data["key"]);
            }

            var newDatas = new Dictionary<string, LocalizationData>();
            foreach (var languege in langueges)
            {
                var newData = CreateInstance<LocalizationData>();
                AssetDatabase.CreateAsset(newData, "Assets/Resources/Localization/" + languege + ".asset");
                newDatas.Add(languege, newData);
            }

            for (int i = 0; i < count; i++)
            {
                var data = jsonData[i];
                foreach (var languege in langueges)
                    newDatas[languege].strings.Add(data["key"], data[languege].Value.ToLower());
            }

            AssetDatabase.Refresh();

            UpdateLocalizationData();
            LocalizationSettingsEditorWindow.UpdateLocalizedObjects();
        }

        private void ImportFromCSV()
        {
            EditorUtility.DisplayDialog("Import from .csv", "This feature is on development", "Ok");
            return;

            var path = EditorUtility.OpenFilePanel("Import from .csv", "", "csv");

            if (string.IsNullOrEmpty(path))
                return;

            var fileData = System.IO.File.ReadAllText(path, System.Text.Encoding.UTF8);
            var csvData = ParseCSV(fileData);

            targetData.strings.Clear();
            foreach (var data in csvData)
            {
                targetData.strings.Add(data["key"], data["string"].Replace("\\r\\n", "\r\n"));
            }

            //EditorGUIUtility.hotControl = -1;
            EditorUtility.SetDirty(targetData);

            UpdateLocalizationData();
            LocalizationSettingsEditorWindow.UpdateLocalizedObjects();
        }

        private static string SPLIT_RE = @",(?=(?:[^""]*""[^""]*"")*(?![^""]*""))";
        private static string LINE_SPLIT_RE = @"\r\n|\n\r|\n|\r";
        private static char[] TRIM_CHARS = { '\"' };
        public static List<Dictionary<string, string>> ParseCSV(string data)
        {
            var list = new List<Dictionary<string, string>>();
            var lines = Regex.Split(data, LINE_SPLIT_RE);

            if (lines.Length <= 1) return list;

            var header = Regex.Split(lines[0], SPLIT_RE);
            for (var i = 1; i < lines.Length; i++)
            {
                var values = Regex.Split(lines[i], SPLIT_RE);
                if (values.Length == 0 || values[0] == "") continue;

                var entry = new Dictionary<string, string>();
                for (var j = 0; j < header.Length && j < values.Length; j++)
                {
                    string value = values[j];
                    value = value.TrimStart(TRIM_CHARS).TrimEnd(TRIM_CHARS);
                    entry[header[j]] = value;
                }
                list.Add(entry);
            }
            return list;
        }
    }
}