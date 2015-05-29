using System;
using System.Diagnostics;
using System.Threading;
using Marea;
using Marea.Terminal;
using Examples;
using System.Linq;

namespace Examples
{
	public class AircraftController : Service, IAircraftController
	{
		private class GUI
		{
			protected AircraftController controller;
			protected LineEditor editor;

			public GUI (AircraftController c)
			{
				controller = c;
				//TODO Check the security of id. It should be protected or something...
				controller.Beep.Subscribe (c.id, Beep);
                editor = new LineEditor(null);
			}

			void Beep (String name, None none)
			{
				//TODO Timeout...
				String txt = editor.Edit ("Stress:", "");

				int value = Int32.Parse (txt);
				controller.SendResponse (value);
			}
		}

		[LocateService("/*/*/*/Examples.CentralServer")]
		ICentralServer centralServer;
		protected bool running = false;
		protected int intervalBetweenResults = -1;
		protected Stopwatch stopwatch;

		public override bool Start ()
		{
			centralServer.TestStatus.Subscribe (id, this.StatusChange);
			stopwatch = new Stopwatch ();

			GUI gui = new GUI (this);
			//Application.Run(gui);

			return true;
		}

		void StatusChange (String mad, Status status)
		{
			if (status.beginOrEnd == true) {
                System.Console.WriteLine("Starting test...");
				running = true;
				intervalBetweenResults = status.intervalBetweenResults;
				Thread th = new Thread (this.Run);
				th.Start ();
			} else {
                System.Console.WriteLine("Stoping test...");
				running = false;
			}
		}

		protected void Run ()
		{
			while (running) {
				Thread.Sleep (intervalBetweenResults);
				stopwatch.Start ();
				if (running) 
					this.Beep.Notify (id, None.Instance);
			}
		}

		public void SendResponse (int value)
		{
			stopwatch.Stop ();	
			Response.Notify (id, new Response { Value = value, Timeout = stopwatch.ElapsedMilliseconds });
		}

		public Event<None> Beep { get; private set; }

		public Event<Response> Response { get; private set; }
	}
}
namespace Marea
{
	/// <summary>
	/// This is the santi-generated Battery proxy for the consumers.
	/// </summary>
    //class AircraftControllerProxy : RemoteProducer, IAircraftController
    //{
    //    protected IRemoteProcedureCallProtocol rpc;

    //    public AircraftControllerProxy (IServiceContainer container, ServiceAddress serviceAddress, TransportAddress control)
    //        : base(control)
    //    {
    //        this.container = container;
    //        this.id = serviceAddress;
    //        this.rpc = container.GetProtocol<IRemoteProcedureCallProtocol> ();
    //    }

    //    #region IAircraftController implementation

    //    public void SendResponse (int value)
    //    {
    //        rpc.CallFunction (this.ControlAddress, id + "/SendResponse", new object[] { value });
    //    }

    //    protected Event<None> _Beep;

    //    public Event<None> Beep {
    //        get {
    //            if (_Beep == null) {
    //                _Beep = container.CreatePrimitive<Event<None>> (id, "Beep");
    //            }
    //            return _Beep;
    //        }
    //    }

    //    protected Event<Response> _Response;

    //    public Event<Response> Response {
    //        get {
    //            if (_Response == null) {
    //                _Response = container.CreatePrimitive<Event<Response>> (id, "Response");
    //            }
    //            return _Response;
    //        }
    //    }

    //    #endregion

    //}

    ///// <summary>
    ///// This is the santi-generated AircraftController query for the consumers.
    ///// </summary>
    //class AircraftControllerQuery : QueryService, IAircraftController
    //{
    //    public AircraftControllerQuery (IServiceContainer container, ServiceAddress serviceAddress) : base(container, serviceAddress)
    //    {
    //    }

    //    public override void AddMatchingService (ServiceAddress serviceAddress, IService service)
    //    {
    //        IAircraftController s = (IAircraftController)service;

    //        if (_Beep != null) {
    //            if (_Beep.GetTotalSubscriptions () > 0)
    //                s.Beep.Subscribe (id, this.FireBeep);
    //        }

    //        if (_Response != null) {
    //            if (_Response.GetTotalSubscriptions () > 0)
    //                s.Response.Subscribe (id, this.FireResponse);
    //        }

    //        AddMatchingServiceAddress (serviceAddress);
    //    }

    //    public override void RemoveMatchingService (ServiceAddress serviceAddress, IService service)
    //    {
    //        IAircraftController s = (IAircraftController)service;
    //        bool unused = true;

    //        if (_Beep.GetTotalSubscriptions () == 0) {
    //            s.Beep.Unsubscribe (id, this.FireBeep);
    //        } else {
    //            unused = false;
    //        }

    //        if (_Response.GetTotalSubscriptions () == 0) {
    //            s.Response.Unsubscribe (id, this.FireResponse);
    //        } else {
    //            unused = false;
    //        }

    //        if (unused) {
    //            RemoveMatchingServiceAddress (serviceAddress);
    //        }
    //    }

    //    #region IAircraftController implementation

    //    public void SendResponse (int value)
    //    {
    //        MareaAddress mad = bindedServices [0];
    //        IAircraftController s = container.GetService<IAircraftController>(mad.ToString());
    //        s.SendResponse (value);
    //    }

    //    protected Event<None> _Beep;

    //    public Event<None> Beep {
    //        get {
    //            if (_Beep == null) {
    //                _Beep = container.CreatePrimitive<Event<None>> (id, "Beep");
    //                _Beep.AddSubscriber (AddOrRemoveSubscriber);
    //            }
    //            return _Beep;
    //        }
    //    }

    //    public void FireBeep (string name, None none)
    //    {
    //        _Beep.Notify (new ServiceAddress (name), none);
    //    }

    //    protected Event<Response> _Response;

    //    public Event<Response> Response {
    //        get {
    //            if (_Response == null) {
    //                _Response = container.CreatePrimitive<Event<Response>> (id, "Response");
    //                _Response.AddSubscriber (AddOrRemoveSubscriber);
    //            }
    //            return _Response;
    //        }
    //    }

    //    public void FireResponse (string name, Response response)
    //    {
    //        _Response.Notify (new ServiceAddress (name), response);
    //    }

    //    #endregion

    //}
}