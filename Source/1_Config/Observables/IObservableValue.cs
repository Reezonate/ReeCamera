namespace ReeCamera {
    public delegate void StateListenerDelegate<in T>(T value, ObservableValueState state);

    public interface IObservableValue<T> {
        #region Value

        T Value { get; }
        ObservableValueState State { get; }
        object LastChangeSource { get; }
        void SetValue(T newValue, object source);
        void PreviewValue(T newValue, object source);
        void CancelPreview(object source);

        #endregion

        #region Event

        void AddStateListener(StateListenerDelegate<T> listener, object ignoreSource);
        void RemoveStateListener(StateListenerDelegate<T> listener);

        #endregion
    }
}