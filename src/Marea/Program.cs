using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Marea.Configuration;


[assembly: log4net.Config.XmlConfigurator (Watch = true)]
namespace Marea
{
	class Program
	{
		public static void Main (String[] args)
		{
			bool startGUI = false;
			bool startConsole = false;
			bool startMonitor = true;
			bool startFromConfig = true;
			String configFile = null;
            
			//TODO NodeMonitor mola mas que NodeManager?

			for (int i=0; i<args.Length; i++) {
				String arg = args[i];
				switch (arg) {
				case "--no-monitor":
					startMonitor = false;
                    break;
				case "--config":
					configFile = args [++i];
					break;
				case "--no-config":
					startFromConfig = false;
					break;
				case "--console":
					startConsole = true;
					break;
				#if !__MonoCS__
				case "--gui":
					startGUI = true;
					break;
				#endif
				case "--help":
					System.Console.WriteLine ("Usage: marea [--no-monitor] [--no-config] [--console] [--gui] | --help");
					return;
				default:
					System.Console.WriteLine ("Incorrect parameter: " + args);
					System.Console.WriteLine ("Usage: marea [--no-monitor] [--no-config | --config <filename.xml>] [--console] [--gui] | --help");
					return;
				}
			}

			ServiceContainer container = new ServiceContainer ();
			container.Start ();

			if (startMonitor) {
				container.StartService ("Marea.NodeManager");
			}
			if (startConsole) {
				container.StartService ("Marea.Console");
			}
			if (startGUI) {
				container.StartService ("Marea.GUI");
			}
			if (startFromConfig) {
				if (configFile == null) {
					ConfigLoader.Init (container);
				} else {
					ConfigLoader.Init (container, configFile);
				}
			}

		}
	}
}
