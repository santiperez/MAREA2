using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;

namespace Marea
{
    /// <summary>
    /// Extension methods to manage assemblies and types.
    /// </summary>
    public static class ExtendedAssembly
    {
        /// <summary>
        /// Gets a list of namespaces used by the given assembly.
        /// </summary>
        public static List<string> GetNamespaces(this Assembly assembly)
        {
			//System.Console.WriteLine ("ASSEMBLY :" + assembly);
			//System.Console.WriteLine ("CODEBASE :" + assembly.CodeBase);
			//System.Console.WriteLine ("----------------------------------");

			Type[] types;
			try {
				types = assembly.GetTypes();
			} catch(Exception e) {
				System.Console.WriteLine ("CAGADA!");
				return new List<String>();
			}
			var tmp1 = types.Select (t => t.Namespace);
			var tmp2 = tmp1.Distinct ();
			var tmp3 = tmp2.ToList<String> ();

			return tmp3;
			
			/*
            return assembly.GetTypes()
                                     .Select(t => t.Namespace)
                                     .Distinct().ToList<String>();
			*/
		}

        /// <summary>
        /// Gets a string alias of the given assembly.
        /// </summary>
        public static String GetAssemblyAlias(this Assembly assembly)
        {
            string assemblyName = assembly.GetName().ToString();
            assemblyName = assemblyName.Remove(assemblyName.IndexOf(","), assemblyName.Length - assemblyName.IndexOf(","));
            return assemblyName;
        }

        /// <summary>
        /// Gets a list of types used by the given assembly.
        /// </summary>
        public static List<Type> GetSerializableTypes(this Assembly assembly)
        {
            Type[] types = null;
            List<Type> serializableTypes = new List<Type>();
            types = assembly.GetTypes();
            foreach (Type type in types)
            {
                if (type.IsSerializable)
                {
                    serializableTypes.Add(type);
                }
            }
            return serializableTypes;
        }
    }
}

