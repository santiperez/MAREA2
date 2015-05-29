using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;

using Marea;
using Examples;

namespace MareaUnitTests.Services.Local.TwoManOneTest
{
    [TestFixture]
    class TwoManOneTestBase : ServiceTestBase
    {
        public static int testLength = 1;
        public static int managersLength = 2;
        public static int containersLength = 1;

        public int managerContainerId = 0;
        public int managerContainerId1 = 0;
        public int testContainerId = 0;

        protected Test test;
        protected TestManager testManager;
        protected TestManager testManager1;

        public TwoManOneTestBase() : base(containersLength, testLength, managersLength) { }

        public TwoManOneTestBase(int containersSize)
            : base(containersSize, testLength, managersLength)
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
            testManager = StartManager(managerContainerId, 0);
            testManager1 = StartManager(managerContainerId1, 1);
            test = StartTest(testContainerId, 0);
        }

        [TearDown]
        public virtual void RunAfterAnyTest()
        {
            StopManager(managerContainerId, 0);
            StopManager(managerContainerId1, 1);
            StopTest(testContainerId, 0);
        }

        [TestCase, NUnit.Framework.Description("Services.Local/Remote.TwoManOneTest(SendVariable)")]
        public void Test00SendVariable()
        {
            test.NotifyVariable();
            TestResult VariableManagerWait = testManager.WaitForVariable();
            TestResult VariableManagerWait1 = testManager1.WaitForVariable();

            Assert.True(VariableManagerWait.Result && test.Variable.Value == VariableManagerWait.Value && VariableManagerWait1.Result && test.Variable.Value == VariableManagerWait1.Value);
        }

        [TestCase, NUnit.Framework.Description("Services.Local/Remote.TwoManOneTest(SendEvent)")]
        public void Test01SendEvent()
        {

            test.NotifyEvent();
            TestResult EventManagerWait = testManager.WaitForEvent();
            TestResult EventManagerWait1 = testManager1.WaitForEvent();

            Assert.True(EventManagerWait.Result && test.Event.Value == EventManagerWait.Value && EventManagerWait1.Result && test.Event.Value == EventManagerWait1.Value);
        }

        [TestCase, NUnit.Framework.Description("Services.Local/Remote.TwoManOneTest(Reset Manager 0 and resend primitives)")]
        public void Test02ResetManager0ResendPrimitives()
        {
            bool stopped = StopManager(managerContainerId, 0);
            testManager = StartManager(managerContainerId, 0);

            test.NotifyEvent();
            TestResult EventManagerWait = testManager.WaitForEvent();
            TestResult EventManagerWait1 = testManager1.WaitForEvent();

            test.NotifyVariable();
            TestResult VariableManagerWait = testManager.WaitForVariable();
            TestResult VariableManagerWait1 = testManager1.WaitForVariable();

            Assert.True(stopped && EventManagerWait.Result && test.Event.Value == EventManagerWait.Value && VariableManagerWait.Result && test.Variable.Value == VariableManagerWait.Value
                && EventManagerWait1.Result && test.Event.Value == EventManagerWait1.Value && VariableManagerWait1.Result && test.Variable.Value == VariableManagerWait1.Value);

        }

        [TestCase, NUnit.Framework.Description("Services.Local/Remote.TwoManOneTest(Reset Manager 1 and resend primitives)")]
        public void Test03ResetManager1ResendPrimitives()
        {
            bool stopped = StopManager(managerContainerId1, 1);
            testManager1 = StartManager(managerContainerId1, 1);

            test.NotifyEvent();
            TestResult EventManagerWait = testManager.WaitForEvent();
            TestResult EventManagerWait1 = testManager1.WaitForEvent();

            test.NotifyVariable();
            TestResult VariableManagerWait = testManager.WaitForVariable();
            TestResult VariableManagerWait1 = testManager1.WaitForVariable();

            Assert.True(stopped && EventManagerWait.Result && test.Event.Value == EventManagerWait.Value && VariableManagerWait.Result && test.Variable.Value == VariableManagerWait.Value
                && EventManagerWait1.Result && test.Event.Value == EventManagerWait1.Value && VariableManagerWait1.Result && test.Variable.Value == VariableManagerWait1.Value);

        }

        [TestCase, NUnit.Framework.Description("Services.Local/Remote.TwoManOneTest(Reset Test and resend primitives)")]
        public void Test04ResetTestResendPrimitives()
        {
            bool stopped = StopTest(testContainerId, 0);
            test = StartTest(testContainerId, 0);

            test.NotifyEvent();
            TestResult EventManagerWait = testManager.WaitForEvent();
            TestResult EventManagerWait1 = testManager1.WaitForEvent();

            test.NotifyVariable();
            TestResult VariableManagerWait = testManager.WaitForVariable();
            TestResult VariableManagerWait1 = testManager1.WaitForVariable();

            Assert.True(stopped && EventManagerWait.Result && test.Event.Value == EventManagerWait.Value && VariableManagerWait.Result && test.Variable.Value == VariableManagerWait.Value
                && EventManagerWait1.Result && test.Event.Value == EventManagerWait1.Value && VariableManagerWait1.Result && test.Variable.Value == VariableManagerWait1.Value);

        }

        [TestCase, NUnit.Framework.Description("Services.Local/Remote.TwoManOneTest(Stop  Manager 0 and resend primitives)")]
        public void Test05StopManager0ResendPrimitives()
        {
            bool stopped = StopManager(managerContainerId, 0);

            test.NotifyEvent();
            TestResult EventManagerWait = testManager.WaitForEvent();
            TestResult EventManagerWait1 = testManager1.WaitForEvent();

            test.NotifyVariable();
            TestResult VariableManagerWait = testManager.WaitForVariable();
            TestResult VariableManagerWait1 = testManager1.WaitForVariable();

            Assert.True(stopped && !EventManagerWait.Result && test.Event.Value != EventManagerWait.Value && !VariableManagerWait.Result && test.Variable.Value != VariableManagerWait.Value
                && EventManagerWait1.Result && EventManagerWait1.Value == test.Event.Value && VariableManagerWait1.Result && VariableManagerWait1.Value == test.Variable.Value);

        }

        [TestCase, NUnit.Framework.Description("Services.Local/Remote.TwoManOneTest(Stop  Manager 1 and resend primitives)")]
        public void Test06StopManager1ResendPrimitives()
        {
            bool stopped = StopManager(managerContainerId1, 1);

            test.NotifyEvent();
            TestResult EventManagerWait = testManager.WaitForEvent();
            TestResult EventManagerWait1 = testManager1.WaitForEvent();

            test.NotifyVariable();
            TestResult VariableManagerWait = testManager.WaitForVariable();
            TestResult VariableManagerWait1 = testManager1.WaitForVariable();

            Assert.True(stopped && EventManagerWait.Result && test.Event.Value == EventManagerWait.Value && VariableManagerWait.Result && test.Variable.Value == VariableManagerWait.Value
                && !EventManagerWait1.Result && EventManagerWait1.Value != test.Event.Value && !VariableManagerWait1.Result && VariableManagerWait1.Value != test.Variable.Value);

        }
    }
}
