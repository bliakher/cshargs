using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;

namespace CShargs
{
    internal class ListReader<T>
    {

        int position_;
        IList<T> items_;

        public int Position => position_;
        public bool EndOfList => position_ >= items_.Count;

        public ListReader(IList<T> items)
        {
            ThrowIf.ArgumentNull(nameof(items), items);
            items_ = items;
            position_ = 0;
        }

        public T Read()
        {
            T result = Peek();
            if (!EndOfList) {
                position_++;
            }

            return result;
        }

        public T Peek() => Peek(0);
        public T Peek(int amount)
        {
            int i = position_ + amount;
            if (i >= 0 && i < items_.Count) {
                return items_[i];
            }

            return default;
        }

        public IEnumerable<T> ReadToEnd()
        {
            var result = items_.Skip(position_);
            position_ = items_.Count;

            return result;
        }
    }
}