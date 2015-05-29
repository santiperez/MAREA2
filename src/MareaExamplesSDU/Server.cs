using System;
using Marea;

namespace Examples
{
	public class Server : Service, IServer
	{
		public int DoSomeCalc (int x, int y)
		{
			Console.WriteLine ("CALC: " + x + "+" + y + "=" + (x + y));
			return x + y;
		}
	}
}