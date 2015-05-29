using System;
using System.Collections.Generic;

namespace Marea
{
    public interface Variable<T> : Primitive
    {
        void Notify(ServiceAddress id, T value);
        void Subscribe(ServiceAddress id, NotifyFunc<T> func);
        void Unsubscribe(ServiceAddress id, NotifyFunc<T> func);
        T Value { get; }
    }
}
