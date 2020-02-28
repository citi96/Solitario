namespace Utility {
    public class DropOutStack<T> {
        private readonly T[] _items;
        private int _top = 0;
        private int _count;

        public DropOutStack(int capacity) {
            _items = new T[capacity];
        }

        public void Push(T item) {
            _count += 1;
            _count = _count > _items.Length ? _items.Length : _count;

            _items[_top] = item;
            _top = (_top + 1) % _items.Length;
        }

        public T Pop() {
            _count -= 1;
            _count = _count < 0 ? 0 : _count;

            _top = (_items.Length + _top - 1) % _items.Length;
            return _items[_top];
        }


        public bool IsEmpty() {
            return _count == 0;
        }
    }
}