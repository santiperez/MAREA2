using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Marea;

namespace PerformanceTests
{
    public class PerformanceTestMonitor : Service, IPerformanceTestMonitor
    {
        [LocateService("*/*/*/*/PerformanceTests.PerformanceTest")]
        private IPerformanceTest test;

        public override bool Start()
        {
            if (test != null)
            {
                test.e_packetSent.Subscribe(id, GetEvent);
                test.v_packetSent.Subscribe(id, GetVariable);
            }
            return true;
        }

        public void GetEvent(String name, Packet<byte> packet)
        {
            e_packetReceived.Notify(id, packet);
        }

        public void GetVariable(String name, Packet<byte> packet)
        {
            v_packetReceived.Notify(id, packet);
        }


        #region PerformanceTestMonitor Members

        public Variable<Packet<byte>> v_packetReceived { get; private set; }

        public Event<Packet<byte>> e_packetReceived { get; private set; }

        #endregion
    }

    /// <summary>
    /// This is the santi-generated PerformanceTestMonitor proxy for the consumers.
    /// </summary>
    public class PerformanceTestMonitorProxy : RemoteProducer, IPerformanceTestMonitor
    {
        public PerformanceTestMonitorProxy(IServiceContainer container, ServiceAddress serviceAddress, TransportAddress control)
            : base(control)
        {
            this.container = container;
            this.id = serviceAddress;
        }

        #region IPerformanceTestMonitor Members

        private Variable<Packet<byte>> _v_packetReceived;
        public Variable<Packet<byte>> v_packetReceived
        {
            get
            {
                if (_v_packetReceived == null)
                {
                    _v_packetReceived = container.CreatePrimitive<Variable<Packet<byte>>>(id, "v_packetReceived");
                }
                return _v_packetReceived;
            }
        }

        private Event<Packet<byte>> _e_packetReceived;
        public Event<Packet<byte>> e_packetReceived
        {
            get
            {
                if (_e_packetReceived == null)
                {
                    _e_packetReceived = container.CreatePrimitive<Event<Packet<byte>>>(id, "e_packetReceived");
                }
                return _e_packetReceived;
            }
        }
        #endregion
    }

    /// <summary>
    /// This is the santi-generated PerformanceTestMonitor query for the consumers.
    /// </summary>
    public class PerformanceTestMonitorQuery : QueryService, IPerformanceTestMonitor
    {
        public PerformanceTestMonitorQuery(IServiceContainer container, ServiceAddress serviceAddress) : base(container, serviceAddress) { }

        public override void AddMatchingService(ServiceAddress serviceAddress, IService service)
        {
            IPerformanceTestMonitor performanceTestmonitor = (IPerformanceTestMonitor)service;

            if (_v_packetReceived != null)
            {
                if (_v_packetReceived.GetTotalSubscriptions() > 0)
                    performanceTestmonitor.v_packetReceived.Subscribe(id, this.FireVariable);
            }

            if (_e_packetReceived != null)
            {
                if (_e_packetReceived.GetTotalSubscriptions() > 0)
                    performanceTestmonitor.e_packetReceived.Subscribe(id, this.FireEvent);
            }

            AddMatchingServiceAddress(serviceAddress);
        }

        public override void RemoveMatchingService(ServiceAddress serviceAddress, IService service)
        {
            IPerformanceTestMonitor performanceTest = (IPerformanceTestMonitor)service;

            if (_v_packetReceived.GetTotalSubscriptions() == 0)
                performanceTest.v_packetReceived.Unsubscribe(id, this.FireVariable);

            if (_e_packetReceived.GetTotalSubscriptions() == 0)
                performanceTest.e_packetReceived.Unsubscribe(id, this.FireEvent);


            int n = 2;
            int[] totalSubscriptions = new int[n];
            totalSubscriptions[0] = _v_packetReceived.GetTotalSubscriptions();
            totalSubscriptions[1] = _e_packetReceived.GetTotalSubscriptions();

            while (--n > 0 && totalSubscriptions[n] == totalSubscriptions[0]) ;

            if (n == 0)
                RemoveMatchingServiceAddress(serviceAddress);
        }

        public void FireEvent(string name, Packet<byte> packet)
        {
            _e_packetReceived.Notify(new ServiceAddress(name), packet);
        }

        public void FireVariable(string name, Packet<byte> packet)
        {
            _v_packetReceived.Notify(new ServiceAddress(name), packet);
        }

        #region IPerformanceTestMonitor Members

        private Variable<Packet<byte>> _v_packetReceived;
        public Variable<Packet<byte>> v_packetReceived
        {
            get
            {
                if (_v_packetReceived == null)
                {
                    _v_packetReceived = container.CreatePrimitive<Variable<Packet<byte>>>(id, "v_packetReceived");
                    ((Primitive)_v_packetReceived).AddSubscriber(AddOrRemoveSubscriber);
                }
                return _v_packetReceived;
            }
        }

        private Event<Packet<byte>> _e_packetReceived;
        public Event<Packet<byte>> e_packetReceived
        {
            get
            {
                if (_e_packetReceived == null)
                {
                    _e_packetReceived = container.CreatePrimitive<Event<Packet<byte>>>(id, "e_packetReceived");
                    ((Primitive)_e_packetReceived).AddSubscriber(AddOrRemoveSubscriber);
                }
                return _e_packetReceived;
            }
        }
        #endregion
    }
}
