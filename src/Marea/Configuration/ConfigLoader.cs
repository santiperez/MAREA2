using System;
using System.IO;
using System.Text;
using System.Xml.Serialization;

namespace Marea.Configuration
{
	public class ConfigLoader
	{
		public Config Configuration { get; set; }

		protected XmlSerializer xml = new XmlSerializer (typeof(Config));

		public static void Init(ServiceContainer container, String filename) {
			System.Console.WriteLine ("Loading config from " + filename + ".");
			try {
				ConfigLoader loader = new ConfigLoader ();
				loader.Load (filename);
				loader.InitServices (container);
			} catch (Exception e) {
				//TODO Continue or exit?
				System.Console.WriteLine ("Error loading config file! "+e.ToString());
			}
		}

		public static void Init (ServiceContainer container)
		{
			if(File.Exists ("startup.xml")) {
				Init (container, "startup.xml");
			}
		}

		public void Load (String filename)
		{
			TextReader reader = File.OpenText (filename);
			Configuration = (Config)xml.Deserialize (reader);
		}

		public void Save (String filename)
		{
			TextWriter writer = File.CreateText (filename);
			xml.Serialize (writer, Configuration);
		}

		public void InitServices (ServiceContainer container)
		{
			Config c = Configuration;
			if (c.DefaultSubsystem != null) {
				container.defaultSubsystem = c.DefaultSubsystem;
			}

			foreach (Service s in c.Start) {
				//TODO hacer caso del subsistema y de la instancia
				container.StartService (s.Name);
			}
		}
	}
}