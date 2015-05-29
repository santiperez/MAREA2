using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;

using Marea;
using Examples;
using System.Threading;

namespace MareaUnitTests.Services.Local.OneManOneTest
{
    [TestFixture]
    class OneManOneTestBase : ServiceTestBase
    {
        public static int testLength = 1;
        public static int managersLength = 1;
        public static int containersLength = 1;

        public int managerContainerId = 0;
        public int testContainerId = 0;

        protected Test test;
        protected TestManager testManager;

        public OneManOneTestBase() : base(containersLength,testLength, managersLength) { }

        public OneManOneTestBase(int containersSize) : base(containersSize, testLength, managersLength) 
        {
            containersLength = containersSize;
        }

        [TestFixtureSetUp]
        public virtual void Init()
        {
            StartContainer(0);
            
        }

        [TestFixtureTearDown]
        public virtual void Dispose()
        {
            StopContainer(0);
        }


        [SetUp]
        public virtual void RunBeforeAnyTest()
        {
            testManager=StartManager(managerContainerId,0);
            test=StartTest(testContainerId,0);
        }

        [TearDown]
        public virtual void RunAfterAnyTest()
        {
            StopManager(managerContainerId, 0);
            StopTest(testContainerId,0);
        }

        [TestCase, NUnit.Framework.Description("Services.Local/Remote.OneManOneTest(SendVariable)")]
        public void Test00SendVariable()
        {
            test.NotifyVariable();
            TestResult VariableManagerWait = testManager.WaitForVariable();

            Assert.True(VariableManagerWait.Result && test.Variable.Value == VariableManagerWait.Value);
        }

        [TestCase, NUnit.Framework.Description("Services.Local/Remote.OneManOneTest(SendEvent)")]
        public void Test01SendEvent()
        {
            test.NotifyEvent();
            TestResult EventManagerWait = testManager.WaitForEvent();

            Assert.True(EventManagerWait.Result && test.Event.Value == EventManagerWait.Value);
        }


        [TestCase, NUnit.Framework.Description("Services.Local/Remote.OneManOneTest(Reset Manager and resend primitives)")]
        public void Test02ResetManagerResendPrimitives()
        {
            bool stopped = StopManager(managerContainerId,0);
            testManager = StartManager(managerContainerId,0);

            test.NotifyEvent();
            TestResult EventManagerWait = testManager.WaitForEvent();

            test.NotifyVariable();
            TestResult VariableManagerWait = testManager.WaitForVariable();

            Assert.True(stopped && EventManagerWait.Result && test.Event.Value == EventManagerWait.Value && VariableManagerWait.Result && test.Variable.Value == VariableManagerWait.Value);

        }

        [TestCase, NUnit.Framework.Description("Services.Local/Remote.OneManOneTest(Reset Test and resend primitives)")]
        public void Test03ResetTestResendPrimitives()
        {
            bool stopped = StopTest(testContainerId,0);
            test = StartTest(testContainerId,0);

            test.NotifyVariable();
            TestResult VariableManagerWait = testManager.WaitForVariable();

            test.NotifyEvent();
            TestResult EventManagerWait = testManager.WaitForEvent();

            Assert.True(stopped && EventManagerWait.Result && test.Event.Value == EventManagerWait.Value && VariableManagerWait.Result && test.Variable.Value == VariableManagerWait.Value);
        }
    }
}
