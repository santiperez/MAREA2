using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Examples;
using Marea;

namespace MareaUnitTests.Services
{
    public class ServiceTestBase
    {
        protected ServiceContainer[] containers;
        private Test[] tests;
        private TestManager[] managers;
        protected int timeout = 0;

        public ServiceTestBase(int containersLength, int testsLength, int managersLength)
        {
            containers = new ServiceContainer[containersLength];
            tests = new Test[testsLength];
            managers = new TestManager[managersLength];
        }

        public Test StartTest(int containerIndex, int index)
        {
            lock (this)
            {
                if (tests[index] != null)
                    throw new Exception();

                else
                {
                    MareaAddress mad = containers[containerIndex].StartService("Examples.Test");
                    Test test = (Test)containers[containerIndex].GetService(mad);
                    tests[index] = test;
                    Thread.Sleep(timeout);
                    return test;
                }
            }
        }

        public bool StopTest(int containerIndex, int index)
        {
            lock (this)
            {
                if (tests[index] == null)
                    return false;

                else
                {
                    bool status = containers[containerIndex].serviceManager.StopService(((Test)tests[index]).Id);
                    tests[index] = null;
                    Thread.Sleep(timeout);
                    return status;
                }
            }
        }


        public TestManager StartManager(int containerIndex, int index)
        {
            lock (this)
            {
                if (managers[index] != null)
                    throw new Exception();
                else
                {
                    MareaAddress mad = containers[containerIndex].StartService("Examples.TestManager");
                    TestManager manager = (TestManager)containers[containerIndex].GetService(mad);
                    managers[index] = manager;
                    Thread.Sleep(timeout);
                    return manager;
                }
            }
        }

        public bool StopManager(int containerIndex, int index)
        {
            lock (this)
            {
                if (managers[index] == null)
                    return false;

                else
                {
                    bool status = containers[containerIndex].serviceManager.StopService(((TestManager)managers[index]).Id);
                    managers[index] = null;
                    Thread.Sleep(timeout);
                    return status;
                }
            }
        }

        public void StartContainer(int containerIndex)
        {
            lock (containers)
            {
                if (containers[containerIndex] != null)
                    throw new Exception();
                else
                {
                    ServiceContainer container = new ServiceContainer();
                    //Task t = Task.Factory.StartNew(container.Start);
                    //Task.WaitAll(t);
                    container.Start();
                    containers[containerIndex] = container;
                }
            }
        }

        public bool StopContainer(int containerIndex)
        {
            lock (containers)
            {
                if (containers[containerIndex] == null)
                    return false;

                else
                {
                    containers[containerIndex].Stop();
                    containers[containerIndex] = null;
                    return true;
                }
            }
        }
    }

}
