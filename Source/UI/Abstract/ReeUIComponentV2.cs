using System;
using System.ComponentModel;
using System.Reflection;
using System.Runtime.CompilerServices;
using BeatSaberMarkupLanguage;
using BeatSaberMarkupLanguage.Attributes;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace ReeCamera.UI {
    public abstract class ReeUIComponentV2 : MonoBehaviour, INotifyPropertyChanged {
        #region Get BSML

        private static string GetBsmlForType(Type componentType) {
            if (TryReadByType(componentType, out var resource)) return resource;
            return TryReadViewDefinition(componentType, out resource) ? resource : GetErrorBSML(componentType);
        }

        private static string GetErrorBSML(Type type) {
            return $"<text text=\"Resource not found! Type: {type.Name}\" align=\"Center\"/>";
        }

        private static bool TryReadByType(Type type, out string result) {
            var targetPostfix = $".{type.Name}.bsml";

            foreach (var resourceName in Assembly.GetExecutingAssembly().GetManifestResourceNames()) {
                if (!resourceName.EndsWith(targetPostfix)) continue;
                result = Utilities.GetResourceContent(type.Assembly, resourceName);
                return true;
            }

            result = default;
            return false;
        }

        private static bool TryReadViewDefinition(Type type, out string result) {
            while (true) {
                if (type == null) {
                    result = default;
                    return false;
                }

                var viewDefinitionAttribute = type.GetCustomAttribute<ViewDefinitionAttribute>();

                if (viewDefinitionAttribute == null) {
                    type = type.BaseType;
                    continue;
                }

                result = Utilities.GetResourceContent(type.Assembly, viewDefinitionAttribute.Definition);
                return true;
            }
        }

        #endregion

        #region Instantiate

        public static T InstantiateOnSceneRoot<T>(bool parseImmediately = true) where T : ReeUIComponentV2 {
            var lastLoadedScene = SceneManager.GetSceneAt(SceneManager.sceneCount - 1);
            var sceneRoot = lastLoadedScene.GetRootGameObjects()[0].transform;
            return Instantiate<T>(sceneRoot, parseImmediately);
        }

        public static ReeUIComponentV2 Instantiate(Type type, Transform parent, bool parseImmediately = true) {
            var component = (ReeUIComponentV2)new GameObject(type.Name).AddComponent(type);
            component.Setup(parent, parseImmediately);
            return component;
        }

        public static T Instantiate<T>(Transform parent, bool parseImmediately = true) where T : ReeUIComponentV2 {
            var component = new GameObject(typeof(T).Name).AddComponent<T>();
            component.Setup(parent, parseImmediately);
            return component;
        }

        #endregion

        #region Events

        protected virtual void OnInitialize() { }

        protected virtual void OnDispose() { }

        #endregion

        #region UnityEvents

        protected virtual void OnEnable() {
            if (!IsParsed || RootNode == null) return;
            RootNode.gameObject.SetActive(true);
        }

        protected virtual void OnDisable() {
            if (!IsParsed || RootNode == null) return;
            RootNode.gameObject.SetActive(false);
        }

        protected virtual void OnDestroy() {
            if (!IsParsed) return;
            OnDispose();
            if (RootNode != null) Destroy(RootNode.gameObject);
            _state = State.Uninitialized;
        }

        #endregion

        #region Setup

        [UIValue("UIComponent"), UsedImplicitly]
        private protected virtual Transform Transform { get; set; }

        private Transform _parent;

        private void Setup(Transform parent, bool parseImmediately) {
            _parent = parent;
            Transform = transform;
            Transform.SetParent(parent, false);
            if (parseImmediately) ParseSelfIfNeeded();
            gameObject.SetActive(false);
        }

        public void SetParent(Transform parent) {
            _parent = parent;
            Transform.SetParent(parent, false);
        }

        #endregion

        #region State

        private State _state = State.Uninitialized;

        protected bool IsParsed => _state == State.HierarchySet;

        private enum State {
            Uninitialized,
            Parsing,
            Parsed,
            HierarchySet
        }

        #endregion

        #region ManualInit

        public void ManualInit(Transform rootNode) {
            DisposeIfNeeded();
            transform.SetParent(rootNode, true);
            ApplyHierarchy();
            OnInitialize();
        }

        #endregion

        #region Parse

        protected Transform RootNode { get; private set; }

        [UIAction("#post-parse"), UsedImplicitly]
        private protected virtual void PostParse() {
            if (_state == State.Parsing) return;
            DisposeIfNeeded();
            ParseSelfIfNeeded();
            ApplyHierarchy();
            OnInitialize();
        }

        private void DisposeIfNeeded() {
            if (_state != State.HierarchySet) return;
            OnDispose();
            _state = State.Uninitialized;
        }

        private void ParseSelfIfNeeded() {
            if (_state != State.Uninitialized) return;
            _state = State.Parsing;
            BSMLParser.instance.Parse(GetBsmlForType(GetType()), gameObject, this);
            RootNode = transform.GetChild(0);
            _state = State.Parsed;
        }

        private void ApplyHierarchy() {
            if (_state != State.Parsed) throw new Exception("Component isn't parsed!");

            var child = Transform.GetChild(0);
            child.SetParent(Transform.parent, true);

            Transform.SetParent(_parent, false);
            gameObject.SetActive(true);
            _state = State.HierarchySet;
        }

        #endregion

        #region NotifyPropertyChanged

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected void NotifyPropertyChanged([CallerMemberName] string propertyName = null) {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion
    }
}