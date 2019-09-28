using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;

[CreateAssetMenu]
public class LocalizationData : ScriptableObject
{
    [SerializeField]
    protected LocalizedStringDictionary m_LocalizedStringDictionary = new LocalizedStringDictionary();
    public LocalizedStringDictionary strings { get { return m_LocalizedStringDictionary; } }

    [SerializeField]
    protected LocalizedSpriteDictionary m_localizedSpriteDictionary = new LocalizedSpriteDictionary();
    public LocalizedSpriteDictionary sprites { get { return m_localizedSpriteDictionary; } }

    [Serializable]
    public class LocalizedStringDictionary : SerializableDictionary<string, string> { }

    [Serializable]
    public class LocalizedSpriteDictionary : SerializableDictionary<string, Sprite> { }
}