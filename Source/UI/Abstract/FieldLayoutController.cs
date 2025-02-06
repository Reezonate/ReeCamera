using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace ReeCamera.UI {
    public class FieldLayoutController : UIBehaviour {
        #region Construct

        private LayoutElement _label;
        private HorizontalLayoutGroup _value;

        protected override void Awake() {
            _label = transform.GetChild(0).GetComponent<LayoutElement>();
            _value = transform.GetChild(1).GetComponent<HorizontalLayoutGroup>();
        }

        protected override void OnTransformParentChanged() {
            UpdateLayout();
        }

        protected override void OnRectTransformDimensionsChange() {
            UpdateLayout();
        }

        #endregion

        #region OnChange

        private LayoutElement _parentLayoutElement;
        private LayoutGroup _parentLayoutGroup;

        private void UpdateLayout() {
            var parent = transform.parent;
            if (parent == null) return;

            _parentLayoutElement = parent.GetComponent<LayoutElement>();
            _parentLayoutGroup = parent.GetComponent<LayoutGroup>();

            if (_label == null || _parentLayoutElement == null) return;

            var contentWidth = _parentLayoutElement.preferredWidth;

            if (_parentLayoutGroup != null) {
                contentWidth -= _parentLayoutGroup.padding.left;
                contentWidth -= _parentLayoutGroup.padding.right;
            }

            _label.preferredWidth = contentWidth - _value.preferredWidth;
        }

        #endregion
    }
}