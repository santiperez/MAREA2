using System;

namespace Marea
{
    /// <summary>
    /// Defines the Marea-visible interface for the RPC protocol.
    /// </summary>
	public interface IRemoteProcedureCallProtocol : IProtocol
	{
		object CallFunction(TransportAddress ta, string function, object[] parameters);

	}
}

