namespace ReeCamera {
    public class CyclicBuffer<T> {
        public CyclicBuffer(int capacity) {
            _capacity = capacity;
            _array = new T[capacity];
        }

        private readonly T[] _array;

        private readonly int _capacity;
        private int _nextElementIndex;
        private int _lastIndex;
        private int _firstIndex;
        public int Size;

        public bool Add(T point) {
            _lastIndex = _nextElementIndex;
            _array[_lastIndex] = point;

            _nextElementIndex += 1;
            if (_nextElementIndex >= _capacity) _nextElementIndex = 0;

            if (Size < _capacity) {
                _firstIndex = _lastIndex - Size;
                if (_firstIndex < 0) _firstIndex = _capacity + _firstIndex;
                Size += 1;
                return false;
            }

            _firstIndex += 1;
            if (_firstIndex >= _capacity) _firstIndex = 0;
            return true;
        }

        public T this[int index] {
            get {
                index += _firstIndex;
                if (index > _capacity - 1) index -= _capacity;
                return _array[index];
            }
            set {
                index += _firstIndex;
                if (index > _capacity - 1) index -= _capacity;
                _array[index] = value;
            }
        }
    }
}