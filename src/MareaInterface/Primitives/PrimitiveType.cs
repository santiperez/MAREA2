using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Marea
{
    [Serializable]
    /// <summary>
    /// Types of primitives offered by Marea.
    /// </summary>
    public enum PrimitiveType
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
}
