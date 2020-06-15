using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UnityEditor
{
    public class CreateNewLocalizationEditorWindow : EditorWindow
    {
        public static void Init()
        {
            CreateNewLocalizationEditorWindow window = (CreateNewLocalizationEditorWindow)GetWindow(typeof(CreateNewLocalizationEditorWindow), true);
            window.minSize = window.maxSize = new Vector2(370, 270);

            Texture icon = EditorGUIUtility.IconContent(EditorGUIUtility.isProSkin ? "d_AlphabeticalSorting" : "AlphabeticalSorting").image;
            window.titleContent = new GUIContent("Add new Language", icon);
            window.Show();
        }

        private string languageCode = "";

        private bool isCopyFromExistingData = true;
        private bool copyStringKeys = true;
        private bool copyStringDatas = true;
        private bool copySpriteKeys = true;
        private bool copySpriteDatas = true;
        private int selectedLanguageIndex = 0;

        private string[] illegalCharacters = { "\\", "/", ";", "*", "?", "\"", "<", ">", "|" };

        void OnGUI()
        {
            GUILayout.Label("Add new Localization Language", EditorStyles.boldLabel);

            languageCode = EditorGUILayout.TextField("Language Code", languageCode).ToLower();

            #region Getting all localizationData
            var datas = new List<LocalizationData>();
            {
                var allAssets = Resources.LoadAll("Localization/");
                foreach (var asset in allAssets)
                {
                    if (asset.GetType() == typeof(LocalizationData))
                        datas.Add(asset as LocalizationData);
                }
            }
            #endregion Getting all localizationData

            bool languageCodeAlreadyExist = datas.Exists((x) => { return x.name.ToLower() == languageCode; });

            GUILayout.BeginHorizontal();
            {
                GUILayout.Space(154);

                bool findIlligalCharacter = false;
                foreach (var illigalCharacter in illegalCharacters)
                {
                    if (languageCode.Contains(illigalCharacter))
                    {
                        findIlligalCharacter = true;
                        break;
                    }
                }

                if (findIlligalCharacter)
                {
                    GUIStyle style = new GUIStyle(EditorStyles.label);
                    style.normal.textColor = Color.red;
                    GUILayout.Label("Language code can be contains\r\nthese characters \\ / ; * ? \" < > |", style);
                }
                else if (string.IsNullOrEmpty(languageCode))
                {
                    GUIStyle style = new GUIStyle(EditorStyles.label);
                    style.normal.textColor = Color.red;
                    GUILayout.Label("Language code can not be empty!", style);
                }
                else if (languageCodeAlreadyExist)
                {
                    GUIStyle style = new GUIStyle(EditorStyles.label);
                    style.normal.textColor = Color.red;
                    GUILayout.Label("This language code is already exist!", style);
                }
                else
                    GUILayout.Label("You can use this language code");
            }
            GUILayout.EndHorizontal();

            GUILayout.Label("Data Settings", EditorStyles.boldLabel);

            #region Copy from existing Data Settings
            isCopyFromExistingData = EditorGUILayout.ToggleLeft("Copy from Existing Data", isCopyFromExistingData);
            if (!isCopyFromExistingData)
            {
                copyStringKeys = false;
                copyStringDatas = false;
                copySpriteKeys = false;
                copySpriteDatas = false;
            }

            string[] languages = new string[datas.Count];
            GUI.enabled = isCopyFromExistingData;
            {
                #region Draw current exist language dropdown

                for (int i = 0; i < datas.Count; i++)
                    languages[i] = datas[i].name;

                if (selectedLanguageIndex >= languages.Length)
                    selectedLanguageIndex = languages.Length - 1;

                EditorGUI.BeginChangeCheck();
                GUILayout.BeginHorizontal();
                GUILayout.Space(25);
                selectedLanguageIndex = EditorGUILayout.Popup("Copy from", selectedLanguageIndex, languages);
                GUILayout.EndHorizontal();
                #endregion Draw current exist language dropdown

                GUILayout.BeginHorizontal();
                GUILayout.Space(25);
                GUILayout.Label("Strings", EditorStyles.boldLabel);
                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal();
                {
                    GUILayout.Space(25);
                    GUILayout.Space(25);
                    copyStringKeys = EditorGUILayout.ToggleLeft("Copy String Keys", copyStringKeys, GUILayout.Width(130));
                    GUI.enabled = copyStringKeys;
                    copyStringDatas = EditorGUILayout.ToggleLeft("Copy Data", copyStringDatas, GUILayout.Width(130));
                    if (!copyStringKeys)
                        copyStringDatas = false;
                    GUI.enabled = isCopyFromExistingData;
                }
                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal();
                GUILayout.Space(25);
                GUILayout.Label("Sprites", EditorStyles.boldLabel);
                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal();
                {
                    GUILayout.Space(25);
                    GUILayout.Space(25);
                    copySpriteKeys = EditorGUILayout.ToggleLeft("Copy Sprite Keys", copySpriteKeys, GUILayout.Width(130));
                    GUI.enabled = copySpriteKeys;
                    copySpriteDatas = EditorGUILayout.ToggleLeft("Copy Data", copySpriteDatas, GUILayout.Width(130));
                    if (!copySpriteKeys)
                        copySpriteDatas = false;
                    GUI.enabled = isCopyFromExistingData;
                }
                GUILayout.EndHorizontal();
            }
            GUI.enabled = true;
            #endregion Copy from existing Data Settings

            if (string.IsNullOrEmpty(languageCode))
                GUI.enabled = false;
            else
                GUI.enabled = !languageCodeAlreadyExist;

            EditorGUILayout.Space();
            if (GUILayout.Button("Create", GUILayout.Height(50)))
            {
                var data = CreateInstance<LocalizationData>();
                if (isCopyFromExistingData && selectedLanguageIndex >= 0)
                {
                    var baseAsset = Resources.Load<LocalizationData>("Localization/" + languages[selectedLanguageIndex]);
                    if (baseAsset != null)
                    {
                        if (copyStringKeys)
                        {
                            foreach (var key in baseAsset.strings.Keys)
                                data.strings.Add(key, null);

                            if (copyStringDatas)
                            {
                                foreach (var key in baseAsset.strings.Keys)
                                    data.strings[key] = baseAsset.strings[key];
                            }
                        }
                        if (copySpriteKeys)
                        {
                            foreach (var key in baseAsset.sprites.Keys)
                                data.sprites.Add(key, null);

                            if (copySpriteDatas)
                            {
                                foreach (var key in baseAsset.sprites.Keys)
                                    data.sprites[key] = baseAsset.sprites[key];
                            }
                        }
                    }
                }

                if (!AssetDatabase.IsValidFolder("Assets/Resources"))
                    AssetDatabase.CreateFolder("Assets", "Resources");
                if (!AssetDatabase.IsValidFolder("Assets/Resources/Localization"))
                    AssetDatabase.CreateFolder("Assets/Resources", "Localization");

                AssetDatabase.CreateAsset(data, "Assets/Resources/Localization/" + languageCode + ".asset");

                EditorUtility.DisplayDialog("Add new Localization Language", string.Format("Created Localization file \"{0}\"", languageCode), "Ok");

                Selection.activeObject = data;
                Close();
                LocalizationEditorWindow.Init().LoadLocalizationData(languageCode);
            }
            GUI.enabled = true;
        }
    }
}