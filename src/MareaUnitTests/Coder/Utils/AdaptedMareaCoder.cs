using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Reflection;
using MareaGen;
using Marea;
using System.Runtime.CompilerServices;

namespace MareaUnitTests
{
    /// <summary>
    /// This class is an adaptation of MAREA coder. Instead of using a NetworkMessage 
    /// it uses a byte[] in order to serialize and deserialize messages.
    /// </summary>
    public class AdaptedMareaCoder
    {
        protected const int MAX_SERIALIZABLE_SIZE = 150000;
        protected static byte[] workBuffer;

        /// <summary>
        /// Constructor.
        /// </summary>
        static AdaptedMareaCoder()
        {
            workBuffer = new byte[MAX_SERIALIZABLE_SIZE];

            Assembly assembly = AssembliesManager.Instance.assembliesCache[CoderConstants.MAREAGEN_ASSEMBLY_NAME.Replace(".dll", "")];
 
            if (assembly != null)
                AssemblyInitializer(null, new AssemblyLoadEventArgs(assembly));
            else
                throw new DllNotFoundException();

        }

        /// <summary>
        /// Calls the static constructors of the MareaGen generated classes.
        /// </summary>
        private static void AssemblyInitializer(object sender, AssemblyLoadEventArgs args)
        {
            // force static constructors in types specified by InitializeOnLoad
            foreach (InitializeOnLoadAttribute attr in args.LoadedAssembly.GetCustomAttributes(typeof(InitializeOnLoadAttribute), false))
                System.Runtime.CompilerServices.RuntimeHelpers.RunClassConstructor(attr.Type.TypeHandle);
        }

        /// <summary>
        /// Serialize objects.
        /// <summary>
        public static byte[] Send(object o)
        {
            int offset = 0;
            if (o == null)
                CoderTypes.FromByte((byte)CoderBytes.NULL, workBuffer, ref offset);
            else
            {
                EncodeData f = (EncodeData)CoderTables.GetInstance().EncodeTable[o.GetType().FullName];
                if (f == null)
                {
                    f = (EncodeData)CoderTables.GetInstance().EncodeTable[typeof(object).FullName];
                }

                CoderTypes.FromByte(f.id, workBuffer, ref offset);
                f.func(o, workBuffer, ref offset);
            }

            byte[] buffer = new byte[offset];
            Array.Copy(workBuffer, buffer, offset);
            return buffer;
        }

        /// <summary>
        /// Deserialize objects.
        /// <summary>
        public static object Receive(byte[] buffer)
        {           
            int offset = 0;
            byte id = (byte)CoderTypes.ToByte(buffer, ref offset);

            if (id == (byte)CoderBytes.NULL)
                return null;
            else
            {   
                DecodeFunction f = (DecodeFunction)CoderTables.GetInstance().DecodeTable[(int)id];
                if (f == null)
                {
                    f = (DecodeFunction)CoderTables.GetInstance().DecodeTable[0];
                }

                return f(buffer, ref offset);
            }
        }
    }
}
