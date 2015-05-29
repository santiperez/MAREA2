using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using System.Collections.Generic;
using Marea.Configuration;
using NUnit.Framework;

namespace MareaUnitTests.Configuration
{
	[TestFixture]
	public class ConfigurationTest
	{
		public ConfigurationTest ()
		{
		}

		[SetUp]
		public void RunBeforeAnyTest ()
		{
		}

		[TestCase, NUnit.Framework.Description("ConfigurationTest(Test01)")]
		public void Test01 ()
		{
			XmlSerializer xml = new XmlSerializer (typeof(Config));

			String input = 
				"<Config>" +
				"\t<Start>" +
				"\t\t<Service Id=\"EC-UPC/127.0.0.1:1234/0/Examples.Battery\" />" +
				"\t\t<Service Id=\"EC-UPC/127.0.0.1:1234/0/Examples.BatteryManager\" />" +
				"\t</Start>" +
				"</Config>";

			Config c2 = (Config)xml.Deserialize (new MemoryStream (Encoding.UTF8.GetBytes (input)));
			Assert.AreEqual (c2.Start.Length, 2);
			Assert.AreEqual (c2.Start [0].Id, "EC-UPC/127.0.0.1:1234/0/Examples.Battery");
			Assert.AreEqual (c2.Start [1].Id, "EC-UPC/127.0.0.1:1234/0/Examples.BatteryManager");
		}

		[TestCase, NUnit.Framework.Description("ConfigurationTest(Test01)")]
		public void Test02 ()
		{
			Config c = new Config ();
			c.DefaultSubsystem = "EC-UPC";
			Service s1 = new Service { Id = "A1", Name = "A.A" };
			Service s2 = new Service { Id = "A2", Name = "A.A", Subsystem = "EC-EPL" };
			c.Start = new Service[] { s1, s2 };
		
			XmlSerializer xml = new XmlSerializer (typeof(Config));

			MemoryStream outMem = new MemoryStream ();
			xml.Serialize (outMem, c);

			MemoryStream inMem = new MemoryStream (outMem.ToArray ());
			Config c2 = (Config)xml.Deserialize (inMem);
			Assert.AreEqual (c2.DefaultSubsystem, "EC-UPC");
			Assert.AreEqual (c2.Start.Length, 2);

			Assert.AreEqual (c2.Start [0].Id, s1.Id);
			Assert.AreEqual (c2.Start [0].Name, s1.Name);
			Assert.IsNull(c2.Start [0].Subsystem);

			Assert.AreEqual (c2.Start [1].Id, s2.Id);
			Assert.AreEqual (c2.Start [1].Name, s2.Name);
			Assert.AreEqual (c2.Start [1].Subsystem, s2.Subsystem);
		}
	}
}