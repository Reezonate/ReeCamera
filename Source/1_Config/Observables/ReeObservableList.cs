using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace ReeCamera {
    public class ReeObservableList<T> {
        #region JsonConverter

        public class Converter : JsonConverter<ReeObservableList<T>> {
            public override void WriteJson(JsonWriter writer, ReeObservableList<T> value, JsonSerializer serializer) {
                serializer.Serialize(writer, value.Items);
            }

            public override ReeObservableList<T> ReadJson(
                JsonReader reader,
                Type objectType,
                ReeObservableList<T> existingValue,
                bool hasExistingValue,
                JsonSerializer serializer
            ) {
                var items = serializer.Deserialize<T[]>(reader);
                if (!hasExistingValue) {
                    var result = new ReeObservableList<T>();
                    result.AddFrom(items);
                    return result;
                }

                existingValue.AddFrom(items);
                return existingValue;
            }
        }

        #endregion

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