using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UnityEngine.UI
{
    public class LocalizedText : Text
    {
        [SerializeField]
        protected string m_LocalizationKey = String.Empty;
        public string localizationKey
        {
            get
            {
                return m_LocalizationKey;
            }
            set
            {
                if (m_LocalizationKey != value)
                {
                    m_LocalizationKey = value;
                    UpdateLocalization();
                }
            }
        }

#if UNITY_EDITOR
        private string prevLocalizationKey;
#endif

        protected override void OnEnable()
        {
            base.OnEnable();
            UpdateLocalization();
#if UNITY_EDITOR
            prevLocalizationKey = m_LocalizationKey;
#endif
        }

#if UNITY_EDITOR
        private void Update()
        {
            if (prevLocalizationKey != m_LocalizationKey)
            {
                prevLocalizationKey = m_LocalizationKey;
                UpdateLocalization();
            }
        }
#endif

        public void UpdateLocalization()
        {
            var localizedText = LocalizationManager.GetLocalizedText(m_LocalizationKey);
            if (localizedText == null) localizedText = m_LocalizationKey;
            this.text = localizedText;
        }
    }
}