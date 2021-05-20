using System;
using System.IO;
using System.Collections.Generic;

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
            if (EndOfList) {
                return default;
            }
            return items_[position_ + amount];
        }
    }
}