using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;

namespace MareaGen
{
    public class MareaGenConfigLoader
    {
        private string configFileName;

        public string ConfigFileName
        {
            get { return configFileName; }
            set { configFileName = value; }
        }

        protected XDocument doc;

        public string[] nonSerializableTypes;

        public MareaGenConfigLoader(string file)
        {
            // Create the XmlDocument.
            doc = XDocument.Load(file);
            IEnumerable<string> ie = doc.Root.Descendants("Non-Serializable-Assemblies").Descendants("Assembly").Select(x=>x.Attribute("Name").Value);
            nonSerializableTypes = ie.ToArray();
            //doc.LoadXml(file);

            //doc.ro
        }

    }
}
