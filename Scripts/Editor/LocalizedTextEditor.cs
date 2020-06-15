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
                if (Application.isPlaying)
                    Destroy(curText);
                else
                    Undo.DestroyObjectImmediate(curText);

                var curLocalizedText = Undo.AddComponent<LocalizedText>(text);

                curLocalizedText.localizationKey = curText.text;

                curLocalizedText.font = curText.font;
                curLocalizedText.fontStyle = curText.fontStyle;
                curLocalizedText.fontSize = curText.fontSize;
                curLocalizedText.lineSpacing = curText.lineSpacing;
                curLocalizedText.supportRichText = curText.supportRichText;
                curLocalizedText.alignment = curText.alignment;
                curLocalizedText.alignByGeometry = curText.alignByGeometry;
                curLocalizedText.horizontalOverflow = curText.horizontalOverflow;
                curLocalizedText.verticalOverflow = curText.verticalOverflow;
                curLocalizedText.resizeTextForBestFit = curText.resizeTextForBestFit;
                curLocalizedText.color = curText.color;
                curLocalizedText.material = curText.material;
                curLocalizedText.raycastTarget = curText.raycastTarget;

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
                if (Application.isPlaying)
                    Destroy(curLocalizedText);
                else
                    Undo.DestroyObjectImmediate(curLocalizedText);

                var curText = Undo.AddComponent<Text>(localizedText);

                curText.text = curLocalizedText.text;

                curText.font = curLocalizedText.font;
                curText.fontStyle = curLocalizedText.fontStyle;
                curText.fontSize = curLocalizedText.fontSize;
                curText.lineSpacing = curLocalizedText.lineSpacing;
                curText.supportRichText = curLocalizedText.supportRichText;
                curText.alignment = curLocalizedText.alignment;
                curText.alignByGeometry = curLocalizedText.alignByGeometry;
                curText.horizontalOverflow = curLocalizedText.horizontalOverflow;
                curText.verticalOverflow = curLocalizedText.verticalOverflow;
                curText.resizeTextForBestFit = curLocalizedText.resizeTextForBestFit;
                curText.color = curLocalizedText.color;
                curText.material = curLocalizedText.material;
                curText.raycastTarget = curLocalizedText.raycastTarget;

                EditorUtility.SetDirty(localizedText);
                EditorSceneManager.MarkAllScenesDirty();
            }

        }
    }
}