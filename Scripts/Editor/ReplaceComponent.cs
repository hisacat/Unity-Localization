using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

internal static class ReplaceComponent
{
    internal static void Replace<T>(Component old) where T : Component
    {
        SerializedObject so = new SerializedObject(old);
        SerializedProperty scriptProperty = so.FindProperty("m_Script");
        so.Update();

        MonoScript newScript = null;
        string[] guids = AssetDatabase.FindAssets("t:monoscript");
        foreach (string guid in guids)
        {
            var path = AssetDatabase.GUIDToAssetPath(guid);
            if (System.IO.Path.GetFileNameWithoutExtension(path) == typeof(T).Name)
            {
                var asset = AssetDatabase.LoadAssetAtPath(path, typeof(MonoScript));
                if (asset != null)
                {
                    var script = asset as MonoScript;
                    if (script.GetClass() == typeof(T))
                    {
                        newScript = script;
                        break;
                    }
                }
            }
        }
        if (newScript == null)
        {
            Debug.LogError("Cannot find script " + typeof(T).Name);
            return;
        }
        scriptProperty.objectReferenceValue = newScript;
        so.ApplyModifiedProperties();
        return;

        /*
        var go = old.gameObject;
        var rt = go.transform as RectTransform;

        var anchorMin = rt.anchorMin; var anchorMax = rt.anchorMax;
        var offsetMin = rt.offsetMin; var offsetMax = rt.offsetMax;

        SerializedProperty prop_iterator = new SerializedObject(old).GetIterator();
        if (Application.isPlaying) Destroy(old);
        else Undo.DestroyObjectImmediate(old);

        var newComponent = Undo.AddComponent<T>(go);
        SerializedObject dest = new SerializedObject(newComponent);
        if (prop_iterator.NextVisible(true))
        {
            while (prop_iterator.NextVisible(true))
            {
                SerializedProperty prop_element = dest.FindProperty(prop_iterator.name);
                if (prop_element != null)
                {
                    //if (prop_element.propertyType == prop_iterator.propertyType)
                    dest.CopyFromSerializedProperty(prop_iterator);
                }
            }
        }

        //tmpro rt bug fix
        rt.anchorMin = anchorMin; rt.anchorMax = anchorMax;
        rt.offsetMin = offsetMin; rt.offsetMax = offsetMax;

        dest.ApplyModifiedProperties();

        return newComponent;
        */
    }
}
