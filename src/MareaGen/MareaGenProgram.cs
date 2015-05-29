using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using System.Text;
using System.IO;
using System.Reflection;
using Marea;

namespace MareaGen
{
    /// <summary>
    /// MAREAGen class program.
    /// </summary>
    public class MareaGenProgram
    {
        /// <summary>
        /// boolean used to create and empty *.dll. Solves circular dependencies.
        /// </summary>
        bool createEmpty = false;

        public static char separator = System.IO.Path.DirectorySeparatorChar;

        /// <summary>
        /// bool used to specify if inlining should be used.
        /// </summary>
#if (NET45)
        bool inlineMethods = true;
#else
        bool inlineMethods = false;
#endif

        String directory = null;

        static MareaGenConfigLoader config;

        List<String> namespaces;
        /// <summary>
        /// Examines the given assemblies and searches for serializable types. Generates 
        /// classes automatically with methods to serialize and deserialize MAREA entities 
        /// marked as serializable. This eliminates the need for developers to implement 
        /// serializing and deserializing code and guarantees run-time type safety.
        /// </summary>
        /// 
        public static DirectoryInfo dirInfo;



        public static void Main(String[] args)
        {
            config = new MareaGenConfigLoader("mareagen-conf.xml");
            MareaGenProgram p = new MareaGenProgram();
            p.directory = args[0];
            p.Init();
            p.Run();

        }

        public void Init()
        { 
            //delete files inside /bin/Debug/build
            string dir= Path.Combine(directory,"build");
            dirInfo=Directory.CreateDirectory(dir);
           
            foreach(FileInfo f in dirInfo.GetFiles())
            {
                f.Delete();
            }

            //Delete files MGenTypes.dll & MGenTypes.xml
            dir = Path.Combine(directory,CoderConstants.MAREAGEN_ASSEMBLY_NAME.Replace("dll", "xml"));
            if (File.Exists(dir))
                File.Delete(dir);
            dir = Path.Combine(directory,CoderConstants.MAREAGEN_ASSEMBLY_NAME);
            if (File.Exists(dir))
                File.Delete(dir);

            //This has to be done later in order to avoid the load of MGenTypes.dll in case if exists
            namespaces = AssembliesManager.Instance.GetNamespaces();
        }

        public List<string> GenerateProxies()
        {
            List<string> remoteProxies = RemoteProxyGenerator.GenerateProxy();
            
            remoteProxies.AddRange(QueryProxyGenerator.GenerateProxy());
  

            return remoteProxies;
        }

        public List<Type> GetSerializableTypesFromDirectoryAssemblies(string dir)
        {
            //List<Type> types = new List<Type>();   
            //DirectoryInfo d = new DirectoryInfo(dir);
            //string assemblyName;

            //foreach (var f in d.EnumerateFiles("*.dll"))
            //{
            //    assemblyName = Path.GetFileNameWithoutExtension(f.Name);
            //    if (!config.nonSerializableTypes.Contains(assemblyName))
            //    {
            //        types.AddRange(AssembliesManager.Instance.GetSerializableTypesFromAssembly(assemblyName));
            //    }

            //}

            //return types;


            List<Type> types = new List<Type>();
            DirectoryInfo d = new DirectoryInfo(dir);
            string assemblyName;

            List<Assembly> assemblies = AssembliesManager.Instance.LoadDllsFromDirectory(dir);

            foreach (Assembly a in assemblies)
            {
                assemblyName = Path.GetFileNameWithoutExtension(a.Location);
                if (!config.nonSerializableTypes.Contains(assemblyName))
                    types.AddRange(AssembliesManager.Instance.GetSerializableTypesFromAssembly(assemblyName));
            }
            return types;
        }

        public void Run()
        {

            Console.ForegroundColor = ConsoleColor.Blue;
            Console.WriteLine("############################### MAREAGEN 1.0 ###############################");
            Console.ForegroundColor = ConsoleColor.Gray;

            //Paths
            
            string pathFile = dirInfo.FullName;

            if (!createEmpty)
            {
                //Get all the types marked as Serializable from assembly names from the specified directory
                //List<Type> mareaInterfaceEntities = AssembliesManager.Instance.GetSerializableTypesFromAssembly("MareaInterface");
                //List<Type> mareaEntities = AssembliesManager.Instance.GetSerializableTypesFromAssembly("Marea");
                //List<Type> mareaTestEntities = AssembliesManager.Instance.GetSerializableTypesFromAssembly("MareaTestsEntities");
                //List<Type> mareaExamplesIDU = AssembliesManager.Instance.GetSerializableTypesFromAssembly("MareaExamplesIDU");

                //IEnumerable<Type> query = mareaInterfaceEntities.Concat(mareaEntities);
                //query = query.Concat(mareaTestEntities);
                //query = query.Concat(mareaExamplesIDU);

                //AssembliesManager.Instance.LoadDllsFromDirectory(directory);
                List<Type> query = GetSerializableTypesFromDirectoryAssemblies(directory);
                
                List<Type> mareaAbstract = new List<Type>();

                //Generate non abstract entities
                foreach (Type t in query)
                {
                    if (t.FullName != "Marea.FileDescription") //This type is not implemented
                    {
                        if (t.IsAbstract || Assembly.GetAssembly(t).GetTypes().Where(myType => myType.IsSubclassOf(t)).ToArray().Length > 0)
                            mareaAbstract.Add(t);
                        else
                            createClass(pathFile, "Marea", CoderTypes.typesCache[t.FullName], t.IsSubclassOf(typeof(Message)));

                    }
                }

                //Generate abstract entities. It's necessary to do it before its implementation to prove an id
                foreach (Type t in mareaAbstract)
                    createClass(pathFile, "Marea", CoderTypes.typesCache[t.FullName], t.IsSubclassOf(typeof(Message)));
            }

            string[] coderPath = Directory.GetFiles(dirInfo.FullName, "*.cs");
            //string[] buildPath = Directory.GetFiles(Environment.CurrentDirectory.Replace(separator + "bin" + separator + "Debug", separator + "Coder"), "*.cs");

            //string[] paths=coderPath.Concat(buildPath).ToArray();
            coderPath = coderPath.Concat(GenerateProxies().ToArray()).ToArray();

            //Compile assembly from MAREAGEN types
            string outputDLLFile = Path.Combine(directory, CoderConstants.MAREAGEN_ASSEMBLY_RELATIVE_PATH);
            MareaGenCompiler.CompileAssemblyFromSources(outputDLLFile, coderPath, directory);

            //Create output XML file with MAREAGEN types
            string outputXMLFile = Path.Combine(directory,CoderConstants.MAREAGEN_ASSEMBLY_RELATIVE_PATH.Replace("dll", "xml"));
            MareaGenCompiler.GenerateCompiledInfo(CoderTables.GetInstance().M2Types, outputXMLFile);
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine("MAREAGEN info created [" + outputXMLFile + "]");
            Console.ForegroundColor = ConsoleColor.Gray;

            //Copy and overwrite *.dll and *.xml file in Libs folder
            string destinationFilePath = Path.Combine(directory, CoderConstants.MAREAGEN_ASSEMBLY_NAME.Replace("dll", "xml"));
            if(destinationFilePath!=outputXMLFile)
                System.IO.File.Copy(outputXMLFile,destinationFilePath, true);
            destinationFilePath = Path.Combine(directory, CoderConstants.MAREAGEN_ASSEMBLY_NAME);
            if(destinationFilePath!=outputDLLFile)
                System.IO.File.Copy(outputDLLFile,destinationFilePath, true);

            Console.WriteLine();
            Console.WriteLine("Press any key to finish ...");
            //Console.ReadKey();
        }

        /// <summary>
        /// Checks if the given namespaces exist in the current domain/cache.
        /// </summary>
        public  void CheckNameSpaces(List<string> namespacesList)
        {
            foreach (string _namespace in namespaces )
            {
                if (!namespaces.Contains(_namespace))
                    throw new NotFoundNamespaceException();
            }
        }

        /// <summary>
        /// Creates a class (.cs file) to serialize and deserialize a specific MAREA type.
        /// </summary>
        bool createClass(string pathFile, string _namespace, Type type, bool isSubClassOfMessage)
        {
            bool isCreated = false;

            //DO NOT INCLUDE GENERICS AND DELEGATES (GENERICS ARE SUPPORTTED)
            //TODO TREAT EXCEPTIONS
            if (!type.IsEnum && !type.IsGenericType && type.BaseType != typeof(System.MulticastDelegate) && !type.IsSubclassOf(typeof(System.Exception)))
            {
                pathFile += separator + "MG_" + type.Name + ".cs";
                StreamWriter writer = new StreamWriter(pathFile);

                List<String> _namespaces = new List<string>();

                //Include default nameSpaces
                _namespaces.Add("System");
                _namespaces.Add("System.Collections");
                _namespaces.Add("System.Collections.Generic");
                _namespaces.Add("System.Linq");
                _namespaces.Add("System.Text");
                _namespaces.Add("System.Net");
                if (inlineMethods)
                    _namespaces.Add("System.Runtime.CompilerServices");

                //Include MareaGen nameSpaces
                List<string> mareaGenNamespaces = Assembly.GetExecutingAssembly().GetNamespaces();
                foreach (string mareGenNamespace in mareaGenNamespaces)
                {
                    if (mareGenNamespace != null && !_namespaces.Contains(mareGenNamespace))
                        _namespaces.Add(mareGenNamespace);
                }

                //Include Type nameSpaces
                if (!_namespaces.Contains(_namespace))
                    _namespaces.Add(_namespace);

                //Include nameSpaces from member Types
                List<string> memberNamespaces = AssembliesManager.Instance.GetNamespacesFromTypeMembers(type);
                foreach (string nameSpace in memberNamespaces)
                {
                    if (!_namespaces.Contains(nameSpace))
                        _namespaces.Add(nameSpace);
                }

                //Check all the NameSpaces
                CheckNameSpaces(_namespaces);

                ClassGenerator.AddClassHeader(_namespaces, _namespace, AccessModifier._public, false, "MG_" + type.Name, ref writer);

                if (!isSubClassOfMessage)
                {
                    CoderTables.GetInstance().M2Types.Add(CoderTables.GetInstance().Indexer, type.FullName);
                    ClassGenerator.AddClassConstructor("MG_" + type.Name, type.FullName, CoderTables.GetInstance().Indexer++, ref writer);
                }
                else
                {
                    CoderTables.GetInstance().M2Types.Add(CoderTables.GetInstance().MareaMessageIndexer, type.FullName);
                    ClassGenerator.AddClassConstructor("MG_" + type.Name, type.FullName, CoderTables.GetInstance().MareaMessageIndexer++, ref writer);
                }

                ClassGenerator.AddClassFingerprint(type, ref writer);

                //Check if its abstract or has subClasses
                if (type.IsAbstract || Assembly.GetAssembly(type).GetTypes().Where(myType => myType.IsSubclassOf(type)).ToArray().Length > 0)
                    ClassGenerator.AddEncodeAbstractOrInheritedPropertyMethod(type, ref writer, inlineMethods);
                else
                    ClassGenerator.AddEncodeMethod(type, ref writer, inlineMethods);

                //Check if its abstract or has subClasses
                if (type.IsAbstract || Assembly.GetAssembly(type).GetTypes().Where(myType => myType.IsSubclassOf(type)).ToArray().Length > 0)
                    ClassGenerator.AddDecodeAbstractOrInheritedPropertyMethod(type, ref writer, inlineMethods);
                else
                    ClassGenerator.AddDecodeMethod(type, ref writer, inlineMethods);

                ClassGenerator.AddClassBottom(true, ref writer);

                writer.Close();
                isCreated = true;
            }
            return isCreated;
        }
    }

    public class NotFoundNamespaceException : Exception
    {
        public NotFoundNamespaceException() { }
        public NotFoundNamespaceException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}