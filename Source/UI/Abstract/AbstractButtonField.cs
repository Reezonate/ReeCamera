using BeatSaberMarkupLanguage.Attributes;
using JetBrains.Annotations;
using TMPro;
using UnityEngine.UI;

namespace ReeCamera.UI {
    [ViewDefinition(Plugin.ResourcesPath + ".BSML.Inputs.Abstract.AbstractButtonField.bsml")]
    public abstract class AbstractButtonField<TV, TC> : AbstractInputField<TV, TC> where TC : InputContext<TV> {
        #region Components

        [UIComponent("Button"), UsedImplicitly]
        private protected Button Button;

        [UIAction("ButtonOnClick"), UsedImplicitly]
        protected void ButtonOnClick() {
            OnEditButtonWasClicked();
        }

        private TextMeshProUGUI _buttonText;

        protected override void OnInitialize() {
            base.OnInitialize();

            _buttonText = Button.gameObject.GetComponentInChildren<TextMeshProUGUI>();
            _buttonText.fontStyle = FontStyles.Italic;

            UpdateText();
        }

        protected abstract override void OnEditButtonWasClicked();

        #endregion

        #region Context

        protected override void OnContextChanged() {
            base.OnContextChanged();
            UpdateText();
        }

        protected override void OnValueChanged(TV value, ObservableValueState state) {
            UpdateText();
        }

        #endregion

        #region Text

        private void UpdateText() {
            if (!IsParsed || Context == null) return;
            _buttonText.text = FormatValue(Context.Value);
        }

        protected abstract string FormatValue(TV value);

        #endregion
    }
}