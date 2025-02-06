using System;

namespace ReeCamera.UI {
    public class InputContext<TV> : IObservableValue<TV> {
        #region Values

        private readonly IObservableValue<TV> _observableValue;

        public string Header = string.Empty;
        public string Label = string.Empty;

        #endregion

        #region PropertiesChanged Event

        public event Action PropertiesChanged;

        public void NotifyPropertiesChanged() {
            PropertiesChanged?.Invoke();
        }

        #endregion

        #region Constructors

        public InputContext() {
            _observableValue = new ObservableValue<TV>();
        }

        public InputContext(TV initialValue) {
            _observableValue = new ObservableValue<TV>(initialValue);
        }

        public InputContext(IObservableValue<TV> value) {
            _observableValue = value;
        }

        #endregion

        #region IObservableValue impl

        public TV Value => _observableValue.Value;
        public ObservableValueState State => _observableValue.State;
        public object LastChangeSource => _observableValue.LastChangeSource;

        public void SetValue(TV newValue, object source) {
            _observableValue.SetValue(newValue, source);
        }

        public void PreviewValue(TV newValue, object source) {
            _observableValue.PreviewValue(newValue, source);
        }

        public void CancelPreview(object source) {
            _observableValue.CancelPreview(source);
        }

        public void AddStateListener(StateListenerDelegate<TV> listener, object ignoreSource) {
            _observableValue.AddStateListener(listener, ignoreSource);
        }

        public void RemoveStateListener(StateListenerDelegate<TV> listener) {
            _observableValue.RemoveStateListener(listener);
        }

        #endregion
    }
}