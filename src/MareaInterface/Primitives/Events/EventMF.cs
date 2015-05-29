using System;

namespace Marea
{
    public interface Event : Primitive
    {
        void Notify(object value);
        void Subscribe(NotifyFuncGeneric func);
        object Value { get; }
    }
}
