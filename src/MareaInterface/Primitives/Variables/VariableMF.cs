using System;

namespace Marea
{
    public interface Variable : Primitive
    {
        void Notify(object value);
        void Subscribe(NotifyFuncGeneric func);
        object Value { get; }
    }
}
