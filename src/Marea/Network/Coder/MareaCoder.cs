using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Reflection;
using MareaGen;

namespace Marea
{
    /// <summary>
    /// Coder. To work properly MGenTypes.dll should be referenced to MAREA project.
    /// MGen tool must be excecuted in case of new types are included.
    /// </summary>
    public class MareaCoder
    {
        /// <summary>
        /// CoderTables static instance used to serialize/deserialize basic types.
        /// </summary>
        public static CoderTables tables;

        /// <summary>
        /// Constructor.
        /// </summary>
        static MareaCoder()
        {
            tables = CoderTables.GetInstance();
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
            try
            {
                // force static constructors in types specified by InitializeOnLoad
                foreach (InitializeOnLoadAttribute attr in args.LoadedAssembly.GetCustomAttributes(typeof(InitializeOnLoadAttribute), false))
                    System.Runtime.CompilerServices.RuntimeHelpers.RunClassConstructor(attr.Type.TypeHandle);
            }
            catch (Exception)
            {

            }

        }

        /// <summary>
        /// Serialize objects.
        /// <summary>
        public static void Send(NetworkMessage networkMessage)
        {
            int offset = 0;
            if (networkMessage.Object == null)
                CoderTypes.FromByte((byte)CoderBytes.NULL, networkMessage.Buffer, ref offset);
            else
            {
                EncodeData f = (EncodeData)tables.EncodeTable[networkMessage.Object.GetType().FullName];
                if (f == null)
                {
                    f = (EncodeData)tables.EncodeTable[typeof(object).FullName];
                }

                CoderTypes.FromByte(f.id, networkMessage.Buffer, ref offset);
                f.func(networkMessage.Object, networkMessage.Buffer, ref offset);
            }
            networkMessage.Offset = offset;
        }

        /// <summary>
        /// Deserialize objects.
        /// <summary>
        public static void Receive(NetworkMessage networkMessage)
        {
            int offset = 0;
            byte id = (byte)CoderTypes.ToByte(networkMessage.Buffer, ref offset);

            if (id == (byte)CoderBytes.NULL)
                networkMessage.Object = null;
            else
            {
                DecodeFunction f = (DecodeFunction)tables.DecodeTable[(int)id];
                if (f == null)
                {
                    f = (DecodeFunction)tables.DecodeTable[0];
                }
                networkMessage.Id = id;
                networkMessage.Object = f(networkMessage.Buffer, ref offset);
            }
        }
    }
}
