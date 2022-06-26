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

        private void ChangeValue<T>(ModConfigEntry configEntry)
        {
            T currentValue = (T)configEntry.EntryConfigBase.BoxedValue;

            int valueIndex = configEntry.AcceptableValues.FindIndex(x => ((T)x).Equals(currentValue));
            if (valueIndex == -1) valueIndex = 0;

            if (valueIndex + 1 < configEntry.AcceptableValues.Count) configEntry.EntryConfigBase.BoxedValue = configEntry.AcceptableValues[valueIndex + 1];
            else configEntry.EntryConfigBase.BoxedValue = configEntry.AcceptableValues[0];
        }

        private void OnClick()
        {
            try
            {
                if (_modConfigEntry == null) return;

                var settingType = _modConfigEntry.EntryConfigBase.SettingType;

                // this should work for both integers and enums because you can cast enums to integers
                // i think the ChangeValue method *should* work generically but I need to do more testing to make sure nothing breaks
                if (settingType == typeof(int) || settingType.IsEnum) ChangeValue<int>(_modConfigEntry);
                else if (settingType == typeof(bool)) ChangeValue<bool>(_modConfigEntry);
                else if (settingType == typeof(float)) ChangeValue<float>(_modConfigEntry);

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
