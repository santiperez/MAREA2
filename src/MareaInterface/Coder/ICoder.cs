namespace Marea
{
    public interface ICoder
    {
        void Start(IServiceContainer container, ITransport transport);
        void Stop();
        void Send(TransportAddress t, Message m);
        void DataReceived(byte[] data, int offset, int length, TransportAddress ta);
    }
}