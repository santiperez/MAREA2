using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Contexts;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Marea;

namespace Examples
{
    public class TestManager : Service, ITestManager
    {
        [LocateService("*/*/*/*/Examples.Test")]
        private ITest test;
        private EventWaitHandle waitVariableHandle;
        private EventWaitHandle waitEventHandle;
        private int variableValue;
        private int eventValue;
        private int timeout = 5000;

        public TestManager()
        {
            test = new Test();
            waitVariableHandle =new EventWaitHandle(false,EventResetMode.ManualReset);
            waitEventHandle = new EventWaitHandle(false, EventResetMode.ManualReset);
        }

		//TODO This exposes the "passport". Can we do it other way?
		public ServiceAddress Id {
			get { return id; }
		}

        public override bool Start()
        {
            Console.WriteLine(id);
            if (test != null)
            {
                waitVariableHandle = new EventWaitHandle(false, EventResetMode.ManualReset);
                waitEventHandle = new EventWaitHandle(false, EventResetMode.ManualReset);
                test.Variable.Subscribe(id, GetVariable);
                test.Event.Subscribe(id, GetEvent);
                return true;
            }
            return false;
        }

        public override bool Stop()
        {
            if (test != null)
            {
                if (test.Variable != null)
                    test.Variable.Unsubscribe(id, this.GetVariable);
                if (test.Event!= null)
                    test.Event.Unsubscribe(id, this.GetEvent);
            }
            return true;         
        }


        public void GetEvent(String name, int ev)
        {
            Console.WriteLine();
            Console.WriteLine("[" + this.id + "]");
            Console.WriteLine("\tPrimitive: " + name);
            Console.WriteLine("\tValue: " + ev);
            eventValue = ev;
            waitEventHandle.Set();
        }

        public void GetVariable(String name, int var)
        {

            Console.WriteLine();
            Console.WriteLine("[" + this.id + "]");
            Console.WriteLine("\tPrimitive: " + name);
            Console.WriteLine("\tValue: " + var);
            variableValue = var;
            waitVariableHandle.Set();
        }

        public TestResult WaitForEvent()
        {
            bool signalled;

            signalled = waitEventHandle.WaitOne(timeout, true);
            waitEventHandle.Reset();

            if (signalled)
            {
                Console.WriteLine("Wait event released!!!", Thread.CurrentThread.GetHashCode());
            }
            else
            {
                Console.WriteLine("Wait event timeout!!!", Thread.CurrentThread.GetHashCode());
            }

            return new TestResult(signalled, eventValue);
        }

        public TestResult WaitForVariable()
        {
            bool signalled;

            signalled = waitVariableHandle.WaitOne(timeout,true);
            waitVariableHandle.Reset();

            if (signalled)
            {
                Console.WriteLine("Wait variable released!!!", Thread.CurrentThread.GetHashCode());
            }
            else
            {
                Console.WriteLine("Wait variable timeout!!!", Thread.CurrentThread.GetHashCode());
            }

            return new TestResult(signalled, variableValue);
        }
    }

    public class TestResult
    {
        private int value;
        public int Value
        {
            get { return this.value; }
            set { this.value = value; }
        }
        
        private bool result;
        public bool Result
        {
            get { return result; }
            set { result = value; }
        }

        public TestResult(bool result,int value)
        {
            this.value = value;
            this.result = result;
        }

    }

    ///// <summary>
    ///// This is the santi-generated TestManagerProxy proxy for the producers.
    ///// </summary>
    //class TestManagerProxy : ITestManager
    //{
    //    IServiceContainer container;
    //    ServiceAddress serviceAddress;

    //    TestManagerProxy(IServiceContainer container, ServiceAddress serviceAddress)
    //    {
    //        this.container = container;
    //        this.serviceAddress = serviceAddress;
    //    }

    //    #region IBatteryManager Members


    //    #endregion

    //}
}
