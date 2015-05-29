using Marea;
using System;

namespace Examples
{
	public class Client : Service, IClient
	{
        ////TODO Porque no funciona sin el primer *
        [LocateService("*/*/*/*/Examples.Server")]
        IServer server;

        public override bool Start()
        {
            ExecuteCalculation();
            return true;
        }

		public void ExecuteCalculation ()
		{            
			int result = server.DoSomeCalc (1, 2);
			Console.WriteLine ("CLIENT RESULT=" + result);
		}
	}
}

