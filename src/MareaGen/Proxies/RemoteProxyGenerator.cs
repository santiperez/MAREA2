using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using Antlr4.StringTemplate;
using System.Linq;
using Marea;

namespace MareaGen
{
    class RemoteProxyGenerator
    {
        protected Template template;
        protected DescriptionBuilder builder;
        protected string txt;

        public RemoteProxyGenerator()
        {

            var assembly = Assembly.GetExecutingAssembly();
            var resourceName = "MareaGen.Proxies.RemoteProxy.stg";

            using (Stream stream = assembly.GetManifestResourceStream(resourceName))
            using (StreamReader reader = new StreamReader(stream))
            {
                txt = reader.ReadToEnd();
            }

            // Read template from external file
            //var sr = new StreamReader ("proxy.stg");
            //var txt = sr.ReadToEnd ();
            //sr.Close ();

            template = new Template(txt);
            builder = new DescriptionBuilder();
        }

        public void ClearTemplate()
        {
            template = new Template(txt);
        }

        public String Generate(Type type)
        {
            ClearTemplate();
            ServiceDescription desc = builder.CreateDescription(type);
            template.Add("NAMESPACE", type.Namespace);
            template.Add("CLASS", type.Name);

            string[] iduNames = type.GetInterfaces().Where(t => t != (typeof(IService))).Select(t => t.FullName).ToArray();
            if (iduNames.Length == 0)
                return null;

            template.Add("INTERFACES", iduNames);

            var variables = from v in desc.variables select new { TYPE = "Variable<" + v.Type + ">", NAME = v.Name };
            var events = from e in desc.events select new { TYPE = "Event<" + e.Type + ">", NAME = e.Name };
            var se = variables.Concat(events).ToArray();
            template.Add("PUBSUBS", se);

            var funcs = from f in desc.functions
                        select new
                        {
                            NAME = f.Name,
                            RETURN = (f.ReturnType.Equals("System.Void") ? "void" : f.ReturnType),
                            HAS_RETURN = (f.ReturnType.Equals("System.Void") ? false : true),
                            PARAMS = from p in f.parameters select new { PARAM_NAME = p.Name, PARAM_TYPE = p.Type }
                        };

            template.Add("FUNCS", funcs.ToArray());

            String output = template.Render();
            return output;
        }

        public static List<String>    GenerateProxy()
        {
            List<String> proxySources = new List<String>();
            RemoteProxyGenerator g = new RemoteProxyGenerator();

            //TODO tendriamos que pasar el IDU o el SDU... ???
            //     el IDU deberia valer no? Porque es el mismo para todos los SDU que lo implementan

            List<String> sdus = AssembliesManager.Instance.GetAllSDUs().Distinct().ToList();

            foreach (string sdu in sdus)
            {
                string code = g.Generate(AssembliesManager.Instance.GetTypeFromFullName(sdu));
                if (code == null)
                    continue;
                string fullPath = (MareaGenProgram.dirInfo.FullName + MareaGenProgram.separator + sdu + "Proxy.cs");
                StreamWriter writer = new StreamWriter(fullPath);
                writer.Write(code);
                writer.Close();
                proxySources.Add(fullPath);
                
            }
            return proxySources;
        }

    }
}
