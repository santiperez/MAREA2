using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace MareaGen
{
    /// <summary>
    /// Configures paths and names of the MAREAGen output files.
    /// </summary>
    public class CoderConstants
    {
		public readonly static string MAREAGEN_ASSEMBLY_NAME = "MGenTypes.dll";
        public readonly static string MAREAGEN_ASSEMBLY_RELATIVE_PATH = "build" + Path.DirectorySeparatorChar + MAREAGEN_ASSEMBLY_NAME;
    }

    /// <summary>
    /// Enum used to represent the most common used bytes.
    /// </summary>
    public enum CoderBytes : byte
    {
        //This bytes are provided according to the designed proposed in CoderTables.cs
        NULL = 64,
        GENERIC = 65,
        NOT_NULL = 127,
    }
}
