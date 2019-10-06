using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UnityEditor
{
    [CustomEditor(typeof(LocalizationSettings), true)]
    public class LocalizationSettingsEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            if (GUILayout.Button("Open Localization Setting", GUILayout.Height(50)))
                LocalizationSettingsEditorWindow.Init();

            GUI.enabled = false;
            base.OnInspectorGUI();
            GUI.enabled = true;
        }
    }
}
