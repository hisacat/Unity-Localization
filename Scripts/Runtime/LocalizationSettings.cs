using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LocalizationSettings : ScriptableObject
{
    [SerializeField]
    protected string m_CurrentLanguage = "";
    public string currentLanguage { get { return m_CurrentLanguage; } set { m_CurrentLanguage = value; } }

    [SerializeField]
    protected string m_LanguageInsteadMissing = "";
    public string languageInsteadMissing { get { return m_LanguageInsteadMissing; } set { m_LanguageInsteadMissing = value; } }

    [SerializeField]
    protected bool m_UsingRuntimeData = false;
    public bool UsingRuntimeData { get { return m_UsingRuntimeData; } set { m_UsingRuntimeData = value; } }
}