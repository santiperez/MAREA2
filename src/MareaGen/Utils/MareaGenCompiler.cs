using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Xml;
using System.Xml.Linq;
using System.Text;
using System.Threading.Tasks;
using System.CodeDom.Compiler;
using Marea;


namespace MareaGen
{
    /// <summary>
    /// Provides automatic compile functionalities.
    /// </summary>
    public class MareaGenCompiler
    {
        /// <summary>
        /// Compiles an assembly from the given *.cs files.
        /// </summary>
        public static void CompileAssemblyFromSources(string assemblyFilePath, String[] sources, string referencesDirectory)
        {
            Microsoft.CSharp.CSharpCodeProvider provider = new Microsoft.CSharp.CSharpCodeProvider();

            foreach (string sourceName in sources)
            {
                FileInfo sourceFile = new FileInfo(sourceName);

                // Select the code provider based on the input file extension.
                if (sourceFile.Extension.ToUpper(CultureInfo.InvariantCulture) != ".CS")
                {
                    Console.WriteLine("Source file must have a .cs");
                }
            }

            CompilerParameters parameters = new CompilerParameters();

            //Default references .NET 4.0
            parameters.ReferencedAssemblies.Add("System.dll");
            parameters.ReferencedAssemblies.Add("System.Core.dll");
            parameters.ReferencedAssemblies.Add("System.Data.dll");
            parameters.ReferencedAssemblies.Add("System.Data.DataSetExtensions.dll");
            parameters.ReferencedAssemblies.Add("System.Xml.dll");
            parameters.ReferencedAssemblies.Add("System.Xml.Linq.dll");
            parameters.ReferencedAssemblies.Add("Microsoft.CSharp.dll");
            parameters.ReferencedAssemblies.Add("System.Core.dll");
            parameters.ReferencedAssemblies.Add("System.Core.dll");

            //MareaGen included references (File name with extension)
            IEnumerable<string> referencedAssemblyNames = AssembliesManager.Instance.GetReferencedAssemblyPaths(referencesDirectory);
            //referencedAssemblyNames = referencedAssemblyNames.Select(Path.GetFileName);

            string projectName = System.Reflection.Assembly.GetExecutingAssembly().GetName().Name + ".exe";

            foreach (string referencedAssemblyName in referencedAssemblyNames)
            {
                if (referencedAssemblyName != CoderConstants.MAREAGEN_ASSEMBLY_NAME && referencedAssemblyName != projectName)
                    parameters.ReferencedAssemblies.Add(referencedAssemblyName);
            }

            parameters.GenerateInMemory = false;
            parameters.GenerateExecutable = false;
            parameters.OutputAssembly = assemblyFilePath;
			parameters.TreatWarningsAsErrors = false;

            CompilerResults results = provider.CompileAssemblyFromFile(parameters, sources);
            if (results.Errors == null || results.Errors.HasErrors == false)
            {

                Console.WriteLine();
                Console.ForegroundColor = ConsoleColor.Green;
                Console.Write("AssemblyName: ");
                Console.ForegroundColor = ConsoleColor.Gray;
                Console.WriteLine(results.CompiledAssembly.FullName);

                Console.WriteLine();
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("Included Types");

                Console.ForegroundColor = ConsoleColor.Gray;

                foreach (Type t in results.CompiledAssembly.GetTypes())
                    Console.WriteLine(t.Name);
                Console.WriteLine();
            }
            else
            {
                String errors = "";
                int i = 0;
                foreach (CompilerError err in results.Errors)
                {
                    errors += ("Error[" + i + "]: " + err.ErrorText + "\n");
                    i++;
                    if (!err.IsWarning)
                    {
                        Console.WriteLine("["+err.FileName+":"+err.Line+"]: "+err.ErrorText);
                    }
                }

                throw new CompileAssemblyFromFileException(errors, new Exception());
            }
        }

        /// <summary>
        /// Generates and XML file which includes the included types by MAREAGen.
        /// </summary>
        public static void GenerateCompiledInfo(Dictionary<byte, string> dictionary, string path)
        {
            XDocument result = new XDocument(new XElement("MareaTypes",
              dictionary.Select(i => new XElement("Type", new XAttribute("Name", i.Value),
                  new XAttribute("ID", i.Key)))
              ));

            var xml = result.ToString();
            StreamWriter writer = new StreamWriter(path);
            writer.Write(xml.ToString());
            writer.Flush();
            writer.Close();
        }
    }

    public class CompileAssemblyFromFileException : Exception
    {
        public CompileAssemblyFromFileException() { }
        public CompileAssemblyFromFileException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}
