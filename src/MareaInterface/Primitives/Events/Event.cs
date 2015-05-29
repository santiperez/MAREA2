using System;

namespace Marea
{
    public interface Event<T> : Primitive
    {
        void Notify(ServiceAddress id, T value);
        void Subscribe(ServiceAddress id, NotifyFunc<T> func);
        void Unsubscribe(ServiceAddress id, NotifyFunc<T> func);
        T Value { get; }
    }
}
