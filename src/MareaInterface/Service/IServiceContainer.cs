using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Marea
{
    public interface IServiceContainer
    {
		// Start/Stop services
		MareaAddress StartService (string serviceType);
		MareaAddress StartService (MareaAddress mareaAddress);
		bool StopService (MareaAddress serviceAddress);	

		// Get Services
		T GetService<T>(string mad);
		T GetService<T> (MareaAddress mad);
		IService GetService(string mad);
		IService GetService(MareaAddress mad);

		//TODO Estas funciones tienen que ser publicas?
		T GetPrimitiveFromService<T>(IService service, String primitiveName) where T : Primitive;
        T CreatePrimitive<T>(ServiceAddress serviceAddress, String primitiveName) where T : Primitive;
        Primitive GetPrimitiveFieldFromService(IService service, String primitiveName);
        Primitive GetPrimitive(ServiceAddress primitiveAddress) ;
        Dictionary<MareaAddress, IService> GetServicesFromQuery(MareaAddress queryServiceAddress);
        
		// Get Protocol
		T GetProtocol<T> () where T : IProtocol;

		// Direct Network Access
		void SendMessage(TransportAddress transportAddress,Message message);
	}
}
