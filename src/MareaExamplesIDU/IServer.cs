using System;
using Marea;

namespace Examples
{
	[ServiceDefinition("This service represents a server executing a remote complex calculation.")]
	public interface IServer
	{
		int DoSomeCalc(int x, int y);

	}
}

