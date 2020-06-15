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
                if (Application.isPlaying)
                    Destroy(curText);
                else
                    Undo.DestroyObjectImmediate(curText);

                RectTransform rt = text.transform as RectTransform;
                var anchorMin = rt.anchorMin; var anchorMax = rt.anchorMax;
                var offsetMin = rt.offsetMin; var offsetMax = rt.offsetMax;
                var curLocalizedText = Undo.AddComponent<LocalizationTextMeshProUGUI>(text);
                rt.anchorMin = anchorMin; rt.anchorMax = anchorMax;
                rt.offsetMin = offsetMin; rt.offsetMax = offsetMax;

                curLocalizedText.localizationKey = curText.text;

                curLocalizedText.font = curText.font;
                curLocalizedText.fontStyle = curText.fontStyle;
                curLocalizedText.fontSize = curText.fontSize;
                curLocalizedText.enableAutoSizing = curText.enableAutoSizing;
                curLocalizedText.color = curText.color;
                curLocalizedText.colorGradient = curText.colorGradient;
                curLocalizedText.colorGradientPreset = curText.colorGradientPreset;
                curLocalizedText.overrideColorTags = curText.overrideColorTags;
                curLocalizedText.characterSpacing = curText.characterSpacing;
                curLocalizedText.wordSpacing = curText.wordSpacing;
                curLocalizedText.lineSpacing = curText.lineSpacing;
                curLocalizedText.paragraphSpacing = curText.paragraphSpacing;
                curLocalizedText.alignment = curText.alignment;
                curLocalizedText.overflowMode = curText.overflowMode;
                curLocalizedText.pageToDisplay = curText.pageToDisplay;
                curLocalizedText.isLinkedTextComponent = curText.isLinkedTextComponent;
                curLocalizedText.linkedTextComponent = curText.linkedTextComponent;
                curLocalizedText.mappingUvLineOffset = curText.mappingUvLineOffset;
                curLocalizedText.enableWordWrapping = curText.enableWordWrapping;
                curLocalizedText.wordWrappingRatios = curText.wordWrappingRatios;
                curLocalizedText.horizontalMapping = curText.horizontalMapping;
                curLocalizedText.verticalMapping = curText.verticalMapping;
                curLocalizedText.margin = curText.margin;
                curLocalizedText.geometrySortingOrder = curText.geometrySortingOrder;
                curLocalizedText.richText = curText.richText;
                curLocalizedText.raycastTarget = curText.raycastTarget;
                curLocalizedText.parseCtrlCharacters = curText.parseCtrlCharacters;
                curLocalizedText.useMaxVisibleDescender = curText.useMaxVisibleDescender;
                curLocalizedText.spriteAsset = curText.spriteAsset;
                curLocalizedText.enableKerning = curText.enableKerning;
                curLocalizedText.extraPadding = curText.extraPadding;

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
                var curLocalizedText = localizedText.GetComponent<LocalizationTextMeshProUGUI>();
                if (Application.isPlaying)
                    Destroy(curLocalizedText);
                else
                    Undo.DestroyObjectImmediate(curLocalizedText);

                RectTransform rt = localizedText.transform as RectTransform;
                var anchorMin = rt.anchorMin; var anchorMax = rt.anchorMax;
                var offsetMin = rt.offsetMin; var offsetMax = rt.offsetMax;
                var curText = Undo.AddComponent<TextMeshProUGUI>(localizedText);
                rt.anchorMin = anchorMin; rt.anchorMax = anchorMax;
                rt.offsetMin = offsetMin; rt.offsetMax = offsetMax;

                curText.text = curLocalizedText.localizationKey;

                curText.font = curLocalizedText.font;
                curText.fontStyle = curLocalizedText.fontStyle;
                curText.fontSize = curLocalizedText.fontSize;
                curText.enableAutoSizing = curLocalizedText.enableAutoSizing;
                curText.color = curLocalizedText.color;
                curText.colorGradient = curLocalizedText.colorGradient;
                curText.colorGradientPreset = curLocalizedText.colorGradientPreset;
                curText.overrideColorTags = curLocalizedText.overrideColorTags;
                curText.characterSpacing = curLocalizedText.characterSpacing;
                curText.wordSpacing = curLocalizedText.wordSpacing;
                curText.lineSpacing = curLocalizedText.lineSpacing;
                curText.paragraphSpacing = curLocalizedText.paragraphSpacing;
                curText.alignment = curLocalizedText.alignment;
                curText.overflowMode = curLocalizedText.overflowMode;
                curText.pageToDisplay = curLocalizedText.pageToDisplay;
                curText.isLinkedTextComponent = curLocalizedText.isLinkedTextComponent;
                curText.linkedTextComponent = curLocalizedText.linkedTextComponent;
                curText.mappingUvLineOffset = curLocalizedText.mappingUvLineOffset;
                curText.enableWordWrapping = curLocalizedText.enableWordWrapping;
                curText.wordWrappingRatios = curLocalizedText.wordWrappingRatios;
                curText.horizontalMapping = curLocalizedText.horizontalMapping;
                curText.verticalMapping = curLocalizedText.verticalMapping;
                curText.margin = curLocalizedText.margin;
                curText.geometrySortingOrder = curLocalizedText.geometrySortingOrder;
                curText.richText = curLocalizedText.richText;
                curText.raycastTarget = curLocalizedText.raycastTarget;
                curText.parseCtrlCharacters = curLocalizedText.parseCtrlCharacters;
                curText.useMaxVisibleDescender = curLocalizedText.useMaxVisibleDescender;
                curText.spriteAsset = curLocalizedText.spriteAsset;
                curText.enableKerning = curLocalizedText.enableKerning;
                curText.extraPadding = curLocalizedText.extraPadding;

                EditorUtility.SetDirty(localizedText);
                EditorSceneManager.MarkAllScenesDirty();
            }
        }
    }
}