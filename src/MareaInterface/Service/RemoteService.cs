using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Marea
{
	//TODO Tienen que ser publicos?
    public class RemoteProducer : IProxyService
    {
        public TransportAddress ControlAddress { get; private set; }
        public TransportAddress DataAddress { get; set; }
        public UnsubscribePrimitiveDelegate UnsubscribePrimitive{ get; set; }

        public IServiceContainer container;
        public ServiceAddress id;

        public RemoteProducer(TransportAddress control)
        {
            this.ControlAddress = control;
        }

        public bool Start(IServiceContainer container, ServiceAddress serviceAddress)
        {
            return true;
        }

        public bool Stop()
        {
            return true;
        }

        public void Unsubscribe(MareaAddress mad,MareaAddress provider, PrimitiveType primitiveType)
        {
            switch (primitiveType)
            {
                case PrimitiveType.Variable:
                    UnsubscribePrimitive(mad, provider, primitiveType, DataAddress, ControlAddress);
                    break;
                case PrimitiveType.Event:
                    UnsubscribePrimitive(mad, provider, primitiveType, ControlAddress, ControlAddress);
                    break;
                default:
                    throw new NotImplementedException();
            }

        }

    }

    public class RemoteConsumer : Service, IProxyService
    {
        public List<TransportAddress> control;
        public TransportAddress data;
        public ServiceDescription description;
        private SendDataPrimitiveDelegate sendData;

        public RemoteConsumer(TransportAddress data, TransportAddress control, ServiceDescription description, SendDataPrimitiveDelegate sendData)
        {
            this.control = new List<TransportAddress>();
            this.control.Add(control);
            this.data = data;
            this.description = description;
            this.sendData = sendData;
        }

        public void NotifyPrimitive<T>(String name, T value) 
        {
            //Send Data thorugh subscribeProtocol
            MareaAddress mad= new MareaAddress(name);
            PrimitiveType type=description.GetTypeFromPrimitve(mad.GetPrimitive());
            sendData(data,name,value,type);
        }
    }

    public class RemoteBroadcastConsumer : RemoteConsumer
    {
        public RemoteBroadcastConsumer(TransportAddress data, TransportAddress control, ServiceDescription description, SendDataPrimitiveDelegate sendData): base(data,control,description,sendData)
        {
        }

        public void AddControlAddress(TransportAddress ta)
        {
            control.Add(ta);
            Console.WriteLine(control.Count());
        }

        public void Remove(TransportAddress ta)
        {
            control.Remove(ta);
            Console.WriteLine(control.Count());
        }
    }
}
