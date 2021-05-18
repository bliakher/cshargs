using System;
using System.IO;
using System.Collections.Generic;

class ListReader<T> {

    int position_;
    IList<T> items_;

    public ListReader(IList<T> items) {
        items_ = items;
        position_ = 0;
    }

    public T Read() {
        if (position_ >= items_.Count) {
            return default;
        }
        return items_[position_++];
    }

    public T Peek() => Peek(0);
    public T Peek(int amount) {
        return items_[position_ + amount];
    }
}
