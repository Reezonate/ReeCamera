using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace ReeCamera {
    [JsonObject]
    public class ObservableValue<T> : IObservableValue<T> {
        #region JsonConverter

        public class Converter : JsonConverter<ObservableValue<T>> {
            public override void WriteJson(JsonWriter writer, ObservableValue<T> value, JsonSerializer serializer) {
                serializer.Serialize(writer, value.Value);
            }

            public override ObservableValue<T> ReadJson(
                JsonReader reader,
                Type objectType,
                ObservableValue<T> existingValue,
                bool hasExistingValue,
                JsonSerializer serializer
            ) {
                var value = serializer.Deserialize<T>(reader);
                if (!hasExistingValue) return new ObservableValue<T>(value);
                existingValue.SetValue(value, existingValue);
                return existingValue;
            }
        }

        public ObservableValue() { }

        public ObservableValue(T initialValue) {
            SetValue(initialValue, this);
        }

        #endregion

        #region Value

        public T Value { get; private set; }

        [JsonIgnore]
        public ObservableValueState State { get; private set; } = ObservableValueState.Uninitialized;

        [JsonIgnore]
        public T LastFinalValue { get; private set; }

        [JsonIgnore]
        public object LastChangeSource { get; private set; }

        public void SetValue(T newValue, object source) {
            SetValue(newValue, ObservableValueState.Final, source);
        }

        public void PreviewValue(T newValue, object source) {
            SetValue(newValue, ObservableValueState.Preview, source);
        }

        public void CancelPreview(object source) {
            SetValue(LastFinalValue, ObservableValueState.Final, source);
        }

        public void SetValue(T newValue, ObservableValueState newState, object source) {
            if (EqualsFast(newValue, newState)) return;
            LastChangeSource = source;
            Value = newValue;
            State = newState;
            if (newState == ObservableValueState.Final) LastFinalValue = newValue;
            NotifyStateChanged();
        }

        private bool EqualsFast(T newValue, ObservableValueState newState) {
            return State == newState && Equals(Value, newValue);
        }

        #endregion

        #region Listeners

        [JsonIgnore]
        private readonly Dictionary<StateListenerDelegate<T>, StateListener> _stateListeners =
            new Dictionary<StateListenerDelegate<T>, StateListener>();

        public void AddStateListener(StateListenerDelegate<T> listener, object ignoreSource) {
            var stateListener = new StateListener(listener, ignoreSource);
            _stateListeners[listener] = stateListener;
            InvokeIfNecessary(stateListener);
        }

        public void RemoveStateListener(StateListenerDelegate<T> listener) {
            if (!_stateListeners.ContainsKey(listener)) return;
            _stateListeners.Remove(listener);
        }

        private readonly struct StateListener {
            public readonly StateListenerDelegate<T> Action;
            public readonly object Source;

            public StateListener(StateListenerDelegate<T> action, object source) {
                Action = action;
                Source = source;
            }
        }

        #endregion

        #region Event

        private void NotifyStateChanged() {
            foreach (var stateListener in _stateListeners.Values) {
                InvokeIfNecessary(stateListener);
            }
        }

        private void InvokeIfNecessary(StateListener listener) {
            if (State is ObservableValueState.Uninitialized || listener.Source == LastChangeSource) return;
            listener.Action?.Invoke(Value, State);
        }

        #endregion
    }
}