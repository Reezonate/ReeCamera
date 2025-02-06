using System.Collections.Generic;

namespace ReeCamera {
    public class ReeObservableList<T> {
        #region Collection

        private readonly List<T> _items = new List<T>();
        public IReadOnlyList<T> Items => _items;

        public void AddItem(T item) {
            _items.Add(item);
            NotifyListHasChanged();
        }

        public void RemoveItem(T item) {
            _items.Remove(item);
            NotifyListHasChanged();
        }

        public void AddFrom(IEnumerable<T> items) {
            _items.AddRange(items);
            NotifyListHasChanged();
        }

        public void CopyFrom(IEnumerable<T> items) {
            _items.Clear();
            _items.AddRange(items);
            NotifyListHasChanged();
        }

        public void Clear() {
            _items.Clear();
            NotifyListHasChanged();
        }

        #endregion

        #region Events

        public delegate void ListChangedDelegate(IReadOnlyList<T> list);

        private event ListChangedDelegate ListHasChangedEvent;

        public void AddStateListener(ListChangedDelegate callback) {
            ListHasChangedEvent += callback;
            callback?.Invoke(Items);
        }

        public void RemoveStateListener(ListChangedDelegate callback) {
            ListHasChangedEvent -= callback;
        }

        public void NotifyListHasChanged() {
            ListHasChangedEvent?.Invoke(Items);
        }

        #endregion
    }
}