using Marea;
using Marea.Terminal;
using System;
using System.Threading;

namespace Examples
{
	class CentralServer : Service, ICentralServer
	{
		[LocateService("*/*/*/*/Examples.AircraftController")]
		IAircraftController controllers;

		protected LineEditor editor = new LineEditor (null);
		protected bool running = false;

		public override bool Start ()
		{
			running = true;
			controllers.Response.Subscribe (id, ReceiveNewResults);
            running = true;
            Thread th = new Thread(Run);
            th.Start();
			return true;
		}

		public override bool Stop ()
		{
			running = false;
			editor.InterruptEdit ();
			return true;
		}

		protected void Run() {
			while (running) {
				Console.WriteLine ("Enter the interval to begin a new test.");
				String txt = editor.Edit ("interval> ", "");
				int interval = Int32.Parse (txt);
				BeginTest(interval);
				Console.WriteLine ("Press Enter to end the test.");
				txt = editor.Edit (">", "");
				EndTest();
			}
		}

		public void BeginTest (int interval)
		{
			Console.WriteLine ("STARTING TEST");
			TestStatus.Notify (id, new Status { beginOrEnd = true, intervalBetweenResults = interval });
		}

		public void EndTest ()
		{
			Console.WriteLine ("STOPPING TEST");
			TestStatus.Notify (id, new Status { beginOrEnd = false });
		}

		protected void ReceiveNewResults (String name, Response result)
		{
			Console.WriteLine ("NEW RESULT FROM " + name);
			Console.WriteLine ("VALUE=" + result.Value + " TIMEOUT=" + result.Timeout);
			//Acumulo los resultados en una matriz
		}

		#region ICentralServer implementation

		public Event<Status> TestStatus { get; private set; }

		#endregion
	}

    //public class CentralServerProxy : RemoteProducer, ICentralServer {

    //    protected IRemoteProcedureCallProtocol rpc;

    //    public CentralServerProxy(IServiceContainer container, ServiceAddress serviceAddress, TransportAddress control) : base(control) {
    //        this.container = container;
    //        this.id = serviceAddress;
    //        this.rpc = container.GetProtocol<IRemoteProcedureCallProtocol> ();
    //    }

    //    protected Event<Status> _TestStatus;
    //    public Event<Status> TestStatus {
    //        get {
    //            if( _TestStatus == null) { _TestStatus = container.CreatePrimitive<Event<Status>>(id,"TestStatus"); }
    //            return _TestStatus;
    //        }
    //    }
	
    //    public void BeginTest(int intervalBetweenResults) {
    //        rpc.CallFunction(this.ControlAddress,id+"/BeginTest", new object[] { intervalBetweenResults });
    //    }

    //    public void EndTest() {
    //        rpc.CallFunction(this.ControlAddress,id+"/EndTest", null);
    //    }

    //} // end-class
		
    //class CentralServerQuery : QueryService, ICentralServer {

    //    public CentralServerQuery(IServiceContainer container, ServiceAddress serviceAddress) : base(container, serviceAddress) { }

    //    protected Event<Status> _TestStatus;
    //    public Event<Status> TestStatus {
    //        get {
    //            if( _TestStatus == null) { 
    //                _TestStatus = container.CreatePrimitive<Event<Status>>(id,"TestStatus");
    //                _TestStatus.AddSubscriber (AddOrRemoveSubscriber);
    //            }
    //            return _TestStatus;
    //        }
    //    }
		 
    //    public void FireTestStaus(string name, Status value) {
    //        _TestStatus.Notify(new ServiceAddress(name), value);
    //    }

    //    public void BeginTest(int intervalBetweenResults) {
    //        MareaAddress mad = bindedServices[0];
    //        ICentralServer s = (ICentralServer)container.GetService(mad);
    //        s.BeginTest(intervalBetweenResults);
    //    }

    //    public void EndTest() {
    //        MareaAddress mad = bindedServices[0];
    //        ICentralServer s = (ICentralServer)container.GetService(mad);
    //        s.EndTest();
    //    }

    //    public override void AddMatchingService(ServiceAddress serviceAddress, IService service)
    //    {
    //        ICentralServer s = (ICentralServer)service;

    //        if (_TestStatus != null)
    //        {
    //            if (_TestStatus.GetTotalSubscriptions () > 0)
    //                s.TestStatus.Subscribe (id, this.FireTestStaus);
    //        }

    //        AddMatchingServiceAddress(serviceAddress);
    //    }

    //    public override void RemoveMatchingService(ServiceAddress serviceAddress, IService service)
    //    {
    //        ICentralServer s = (ICentralServer)service;
    //        bool unused = true;

    //        if (_TestStatus.GetTotalSubscriptions () == 0) {
    //            s.TestStatus.Unsubscribe (id, this.FireTestStaus);
    //        } else {
    //            unused = false;
    //        }

    //        if (unused) {
    //            RemoveMatchingServiceAddress (serviceAddress);
    //        }
    //    }

    //} // end-class
	
} // end-namespace



