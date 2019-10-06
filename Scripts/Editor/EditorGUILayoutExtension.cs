using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UnityEditor
{
    public static class EditorGUILayoutExtension
    {
        public static Object DisabledObjectField(string label, Object obj, System.Type objType, bool allowSceneObjects)
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField(label, GUILayout.Width(146));
            GUI.enabled = false;
            obj = EditorGUILayout.ObjectField(obj, typeof(LocalizationData), allowSceneObjects) as LocalizationData;
            GUI.enabled = true;
            EditorGUILayout.EndHorizontal();

            return obj;
        }
    }
}