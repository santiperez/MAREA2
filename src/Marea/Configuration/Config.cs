using System;
using System.Xml.Serialization;

namespace Marea.Configuration
{
	[XmlRoot()]  
	public class Config
	{  
		[XmlAttribute]  
		public String DefaultSubsystem { get; set; }

		public Service[] Start { get ; set; }
	}

	public class Service
	{  
		[XmlAttribute]  
		public String Name { get; set; }

		[XmlAttribute]  
		public String Id { get; set; }

		[XmlAttribute]  
		public String Subsystem { get; set; }
	}

}
