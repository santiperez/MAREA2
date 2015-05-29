using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Marea;

namespace MareaUnitTests
{
    [Serializable]
    /// <summary>
    /// Types of primitives offered by Marea.
    /// </summary>
    public enum Marea1Primitive
    {
        /// <summary> Non-reliable primitive. </summary>
        Variable,
        /// <summary> Reliable primitive. </summary>
        Event,
        /// <summary> Reliable function. </summary>
        Invocation,
        /// <summary> Reliable file transmission. </summary>
        File
    };

    /// <summary>
    /// Marea message used to transfer event and variables values, non-optimized version.
    /// </summary>
    [Serializable]
    public class DataMarea1 : Marea1Message
    {
        public DataMarea1() { }

        public DataMarea1(string name, object data, Marea1Primitive primitive)
        {
            this.name = name;
            this.data = data;
            this.primitive = primitive;
        }
        public string name;
        public object data;
        public Marea1Primitive primitive;
    }
}
