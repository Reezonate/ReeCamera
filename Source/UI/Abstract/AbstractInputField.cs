using BeatSaberMarkupLanguage.Attributes;
using BeatSaberMarkupLanguage.Components;
using JetBrains.Annotations;
using TMPro;

namespace ReeCamera.UI {
    public class AbstractInputField<TV, TC> : AbstractInputComponent<TV, TC> where TC : InputContext<TV> {
        #region Components

        [UIComponent("FieldLabelComponent"), UsedImplicitly]
        protected TextMeshProUGUI LabelComponent;

        [UIComponent("EditIcon"), UsedImplicitly]
        protected ClickableImage EditIcon;

        protected FieldLayoutController LayoutController { get; private set; }

        protected override void OnInitialize() {
            base.OnInitialize();

            LabelComponent.alignment = TextAlignmentOptions.MidlineLeft;
            LabelComponent.overflowMode = TextOverflowModes.Ellipsis;
            LabelComponent.enableWordWrapping = false;
            LabelComponent.fontSize = 4.5f;

            LayoutController = RootNode.gameObject.AddComponent<FieldLayoutController>();
            InitializeEditIcon();
        }

        #endregion

        #region EditIcon

        [UIAction("EditIconOnClick")]
        protected void EditIconOnClick() {
            OnEditButtonWasClicked();
        }

        private void InitializeEditIcon() {
            if (EditIcon == null) return;
            SmoothHoverController.Scale(EditIcon.gameObject, 0.8f, 1.0f);
        }

        protected virtual void OnEditButtonWasClicked() { }

        #endregion

        #region Context

        protected override void OnContextChanged() {
            LabelComponent.text = Context.Label;
        }

        #endregion
    }
}