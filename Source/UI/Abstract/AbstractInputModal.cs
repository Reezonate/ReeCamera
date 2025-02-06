using System;
using BeatSaberMarkupLanguage.Attributes;
using HMUI;
using JetBrains.Annotations;
using ReeSabers.UI;
using UnityEngine.UI;

namespace ReeCamera.UI {
    public abstract class AbstractInputModal<TV, TC> : AbstractInputComponent<TV, TC>, IReeModal where TC : InputContext<TV> {
        #region Components

        [UIComponent("ButtonsBackground"), UsedImplicitly]
        private protected ImageView ButtonsBackground;

        [UIComponent("CancelButton"), UsedImplicitly]
        private protected Button CancelButton;

        [UIComponent("ApplyButton"), UsedImplicitly]
        protected Button ApplyButton;

        #endregion

        #region OnInitialize / OnDispose

        protected override void OnInitialize() {
            base.OnInitialize();
            UIUtils.ApplySkew(ButtonsBackground);
        }

        #endregion

        #region Events

        [UIAction("CancelOnClick"), UsedImplicitly]
        protected void CancelOnClick() {
            CancelPreview();
            Close();
        }

        [UIAction("ApplyOnClick"), UsedImplicitly]
        protected void ApplyOnClick() {
            ApplyValue(Context.Value);
            Close();
        }

        #endregion

        #region IReeModal

        protected virtual void OnResume() { }
        protected virtual void OnPause() { }

        protected virtual void OnInterrupt() {
            CancelPreview();
        }

        private Action _closeAction;

        public void Resume(object state, Action closeAction) {
            if (state is not TC context) return;
            SetContext(context);

            _closeAction = closeAction;
            gameObject.SetActive(true);
            RootNode.gameObject.SetActive(true);
            OnResume();
        }

        public void Pause() {
            gameObject.SetActive(false);
            RootNode.gameObject.SetActive(false);
            OnPause();
        }

        public void Interrupt() {
            gameObject.SetActive(false);
            RootNode.gameObject.SetActive(false);
            OnInterrupt();
        }

        public void Close() {
            _closeAction?.Invoke();
        }

        #endregion
    }
}