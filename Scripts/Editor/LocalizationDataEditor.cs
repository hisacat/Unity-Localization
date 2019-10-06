using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UnityEditor
{
    [CustomEditor(typeof(LocalizationData), true)]
    public class LocalizationDataEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            if (GUILayout.Button("Open Localization Editor", GUILayout.Height(50)))
                LocalizationEditorWindow.Init().LoadLocalizationData(target.name);

            GUI.enabled = false;
            base.OnInspectorGUI();
            GUI.enabled = true;
        }
    }
}