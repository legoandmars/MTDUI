#nullable enable

using MTDUI.Data;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace MTDUI.UI
{
    public class ModOptionComponent : MonoBehaviour
    {
        private Button? _button = null;
        private TMP_Text? _text = null;
        private ModConfigEntry? _modConfigEntry = null;

        void Awake()
        {
            _button = GetComponent<Button>();
            _text = GetComponentInChildren<TextMeshProUGUI>(true);

            _button.onClick.AddListener(new UnityAction(OnClick));
            SetText();
        }

        void OnDestroy()
        {
            if (_button != null) _button.onClick.RemoveAllListeners();
        }

        public void Initialize(ModConfigEntry modConfigEntry)
        {
            _modConfigEntry = modConfigEntry;

            SetText();
        }

        private void SetText()
        {
            // todo: localization support (if people want to do that for mods?)
            if (_text != null && _modConfigEntry != null) _text.text = $"{_modConfigEntry.EntryConfigBase.Definition.Key}: {_modConfigEntry.EntryConfigBase.BoxedValue}";
        }

        private void OnClick()
        {
            try
            {
                // this should work for both integers and enums because you can cast enums to integers
                if (_modConfigEntry != null && (_modConfigEntry.EntryConfigBase.SettingType == typeof(int) || _modConfigEntry.EntryConfigBase.SettingType.IsEnum))
                {
                    var currentValue = (int)_modConfigEntry.EntryConfigBase.BoxedValue;
                    var valueIndex = _modConfigEntry.AcceptableValues.FindIndex(x => (int)x == currentValue);
                    if (valueIndex == -1) valueIndex = 0;

                    if (valueIndex + 1 < _modConfigEntry.AcceptableValues.Count) _modConfigEntry.EntryConfigBase.BoxedValue = _modConfigEntry.AcceptableValues[valueIndex + 1];
                    else _modConfigEntry.EntryConfigBase.BoxedValue = _modConfigEntry.AcceptableValues[0];
                }

                SetText();
            }
            catch (Exception ex)
            {
                Debug.Log(ex);
                Debug.Log(ex.StackTrace);
            }
        }
    }
}
