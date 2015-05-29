using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Marea;

namespace Examples
{
    /// <summary>
    /// This is the user implementation of the Test service.
    /// </summary>
    public class Test : Service, ITest
    {
        private int variableValue;
        private int eventValue;

		//TODO This exposes the "passport". Can we do it other way?
		public ServiceAddress Id {
			get { return id; }
		}

        public override bool Start()
        {
            Random rand = new Random();
            eventValue = rand.Next(Int32.MaxValue/4)*3;
            variableValue = rand.Next(Int32.MaxValue / 4) * 3;
            return true;
        }

        public override bool Stop()
        {
            return true;
        }

        public void NotifyVariable()
        {
            variableValue = variableValue + 1;
            Variable.Notify(id, variableValue);
            Console.WriteLine("Sending Variable with value:" + Variable.Value);
        }

        public void NotifyEvent()
        {
            eventValue = eventValue + 1;
            Event.Notify(id, eventValue);
            Console.WriteLine("Sending Event with value:" + Event.Value);
        }

        public Variable<int> Variable { get; private set; }

        public Event<int> Event { get; private set; }
    }

    ///// <summary>
    ///// This is the santi-generated Test proxy for the consumers.
    ///// </summary>
    //class TestProxy : RemoteProducer, ITest
    //{
    //    public TestProxy(IServiceContainer container, ServiceAddress serviceAddress, TransportAddress control)
    //        : base(control)
    //    {
    //        this.container = container;
    //        this.id = serviceAddress;
    //    }


    //    private Variable<int> _Variable;
    //    public Variable<int> Variable
    //    {
    //        get
    //        {
    //            if (_Variable == null)
    //            {
    //                _Variable = container.CreatePrimitive<Variable<int>>(id, "Variable");
    //            }
    //            return _Variable;
    //        }
    //    }

    //    private Event<int> _Event;
    //    public Event<int> Event
    //    {
    //        get
    //        {
    //            if (_Event == null)
    //            {
    //                _Event = container.CreatePrimitive<Event<int>>(id, "Event");
    //            }
    //            return _Event;
    //        }
    //    }
    //}


    /// <summary>
    /// This is the santi-generated Test query for the consumers.
    /// </summary>
    //class TestQuery : QueryService, ITest
    //{
    //    public TestQuery(IServiceContainer container, ServiceAddress serviceAddress) : base(container, serviceAddress) { }

    //    public override void AddMatchingService(ServiceAddress serviceAddress, IService service)
    //    {
    //        ITest test = (ITest)service;

    //        if (_Event != null)
    //        {
    //            if (_Event.GetTotalSubscriptions() > 0)
    //                test.Event.Subscribe(id, this.FireEvent);
    //        }

    //        if (_Variable != null)
    //        {
    //            if (_Variable.GetTotalSubscriptions() > 0)
    //                test.Variable.Subscribe(id, this.FireVariable);
    //        }

    //        AddMatchingServiceAddress(serviceAddress);
    //    }

    //    public override void RemoveMatchingService(ServiceAddress serviceAddress, IService service)
    //    {
    //        ITest test = (ITest)service;

    //        if (_Event.GetTotalSubscriptions() == 0)
    //            test.Event.Unsubscribe(id, this.FireEvent);

    //        if (_Variable.GetTotalSubscriptions() == 0)
    //            test.Variable.Unsubscribe(id, this.FireVariable);

    //        int n = 2;
    //        int[] totalSubscriptions = new int[n];
    //        totalSubscriptions[0] = _Event.GetTotalSubscriptions();
    //        totalSubscriptions[1] = _Variable.GetTotalSubscriptions();

    //        while (--n > 0 && totalSubscriptions[n] == totalSubscriptions[0]) ;

    //        if (n == 0)
    //            RemoveMatchingServiceAddress(serviceAddress);
    //    }

    //    public void FireEvent(string name, int ev)
    //    {
    //        _Event.Notify(new ServiceAddress(name), ev);
    //    }

    //    public void FireVariable(string name, int var)
    //    {
    //        _Variable.Notify(new ServiceAddress(name), var);
    //    }

    //    private Variable<int> _Variable;
    //    public Variable<int> Variable
    //    {
    //        get
    //        {
    //            if (_Variable == null)
    //            {
    //                _Variable = container.CreatePrimitive<Variable<int>>(id, "Variable");
    //                ((Primitive)_Variable).AddSubscriber(AddOrRemoveSubscriber);
    //            }
    //            return _Variable;
    //        }
    //    }

    //    private Event<int> _Event;
    //    public Event<int> Event
    //    {
    //        get
    //        {
    //            if (_Event == null)
    //            {
    //                _Event = container.CreatePrimitive<Event<int>>(id, "Event");
    //                ((Primitive)_Event).AddSubscriber(AddOrRemoveSubscriber);
    //            }
    //            return _Event;
    //        }
    //    }

    //}
}
