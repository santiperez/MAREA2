using System.Diagnostics;
using System.Reflection;
using Args;
using Thorn;

namespace Marea
{
	[ThornExport]
	public class Commands
	{
		private Marea.Console console;
        private INodeManager node;

		public Commands(Marea.Console console, INodeManager node) {
			this.console = console;
            this.node = node;
		}

        [System.ComponentModel.Description("Adds two int")]
        public void Add(AddArgs arg)
        {
            var s = node.Add(arg.a,arg.b);
            System.Console.WriteLine("Return " + s);
        }

		[System.ComponentModel.Description("Starts a new service")]
		public void Start(StartServiceArgs arg) {
            var s = node.StartService(arg.serviceName);
            System.Console.WriteLine("Started service " + s);
		}

		[System.ComponentModel.Description("Stops a running service")]
		public void Stop(StopServiceArgs arg) {
			System.Console.WriteLine ("STOPPING " + arg.identifier);
			//TODO
			System.Console.WriteLine ("*TODO*");
		}

		[System.ComponentModel.Description("Set the default subsystem")]
		public void Set_Default_Subsystem(Set_Default_SubsystemArgs arg) {
			System.Console.WriteLine ("SET DEFAULT SUBSYS TO " + arg.identifier);
			//TODO
			System.Console.WriteLine ("*TODO*");
		}

		[System.ComponentModel.Description("Show the available services in the container (i.e the ones with SDU available)")]
		public void Show_Available_Services() {
			System.Console.WriteLine ("Available Services:");
			System.Console.WriteLine ("-------------------");
			foreach (string s in node.GetAvailableServices())
			{
				System.Console.WriteLine(" > " + s);
			}
		}

		[System.ComponentModel.Description("Show the running services in the container")]
		public void Show_Running_Services() {
			System.Console.WriteLine ("Running Services:");
			System.Console.WriteLine ("-----------------");
			foreach (MareaAddress s in node.GetRunningServices())
			{
				System.Console.WriteLine(" > " + s);
			}
		}

		[System.ComponentModel.Description("Show all the known services (both local to the container and remote)")]
		public void Show_Known_Services() {
			System.Console.WriteLine ("Known Services:");
			System.Console.WriteLine ("---------------");
			foreach (MareaAddress s in node.GetKnownServices())
			{
				System.Console.WriteLine(" > " + s);
			}
		}

		[System.ComponentModel.Description("Exits the console")]
		public void Exit() {
			//TODO Don't call directly Stop!!! Call the container to Stop the Service!!!
			console.Stop ();	
		}

		[System.ComponentModel.Description("Shutdown this Marea container")]
		public void Shutdown() {
			//TODO It does not shutdown :??
			node.Shutdown ();
		}

        [System.ComponentModel.Description("Show MAREA info")]
        public void Info()
        {
            FileVersionInfo fVersionInfo =FileVersionInfo.GetVersionInfo(Assembly.GetEntryAssembly().Location);

            System.Console.WriteLine("");
            System.Console.WriteLine("Product Name: " + fVersionInfo.ProductName);
            System.Console.WriteLine("Description: "+ fVersionInfo.Comments);
            System.Console.WriteLine("Company Name: " + fVersionInfo.CompanyName);
            System.Console.WriteLine("Product Version: " + fVersionInfo.ProductVersion);
            System.Console.WriteLine("");
        }

	}

    public class AddArgs
    {
        [ArgsMemberSwitch(0)]
        [System.ComponentModel.Description("First integer")]
        public int a;
        [ArgsMemberSwitch(1)]
        [System.ComponentModel.Description("Second integer")]
        public int b;
    }

	public class StartServiceArgs 
	{
		[ArgsMemberSwitch(0)]
		[System.ComponentModel.Description("Name (i.e Type) of the service")]
		public string serviceName;

		[System.ComponentModel.Description("Identifier of the service to use")]
		public string identifier;

		[System.ComponentModel.Description("Subsystem where to start the service")]
		public string subsystem;
	}

	public class StopServiceArgs {
		[ArgsMemberSwitch(0)]
		[System.ComponentModel.Description("Identifier of the service to stop")]
		public string identifier;
	}

	public class Set_Default_SubsystemArgs {
		[ArgsMemberSwitch(0)]
		[System.ComponentModel.Description("Identifier of the default subsystem (i.e EC-UPC)")]
		public string identifier;
	}

}

