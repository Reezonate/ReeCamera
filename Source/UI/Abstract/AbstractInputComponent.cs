namespace ReeCamera.UI {
    public abstract class AbstractInputComponent<TV, TC> : ReeUIComponentV2 where TC : InputContext<TV> {
        #region Context

        protected TC Context;
        private bool _hasContext;

        public void ClearContext() {
            UnSubscribe();
            Context = default;
            _hasContext = false;
        }

        public void SetContext(TC context) {
            if (Equals(Context, context)) return;
            UnSubscribe();
            Context = context;
            _hasContext = context != null;
            OnContextChanged();
            ReSubscribe();
        }

        protected void PreviewValue(TV value, bool notifySelf = false) {
            if (!_hasContext) return;
            Context.PreviewValue(value, notifySelf ? Context : this);
        }

        protected void ApplyValue(TV value, bool notifySelf = false) {
            if (!_hasContext) return;
            Context.SetValue(value, notifySelf ? Context : this);
        }

        protected void CancelPreview(bool notifySelf = true) {
            if (!_hasContext) return;
            Context.CancelPreview(notifySelf ? Context : this);
        }

        #endregion

        #region OnInitialize / OnDispose

        protected override void OnInitialize() {
            base.OnInitialize();
            ReSubscribe();
        }

        protected override void OnDispose() {
            UnSubscribe();
            base.OnDispose();
        }

        #endregion

        #region Events

        private void ReSubscribe() {
            if (!_hasContext) return;
            UnSubscribe();
            Context.PropertiesChanged += OnContextChanged;
            Context.AddStateListener(OnValueChanged, this);
        }

        private void UnSubscribe() {
            if (!_hasContext) return;
            Context.PropertiesChanged -= OnContextChanged;
            Context.RemoveStateListener(OnValueChanged);
        }

        protected virtual void OnValueChanged(TV value, ObservableValueState state) { }
        protected virtual void OnContextChanged() { }

        #endregion
    }
}