using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using Marea;
using System.IO;

namespace PerformanceTests
{
    public class PerformanceTest : Service, IPerformanceTest
    {
        [LocateService("*/*/*/*/PerformanceTests.PerformanceTestMonitor")]
        private IPerformanceTestMonitor monitor;

        private StreamWriter sw;
        private Semaphore sem;
        private Timer tm;
        private Semaphore semTest;
        private int TIMEOUT;
        private int cont;
        private long clock_freq;

        //Configuration menu
        private int type;

        //Test options
        private ushort primitive;
        private int numberBytes;
        private int numberPackets;
        private int freq;


        private void Initialize()
        {
            TIMEOUT = 15000;
            type = 0;
            numberBytes = 0;
            numberPackets = 0;
            freq = 0;
            cont = 0;

            semTest = new Semaphore(0, 1);
            tm = new Timer(new TimerCallback(timer_tick));
        }

        public override bool Start()
        {
            if (monitor != null)
            {
                monitor.e_packetReceived.Subscribe(id, GetPrimitive);
                monitor.v_packetReceived.Subscribe(id, GetPrimitive);
            }

            Initialize();
            Configure();
            return true;
        }

        public void GetPrimitive(String name, Packet<byte> packet)
        {
            tm.Change(TIMEOUT, 0);

            cont++;

            Received(packet);

            if (cont == numberPackets)
                NotifyFinish();
        }

        public override bool Stop()
        {
            // End up the service
            cont = 0;

            return true;
        }


        void Received(Packet<byte> p)
        {
            long ticks = PerformanceTimer.TicksDifference(p.time);
            double ms = 1000.0 * ticks / clock_freq;

            lock (sw)
            {
                sw.WriteLine("Received-" + p.number + "-" + ms.ToString());
            }
            if (sem != null)
            {
                sem.Release();
            }
        }

        void OpenStreamWriter()
        {
            string title = "";
            switch (primitive)
            {
                case 1:
                    title += "-Variables";
                    break;
                case 2:
                    title += "-Events";
                    break;
                default:
                    throw new NotImplementedException();
            }
            title += "-SpecFrequency";
            title += "-Byte";

            string path = DateTime.Now.Hour.ToString() + "." + DateTime.Now.Minute.ToString() + "." + DateTime.Now.Second.ToString() + title + "-" + freq + "Hz" + "-" + +numberBytes + "Bytes" + ".txt";
            DirectoryInfo d = new DirectoryInfo(System.Environment.GetEnvironmentVariable("HOMEDRIVE") + "\\Tests");
            if (!d.Exists)
            {
                d.Create();
            }

            sw = new StreamWriter(d.FullName + "\\" + path);
            sw.WriteLine(numberPackets);

        }

        private void NotifyFinish()
        {
            cont = 0;

            Console.WriteLine("Test successful - Finish");

            if (semTest != null)
            {
                semTest.Release();
            }
        }

        void Configure()
        {
            #region Configuration Menu
            try
            {
                //SELECT PRIMITIVE
                Console.WriteLine("Primitive: ");
                Console.WriteLine("1-Variable");
                Console.WriteLine("2-Events");

                primitive = Convert.ToUInt16(Console.ReadLine());

                if (primitive < 3 && primitive > 0)
                {
                    type = 3;
                    if (type == 3)
                    {
                        Console.WriteLine("Frequency: ");
                        freq = Convert.ToInt32(Console.ReadLine());
                    }

                    Console.WriteLine("Bytes to send: ");
                    numberBytes = Convert.ToInt32(Console.ReadLine());

                    Console.WriteLine("number of packets: ");
                    numberPackets = Convert.ToInt32(Console.ReadLine());

                }
                else
                    throw new Exception();
            }
            catch (Exception)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("\n  Wrong Format  \n ");
                Console.ForegroundColor = ConsoleColor.Gray;
                Configure();
            }
            #endregion

            Thread t = new Thread(new ThreadStart(this.Run));
            t.Start();
        }

        private void Run()
        {
            clock_freq = PerformanceTimer.Clock_freq();
            TestFrequency();
            tm.Dispose();
        }

        void TestFrequency()
        {
            OpenStreamWriter();
            int timeSleep = 1000 / freq;
            Packet<byte> pb = new Packet<byte>();
            int n = numberBytes / Marshal.SizeOf(new byte());

            for (pb.number = 1; pb.number < (numberPackets + 1); pb.number++)
            {
                pb.data = new byte[n];
                pb.time = PerformanceTimer.Ticks();
                RunTest(pb);
                Thread.Sleep(timeSleep);
            }

            sw.WriteLine("ALL PACKETS HAVE BEEN SENT");
            semTest.WaitOne();
        }

        void RunTest(Packet<byte> p)
        {
            switch (primitive)
            {
                //variable
                case 1:
                    v_packetSent.Notify(id, p);
                    break;
                //event
                case 2:
                    //event
                    e_packetSent.Notify(id, p);
                    break;
                default:
                    throw new NotImplementedException();
            }
        }

        public void timer_tick(object o)
        {
            cont = 0;
            //Console.ForegroundColor = ConsoleColor.Red;
            //Console.WriteLine("TimedOut - Finish");
            //Console.ForegroundColor = ConsoleColor.Gray;

            Console.WriteLine("Time Out - Finish");
            if (semTest != null)
            {
                semTest.Release();
            }
        }

        #region PerformanceTest Members

        public Variable<Packet<byte>> v_packetSent { get; private set; }

        public Event<Packet<byte>> e_packetSent { get; private set; }

        #endregion
    }

    /// <summary>
    /// This is the santi-generated PerformanceTest proxy for the consumers.
    /// </summary>
    public class PerformanceTestProxy : RemoteProducer, IPerformanceTest
    {
        public PerformanceTestProxy(IServiceContainer container, ServiceAddress serviceAddress, TransportAddress control)
            : base(control)
        {
            this.container = container;
            this.id = serviceAddress;
        }

        #region IPerformanceTest Members

        private Variable<Packet<byte>> _v_packetSent;
        public Variable<Packet<byte>> v_packetSent
        {
            get
            {
                if (_v_packetSent == null)
                {
                    _v_packetSent = container.CreatePrimitive<Variable<Packet<byte>>>(id, "v_packetSent");
                }
                return _v_packetSent;
            }
        }

        private Event<Packet<byte>> _e_packetSent;
        public Event<Packet<byte>> e_packetSent
        {
            get
            {
                if (_e_packetSent == null)
                {
                    _e_packetSent = container.CreatePrimitive<Event<Packet<byte>>>(id, "e_packetSent");
                }
                return _e_packetSent;
            }
        }
        #endregion
    }

    /// <summary>
    /// This is the santi-generated PerformanceTest query for the consumers.
    /// </summary>
    public class PerformanceTestQuery : QueryService, IPerformanceTest
    {
        public PerformanceTestQuery(IServiceContainer container, ServiceAddress serviceAddress) : base(container, serviceAddress) { }

        public override void AddMatchingService(ServiceAddress serviceAddress, IService service)
        {
            IPerformanceTest performanceTest = (IPerformanceTest)service;

            if (_v_packetSent != null)
            {
                if (_v_packetSent.GetTotalSubscriptions() > 0)
                    performanceTest.v_packetSent.Subscribe(id, this.FireVariable);
            }

            if (_e_packetSent != null)
            {
                if (_e_packetSent.GetTotalSubscriptions() > 0)
                    performanceTest.e_packetSent.Subscribe(id, this.FireEvent);
            }

            AddMatchingServiceAddress(serviceAddress);
        }

        public override void RemoveMatchingService(ServiceAddress serviceAddress, IService service)
        {
            IPerformanceTest performanceTest = (IPerformanceTest)service;

            if (_v_packetSent.GetTotalSubscriptions() == 0)
                performanceTest.v_packetSent.Unsubscribe(id, this.FireVariable);

            if (_e_packetSent.GetTotalSubscriptions() == 0)
                performanceTest.e_packetSent.Unsubscribe(id, this.FireEvent);


            int n = 2;
            int[] totalSubscriptions = new int[n];
            totalSubscriptions[0] = _v_packetSent.GetTotalSubscriptions();
            totalSubscriptions[1] = _e_packetSent.GetTotalSubscriptions();

            while (--n > 0 && totalSubscriptions[n] == totalSubscriptions[0]) ;

            if (n == 0)
                RemoveMatchingServiceAddress(serviceAddress);
        }

        public void FireEvent(string name, Packet<byte> packet)
        {
            _e_packetSent.Notify(new ServiceAddress(name), packet);
        }

        public void FireVariable(string name, Packet<byte> packet)
        {
            _v_packetSent.Notify(new ServiceAddress(name), packet);
        }

        #region IPerformanceTest Members

        private Variable<Packet<byte>> _v_packetSent;
        public Variable<Packet<byte>> v_packetSent
        {
            get
            {
                if (_v_packetSent == null)
                {
                    _v_packetSent = container.CreatePrimitive<Variable<Packet<byte>>>(id, "v_packetSent");
                    ((Primitive)_v_packetSent).AddSubscriber(AddOrRemoveSubscriber);
                }
                return _v_packetSent;
            }
        }

        private Event<Packet<byte>> _e_packetSent;
        public Event<Packet<byte>> e_packetSent
        {
            get
            {
                if (_e_packetSent == null)
                {
                    _e_packetSent = container.CreatePrimitive<Event<Packet<byte>>>(id, "e_packetSent");
                    ((Primitive)_e_packetSent).AddSubscriber(AddOrRemoveSubscriber);
                }
                return _e_packetSent;
            }
        }
        #endregion
    }
}
