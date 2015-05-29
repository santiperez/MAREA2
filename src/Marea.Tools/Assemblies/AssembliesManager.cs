using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.IO;
using System.Globalization;

namespace Marea
{
    /// <summary>
    /// Manages the assemblies, namespaces and types.
    /// </summary>
    public class AssembliesManager
    {
        /// <summary>
        /// Cache of assemblies: Requests for assemblies can be served faster in the future. 
        /// </summary>
        public Dictionary<string, Assembly> assembliesCache;

        /// <summary>
        /// Cache of types: Requests for types can be served faster in the future. 
        /// </summary>
        private Dictionary<string, Type> typesCache;

        /// <summary>
        /// AssembliesManager instance. 
        /// </summary>
        private static AssembliesManager instance = null;

        /// <summary>
        /// Default constructor. Loads all Dlls from application base directory 
        /// </summary>
        public AssembliesManager() : this(true)
        {
        }

        /// <summary>
        /// Constructor. Both assembly and types cache is empty.
        /// </summary>
        public AssembliesManager(bool loadDlls)
        {
            assembliesCache = new Dictionary<string, Assembly>();
            typesCache = new Dictionary<string, Type>();

            if (loadDlls)
            {
                this.LoadDlls();
            }
        }

        public List<Assembly> LoadDllsFromDirectory(string dir)
        {
            List<Assembly> assemblies = new List<Assembly>();

            DirectoryInfo i = new DirectoryInfo(dir);
            FileInfo[] files = i.GetFiles("*.dll");
            files=files.Concat(i.GetFiles("*.exe")).ToArray();
            foreach (FileInfo f in files)
            {
                try
                {
                    assemblies.Add(this.LoadAssembly(f.FullName));
                }

                catch (Exception)
                {
                    continue;
                    // Ignore bad or non-CLR DLLs.
                }
                
            }
            return assemblies;
        }

        /// <summary>
        /// Loads all the DLLs in the application directory.
        /// </summary>
        private void LoadDlls()
        {
            LoadDllsFromDirectory(AppDomain.CurrentDomain.SetupInformation.ApplicationBase);
        }

        /// <summary>
        /// Manages AssembliesManager throught singleton pattern.
        /// </summary>
        public static AssembliesManager Instance
        {
            get
            {
                if (instance == null)
                    instance = new AssembliesManager();

                return instance;
            }

        }

        /// <summary>
        /// Gets the full path of the assemblies included as reference to the current project (dll, exe and vshost files).
        /// </summary>
        public IEnumerable<String> GetReferencedAssemblyPaths()
        {
            return GetReferencedAssemblyPaths(Environment.CurrentDirectory);
        }

        public IEnumerable<String> GetReferencedAssemblyPaths(String directory)
        {
            IEnumerable<String> assemblyNames = from f in Directory.EnumerateFiles(directory)
                                                where f.EndsWith(".dll") || f.EndsWith(".exe") && !f.Contains("vshost")
                                                select f;
            return assemblyNames;
        }

        private Assembly LoadAssembly(string assemblyFullName)
        {
            
            //assemblyFullName = assemblyFullName.Remove(assemblyFullName.Length - 4);
            Assembly assembly = Assembly.LoadFrom(assemblyFullName);

              if (assembly != null)
              {
                    string assemblyAlias = assembly.GetAssemblyAlias();
                    if (!assembliesCache.ContainsKey(assemblyAlias))
                    {
                        assembliesCache[assemblyAlias] = assembly;
                        AddTypes(assembly.GetTypes());
                    }
              }
            return assembly;
        }

        private void AddTypes(Type[] types)
        {
            foreach (Type t in types)
            {
                if (!typesCache.ContainsKey(t.FullName))
                    typesCache.Add(t.FullName, t);
            }

        }
 
        /// <summary>
        /// Returns all the namespaces of the assemblies.
        /// </summary>
        public List<String> GetNamespaces()
        {
            List<string> namespaces = new List<string>();
            List<Assembly> assemblies = assembliesCache.Values.ToList();
            if (assemblies != null)
            {
                foreach (Assembly assembly in assemblies)
                {
                    List<String>_namespaces = assembly.GetNamespaces();
                    foreach (string _namespace in _namespaces)
                        if (!namespaces.Contains(_namespace))
                            namespaces.Add(_namespace);
                }
            }

            return namespaces;
        }

        public Dictionary<string, Assembly> GetAllAssemblies()
        {
            return assembliesCache;
        }


        public Dictionary<string,Type> GetAllTypes()
        {
            return typesCache;
        }

        public Type GetTypeFromFullName(string typeFullName)
        {
            Type type = null;

            //This only happens if is a generic
            //There are two possible solutions, Solution 1 is
            //1) Get the assembly and get the type from the full name type
            //2) Get the full name and get the generic parameters, with regex for instance,
            //and then build the generic type with both. http://msdn.microsoft.com/en-us/library/b8ytshk6.aspx

            //Comments
            //Solution 1-> Is slow. To do it faster we should consider to save information of the relation between assemblies and types
            //Maybe reconsider to redesign the cache
            //Solution 2-> low scalability. What happens with multiple encapsulation of generics??
            if (typeFullName.IndexOf("`") > -1)
            {
                foreach (Assembly assembly in assembliesCache.Values)
                {
                    type = assembly.GetType(typeFullName);
                    if (type != null)
                        break;
                }

            }

            //Non-generic type
            else
            {
                if (!typesCache.TryGetValue(typeFullName, out type))
                {//basic type
                    type = Type.GetType(typeFullName);
                    typesCache.Add(typeFullName,type);
                }
            }

            return type;
        }

        /// <summary>
        /// Gets all the serializable types from the given assembly name.
        /// </summary>
        public List<Type> GetSerializableTypesFromAssembly(string assemblyAlias)
        {
            Assembly assembly = assembliesCache[assemblyAlias];
            List<Type> types=null;
            if (assembly != null)
            {
                types=assembly.GetSerializableTypes();
                return types;
            }
            return null;
        }

        /// <summary>
        /// Gets all the types derived from the given type.
        /// </summary>
        public List<Type> GetAllSubClassesFrom(string className)
        {
            List<Type> types = null;
            Type type = typesCache[className];
            if (type != null)
            {
                types = new List<Type>();
                foreach (Type t in typesCache.Values)
                {
                    if (t.IsSubclassOf(type))
                        types.Add(t);
                }
            }
            return types;
        }

        /// <summary>
        /// Gets all the namespaces used by the given type (i.e the namespaces of its fields)
        /// </summary>
        public List<string> GetNamespacesFromTypeMembers(Type t)
        {
            List<string> namespaces = new List<string>();

            FieldInfo[] fieldInfos = t.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);

            foreach (FieldInfo fieldInfo in fieldInfos)
            {
                if (!namespaces.Contains(fieldInfo.DeclaringType.Namespace))
                    namespaces.Add(fieldInfo.DeclaringType.Namespace);
            }

            if (!namespaces.Contains(t.Namespace))
                namespaces.Add(t.Namespace);

            return namespaces;
        }

        public static void FindFields(ICollection<FieldInfo> fields, Type t)
        {
            BindingFlags flags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance;

            foreach (FieldInfo field in t.GetFields(flags))
            {
                // Ignore inherited fields.
                if (field.DeclaringType == t)
                    fields.Add(field);
            }

            Type baseType = t.BaseType;
            if (baseType != null)
                FindFields(fields, baseType);
        }

        public List<String> GetAllIDUs()
        {
            List<String> idus = new List<string>();
            foreach (Assembly a in AppDomain.CurrentDomain.GetAssemblies())
            {
#if __MonoCS__
                if(!a.Location.StartsWith(AppDomain.CurrentDomain.BaseDirectory)) 
                {
                    continue;
                }
#endif
                foreach (Type t in a.GetTypes())
                {
                    if (t.GetCustomAttribute(typeof(ServiceDefinitionAttribute), true) != null)
                        idus.Add(t.FullName);

                }
            }
            //TODO Remove Distinct 
            idus = idus.Distinct().ToList();
            return idus;
        }

        public List<String> GetAllSDUs()
        {
            List<String> sdus = new List<string>();
            foreach (Assembly a in AppDomain.CurrentDomain.GetAssemblies())
            {
#if __MonoCS__
                if(!a.Location.StartsWith(AppDomain.CurrentDomain.BaseDirectory)) 
                {
                    continue;
                }
#endif
                foreach (Type t in a.GetTypes())
                {
                    if (t.GetInterface("Marea.IService") != null &&
                        t.GetInterface("Marea.IProxyService") == null &&
                        //ÑAPA Next are the previous code. The modified one work when the same assemblies are loaded from different folders on MareaGEN
                        //t != typeof(Service) && 
                        t.FullName != typeof(Service).FullName &&
                        t.FullName != typeof(IProxyService).FullName )
                    {
                        sdus.Add(t.FullName);
                    }                        
                }
            }
            //TODO Remove Distinct 
            sdus = sdus.Distinct().ToList();
            return sdus;
        }
    }
}
