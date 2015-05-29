using Marea;
using System;

namespace Examples
{
	[ServiceDefinition("This service represents a client calling a remote server.")]
	public interface IClient
	{
		void ExecuteCalculation ();
	}
}