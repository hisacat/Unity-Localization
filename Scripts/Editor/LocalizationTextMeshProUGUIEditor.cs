using UnityEngine;
using UnityEngine.UI;
using UnityEditor;
using System.Collections.Generic;
using UnityEditor.SceneManagement;

namespace TMPro.EditorUtilities
{
    [CustomEditor(typeof(LocalizationTextMeshProUGUI), true), CanEditMultipleObjects]
    public class LocalizationTextMeshProUGUIEditor : TMP_UiEditorPanel
    {
        protected SerializedProperty m_LocalizationKey;

        protected override void OnEnable()
        {
            m_LocalizationKey = serializedObject.FindProperty("m_LocalizationKey");
            base.OnEnable();
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            EditorGUILayout.PropertyField(m_LocalizationKey);
            EditorGUILayout.Space();
            EditorGUILayout.LabelField("This is localized text. \"Text\" field will be override");
            serializedObject.ApplyModifiedProperties();

            base.OnInspectorGUI();
        }


        [MenuItem("Localization/TextMeshPro to Localized TextMeshPro", true)]
        static bool ValidateTextToLocalizedText()
        {
            var objs = Selection.objects;
            foreach (var obj in objs)
            {
                if (obj.GetType() != typeof(GameObject))
                    return false;
                else
                {
                    if ((obj as GameObject).GetComponent<TextMeshProUGUI>() == null)
                        return false;
                    else
                    {
                        var text = (obj as GameObject).GetComponent<TextMeshProUGUI>();
                        if (text.GetType() != typeof(TextMeshProUGUI))
                            return false;
                    }
                }
            }
            return objs.Length <= 0 ? false : true;
        }

        [MenuItem("Localization/TextMeshPro to Localized TextMeshPro")]
        public static void TextToLocalizedText()
        {
            var objs = Selection.objects;
            List<GameObject> texts = new List<GameObject>();
            foreach (var obj in objs)
            {
                if (obj.GetType() != typeof(GameObject))
                    continue;
                else
                {
                    if ((obj as GameObject).GetComponent<TextMeshProUGUI>() != null)
                        texts.Add(obj as GameObject);
                }
            }

            foreach (var text in texts)
            {
                var curText = text.GetComponent<TextMeshProUGUI>();
                ReplaceComponent.Replace<LocalizationTextMeshProUGUI>(curText);

                EditorUtility.SetDirty(text);
                EditorSceneManager.MarkAllScenesDirty();
            }
        }

        [MenuItem("Localization/Localized TextMeshPro to TextMeshPro", true)]
        static bool ValidateLocalizedTextToText()
        {
            var objs = Selection.objects;
            foreach (var obj in objs)
            {
                if (obj.GetType() != typeof(GameObject))
                    return false;
                else
                {
                    if ((obj as GameObject).GetComponent<LocalizationTextMeshProUGUI>() == null)
                        return false;
                }
            }
            return objs.Length <= 0 ? false : true;
        }

        [MenuItem("Localization/Localized TextMeshPro to TextMeshPro")]
        public static void LocalizedTextToText()
        {
            var objs = Selection.objects;
            List<GameObject> localizedTexts = new List<GameObject>();
            foreach (var obj in objs)
            {
                if (obj.GetType() != typeof(GameObject))
                    continue;
                else
                {
                    if ((obj as GameObject).GetComponent<LocalizationTextMeshProUGUI>() != null)
                        localizedTexts.Add(obj as GameObject);
                }
            }

            foreach (var localizedText in localizedTexts)
            {
                var curText = localizedText.GetComponent<LocalizationTextMeshProUGUI>();
                ReplaceComponent.Replace<TextMeshProUGUI>(curText);

                EditorUtility.SetDirty(localizedText);
                EditorSceneManager.MarkAllScenesDirty();
            }
        }
    }
}