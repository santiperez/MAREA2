using System;
using System.Collections.Generic;
using System.Text;

namespace Marea
{
    [Serializable]
    public struct Packet<T>
    {
        public int id;
        public long time;
        public long number;
        public T[] data;
    }
}
