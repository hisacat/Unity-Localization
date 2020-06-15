using System.Collections.Generic;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.UI;

namespace UnityEditor.UI
{
    [CustomEditor(typeof(LocalizedText), true)]
    [CanEditMultipleObjects]
    public class LocalizedTextEditor : GraphicEditor
    {
        protected SerializedProperty m_LocalizationKey;
        protected SerializedProperty m_FontData;

        protected override void OnEnable()
        {
            base.OnEnable();
            m_LocalizationKey = serializedObject.FindProperty("m_LocalizationKey");
            m_FontData = serializedObject.FindProperty("m_FontData");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            EditorGUILayout.PropertyField(m_LocalizationKey);
            EditorGUILayout.PropertyField(m_FontData);
            AppearanceControlsGUI();
            serializedObject.ApplyModifiedProperties();
        }

        [MenuItem("Localization/Text to Localized Text", true)]
        static bool ValidateTextToLocalizedText()
        {
            var objs = Selection.objects;
            foreach (var obj in objs)
            {
                if (obj.GetType() != typeof(GameObject))
                    return false;
                else
                {
                    if ((obj as GameObject).GetComponent<Text>() == null)
                        return false;
                    else
                    {
                        var text = (obj as GameObject).GetComponent<Text>();
                        if (text.GetType() != typeof(Text))
                            return false;
                    }
                }
            }
            return objs.Length <= 0 ? false : true;
        }

        [MenuItem("Localization/Text to Localized Text")]
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
                    if ((obj as GameObject).GetComponent<Text>() != null)
                        texts.Add(obj as GameObject);
                }
            }

            foreach (var text in texts)
            {
                var curText = text.GetComponent<Text>();
                ReplaceComponent.Replace<LocalizedText>(curText);

                EditorUtility.SetDirty(text);
                EditorSceneManager.MarkAllScenesDirty();
            }
        }

        [MenuItem("Localization/Localized Text to Text", true)]
        static bool ValidateLocalizedTextToText()
        {
            var objs = Selection.objects;
            foreach (var obj in objs)
            {
                if (obj.GetType() != typeof(GameObject))
                    return false;
                else
                {
                    if ((obj as GameObject).GetComponent<LocalizedText>() == null)
                        return false;
                }
            }
            return objs.Length <= 0 ? false : true;
        }

        [MenuItem("Localization/Localized Text to Text")]
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
                    if ((obj as GameObject).GetComponent<LocalizedText>() != null)
                        localizedTexts.Add(obj as GameObject);
                }
            }

            foreach (var localizedText in localizedTexts)
            {
                var curLocalizedText = localizedText.GetComponent<LocalizedText>();
                ReplaceComponent.Replace<Text>(curLocalizedText);

                EditorUtility.SetDirty(localizedText);
                EditorSceneManager.MarkAllScenesDirty();
            }

        }
    }
}