using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using NUnit.Framework;

using Marea;
using Examples;

namespace MareaUnitTests.Services.Local.TwoManTwoTest
{
    [TestFixture]
    class TwoManTwoTestBase : ServiceTestBase
    {
        public static int testLength = 2;
        public static int managersLength = 2;
        public static int containersLength = 1;

        public int managerContainerId = 0;
        public int managerContainerId1 = 0;
        public int testContainerId = 0;
        public int testContainerId1 = 0;

        protected Test test;
        protected Test test1;
        protected TestManager testManager;
        protected TestManager testManager1;

        public TwoManTwoTestBase() : base(containersLength, testLength, managersLength) { }

        public TwoManTwoTestBase(int containersSize): base(containersSize, testLength, managersLength)
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
            test1 = StartTest(testContainerId1, 1);
        }

        [TearDown]
        public virtual void RunAfterAnyTest()
        {
            StopManager(managerContainerId, 0);
            StopManager(managerContainerId1, 1);
            StopTest(testContainerId, 0);
            StopTest(testContainerId1, 1);
        }

        [TestCase, NUnit.Framework.Description("Services.Local/Remote.TwoManTwoTest(SendVariables)")]
        public void Test00SendVariables()
        {
            test.NotifyVariable();
            TestResult VariableManagerWait = testManager.WaitForVariable();
            TestResult VariableManagerWait1 = testManager1.WaitForVariable();

            bool testSent = VariableManagerWait.Result && test.Variable.Value == VariableManagerWait.Value && VariableManagerWait1.Result && test.Variable.Value == VariableManagerWait1.Value;

            test1.NotifyVariable();
            TestResult VariableManagerWait2 = testManager.WaitForVariable();
            TestResult VariableManagerWait3 = testManager1.WaitForVariable();

            bool testSent1 = VariableManagerWait2.Result && test1.Variable.Value == VariableManagerWait2.Value && VariableManagerWait3.Result && test1.Variable.Value == VariableManagerWait3.Value;

            Assert.True(testSent && testSent1);
        }

        [TestCase, NUnit.Framework.Description("Services.Local/remote.TwoManTwoTest(SendEvents)")]
        public void Test01SendEvents()
        {
            test.NotifyEvent();
            TestResult EventManagerWait = testManager.WaitForEvent();
            TestResult EventManagerWait1 = testManager1.WaitForEvent();

            bool testSent = EventManagerWait.Result && test.Event.Value == EventManagerWait.Value && EventManagerWait1.Result && test.Event.Value == EventManagerWait1.Value;

            test1.NotifyEvent();
            TestResult EventManagerWait2 = testManager.WaitForEvent();
            TestResult EventManagerWait3 = testManager1.WaitForEvent();

            bool testSent1 = EventManagerWait2.Result && test1.Event.Value == EventManagerWait2.Value && EventManagerWait3.Result && test1.Event.Value == EventManagerWait3.Value;

            Assert.True(testSent && testSent1);
        }

        [TestCase, NUnit.Framework.Description("Services.Local/Remote.TwoManTwoTest(Reset Test 0 and resend primitives)")]
        public void Test02ResetTest0ResendPrimitives()
        {
            //Reset test
            bool stopped = StopTest(testContainerId, 0);
            test = StartTest(testContainerId, 0);

            //Events
            test.NotifyEvent();
            TestResult EventManagerWait = testManager.WaitForEvent();
            TestResult EventManagerWait1 = testManager1.WaitForEvent();

            test1.NotifyEvent();
            TestResult EventManagerWait2 = testManager.WaitForEvent();
            TestResult EventManagerWait3 = testManager1.WaitForEvent();

            bool testSentEvents = EventManagerWait.Result && test.Event.Value == EventManagerWait.Value && EventManagerWait1.Result && test.Event.Value == EventManagerWait1.Value
                && EventManagerWait2.Result && test1.Event.Value == EventManagerWait2.Value && EventManagerWait3.Result && test1.Event.Value == EventManagerWait3.Value;

            //Variables
            test.NotifyVariable();
            TestResult VariableManagerWait = testManager.WaitForVariable();
            TestResult VariableManagerWait1 = testManager1.WaitForVariable();

            test1.NotifyVariable();
            TestResult VariableManagerWait2 = testManager.WaitForVariable();
            TestResult VariableManagerWait3 = testManager1.WaitForVariable();

            bool testSentVariables = VariableManagerWait.Result && test.Variable.Value == VariableManagerWait.Value && VariableManagerWait1.Result && test.Variable.Value == VariableManagerWait1.Value
                && VariableManagerWait2.Result && test1.Variable.Value == VariableManagerWait2.Value && VariableManagerWait3.Result && test1.Variable.Value == VariableManagerWait3.Value;

            Assert.True(stopped && testSentVariables && testSentEvents);
        }

        [TestCase, NUnit.Framework.Description("Services.Local/remote.TwoManTwoTest(Reset Test 1 and resend primitives)")]
        public void Test03ResetTest1ResendPrimitives()
        {
            //Reset test
            bool stopped = StopTest(testContainerId1, 1);
            test = StartTest(testContainerId1, 1);

            //Events
            test.NotifyEvent();
            TestResult EventManagerWait = testManager.WaitForEvent();
            TestResult EventManagerWait1 = testManager1.WaitForEvent();

            test1.NotifyEvent();
            TestResult EventManagerWait2 = testManager.WaitForEvent();
            TestResult EventManagerWait3 = testManager1.WaitForEvent();

            bool testSentEvents = EventManagerWait.Result && test.Event.Value == EventManagerWait.Value && EventManagerWait1.Result && test.Event.Value == EventManagerWait1.Value
                && EventManagerWait2.Result && test1.Event.Value == EventManagerWait2.Value && EventManagerWait3.Result && test1.Event.Value == EventManagerWait3.Value;

            //Variables
            test.NotifyVariable();
            TestResult VariableManagerWait = testManager.WaitForVariable();
            TestResult VariableManagerWait1 = testManager1.WaitForVariable();

            test1.NotifyVariable();
            TestResult VariableManagerWait2 = testManager.WaitForVariable();
            TestResult VariableManagerWait3 = testManager1.WaitForVariable();

            bool testSentVariables = VariableManagerWait.Result && test.Variable.Value == VariableManagerWait.Value && VariableManagerWait1.Result && test.Variable.Value == VariableManagerWait1.Value
                && VariableManagerWait2.Result && test1.Variable.Value == VariableManagerWait2.Value && VariableManagerWait3.Result && test1.Variable.Value == VariableManagerWait3.Value;

            Assert.True(stopped && testSentVariables && testSentEvents);
        }

        [TestCase, NUnit.Framework.Description("Services.Local/Remote.TwoManTwoTest(Reset Manager 0 and resend primitives)")]
        public void Test04ResetManager0ResendPrimitives()
        {
            //Reset test
            bool stopped = StopManager(managerContainerId1, 1);
            testManager1 = StartManager(managerContainerId1, 1);

            //Events
            test.NotifyEvent();
            TestResult EventManagerWait = testManager.WaitForEvent();
            TestResult EventManagerWait1 = testManager1.WaitForEvent();

            test1.NotifyEvent();
            TestResult EventManagerWait2 = testManager.WaitForEvent();
            TestResult EventManagerWait3 = testManager1.WaitForEvent();

            bool testSentEvents = EventManagerWait.Result && test.Event.Value == EventManagerWait.Value && EventManagerWait1.Result && test.Event.Value == EventManagerWait1.Value
                && EventManagerWait2.Result && test1.Event.Value == EventManagerWait2.Value && EventManagerWait3.Result && test1.Event.Value == EventManagerWait3.Value;

            //Variables
            test.NotifyVariable();
            TestResult VariableManagerWait = testManager.WaitForVariable();
            TestResult VariableManagerWait1 = testManager1.WaitForVariable();

            test1.NotifyVariable();
            TestResult VariableManagerWait2 = testManager.WaitForVariable();
            TestResult VariableManagerWait3 = testManager1.WaitForVariable();

            bool testSentVariables = VariableManagerWait.Result && test.Variable.Value == VariableManagerWait.Value && VariableManagerWait1.Result && test.Variable.Value == VariableManagerWait1.Value
                && VariableManagerWait2.Result && test1.Variable.Value == VariableManagerWait2.Value && VariableManagerWait3.Result && test1.Variable.Value == VariableManagerWait3.Value;

            Assert.True(stopped && testSentVariables && testSentEvents);
        }

        [TestCase, NUnit.Framework.Description("Services.Local/Remote.TwoManTwoTest(Reset Manager 1 and resend primitives)")]
        public void Test05ResetManager1ResendPrimitives()
        {
            //Reset test
            bool stopped = StopManager(managerContainerId1, 1);
            testManager1 = StartManager(managerContainerId1, 1);

            //Events
            test.NotifyEvent();
            TestResult EventManagerWait = testManager.WaitForEvent();
            TestResult EventManagerWait1 = testManager1.WaitForEvent();

            test1.NotifyEvent();
            TestResult EventManagerWait2 = testManager.WaitForEvent();
            TestResult EventManagerWait3 = testManager1.WaitForEvent();

            bool testSentEvents = EventManagerWait.Result && test.Event.Value == EventManagerWait.Value && EventManagerWait1.Result && test.Event.Value == EventManagerWait1.Value
                && EventManagerWait2.Result && test1.Event.Value == EventManagerWait2.Value && EventManagerWait3.Result && test1.Event.Value == EventManagerWait3.Value;

            //Variables
            test.NotifyVariable();
            TestResult VariableManagerWait = testManager.WaitForVariable();
            TestResult VariableManagerWait1 = testManager1.WaitForVariable();

            test1.NotifyVariable();
            TestResult VariableManagerWait2 = testManager.WaitForVariable();
            TestResult VariableManagerWait3 = testManager1.WaitForVariable();

            bool testSentVariables = VariableManagerWait.Result && test.Variable.Value == VariableManagerWait.Value && VariableManagerWait1.Result && test.Variable.Value == VariableManagerWait1.Value
                && VariableManagerWait2.Result && test1.Variable.Value == VariableManagerWait2.Value && VariableManagerWait3.Result && test1.Variable.Value == VariableManagerWait3.Value;

            Assert.True(stopped && testSentVariables && testSentEvents);
        }

        [TestCase, NUnit.Framework.Description("Services.Local/Remote.TwoManTwoTest(Stop Test 0 and resend primitives)")]
        public void Test06StopTest0ResendPrimitives()
        {
            //Stop test
            bool stopped = StopTest(testContainerId, 0);

            //Events
            test1.NotifyEvent();
            TestResult EventManagerWait = testManager.WaitForEvent();
            TestResult EventManagerWait1 = testManager1.WaitForEvent();

            bool testSentEvents = EventManagerWait.Result && test1.Event.Value == EventManagerWait.Value && EventManagerWait1.Result && test1.Event.Value == EventManagerWait1.Value;

            //Variables
            test1.NotifyVariable();
            TestResult VariableManagerWait = testManager.WaitForVariable();
            TestResult VariableManagerWait1 = testManager1.WaitForVariable();

            bool testSentVariables = VariableManagerWait.Result && test1.Variable.Value == VariableManagerWait.Value && VariableManagerWait1.Result && test1.Variable.Value == VariableManagerWait1.Value;

            Assert.True(stopped && testSentVariables && testSentEvents);
        }

        [TestCase, NUnit.Framework.Description("Services.Local/Remote.TwoManTwoTest(Stop Test 1 and resend primitives)")]
        public void Test07StopTest1ResendPrimitives()
        {
            //Stop test
            bool stopped = StopTest(testContainerId1, 1);

            //Events
            test.NotifyEvent();
            TestResult EventManagerWait = testManager.WaitForEvent();
            TestResult EventManagerWait1 = testManager1.WaitForEvent();

            bool testSentEvents = EventManagerWait.Result && test.Event.Value == EventManagerWait.Value && EventManagerWait1.Result && test.Event.Value == EventManagerWait1.Value;

            //Variables
            test.NotifyVariable();
            TestResult VariableManagerWait = testManager.WaitForVariable();
            TestResult VariableManagerWait1 = testManager1.WaitForVariable();

            bool testSentVariables = VariableManagerWait.Result && test.Variable.Value == VariableManagerWait.Value && VariableManagerWait1.Result && test.Variable.Value == VariableManagerWait1.Value;

            Assert.True(stopped && testSentVariables && testSentEvents);
        }


        [TestCase, NUnit.Framework.Description("Services.Local/Remote.TwoManTwoTest(Stop Manager 0 and resend primitives)")]
        public void Test08StopManager0ResendPrimitives()
        {
            //Stop test
            bool stopped = StopManager(managerContainerId, 0);

            //Events
            test.NotifyEvent();
            TestResult EventManagerWait = testManager.WaitForEvent();
            TestResult EventManagerWait1 = testManager1.WaitForEvent();

            test1.NotifyEvent();
            TestResult EventManagerWait2 = testManager.WaitForEvent();
            TestResult EventManagerWait3 = testManager1.WaitForEvent();

            bool testSentEvents = !EventManagerWait2.Result && test1.Event.Value != EventManagerWait2.Value && EventManagerWait3.Result && test1.Event.Value == EventManagerWait3.Value
                && !EventManagerWait.Result && test.Event.Value != EventManagerWait.Value && EventManagerWait1.Result && test.Event.Value == EventManagerWait1.Value;

            //Variables
            test.NotifyVariable();
            TestResult VariableManagerWait = testManager.WaitForVariable();
            TestResult VariableManagerWait1 = testManager1.WaitForVariable();

            test1.NotifyVariable();
            TestResult VariableManagerWait2 = testManager.WaitForVariable();
            TestResult VariableManagerWait3 = testManager1.WaitForVariable();

            bool testSentVariables = !VariableManagerWait2.Result && test1.Variable.Value != VariableManagerWait2.Value && VariableManagerWait3.Result && test1.Variable.Value == VariableManagerWait3.Value
                && !VariableManagerWait.Result && test.Variable.Value != VariableManagerWait.Value && VariableManagerWait1.Result && test.Variable.Value == VariableManagerWait1.Value;

            Assert.True(stopped && testSentVariables && testSentEvents);
        }

        [TestCase, NUnit.Framework.Description("Services.Local.TwoManTwoTest(Stop Manager 1 and resend primitives)")]
        public void Test08StopManager1ResendPrimitives()
        {
            //Stop test
            bool stopped = StopManager(managerContainerId1, 1);

            //Events
            test.NotifyEvent();
            TestResult EventManagerWait = testManager.WaitForEvent();
            TestResult EventManagerWait1 = testManager1.WaitForEvent();

            test1.NotifyEvent();
            TestResult EventManagerWait2 = testManager.WaitForEvent();
            TestResult EventManagerWait3 = testManager1.WaitForEvent();

            bool testSentEvents = EventManagerWait2.Result && test1.Event.Value == EventManagerWait2.Value && !EventManagerWait3.Result && test1.Event.Value != EventManagerWait3.Value
                && EventManagerWait.Result && test.Event.Value == EventManagerWait.Value && !EventManagerWait1.Result && test.Event.Value != EventManagerWait1.Value;

            //Variables
            test.NotifyVariable();
            TestResult VariableManagerWait = testManager.WaitForVariable();
            TestResult VariableManagerWait1 = testManager1.WaitForVariable();

            test1.NotifyVariable();
            TestResult VariableManagerWait2 = testManager.WaitForVariable();
            TestResult VariableManagerWait3 = testManager1.WaitForVariable();

            bool testSentVariables = VariableManagerWait2.Result && test1.Variable.Value == VariableManagerWait2.Value && !VariableManagerWait3.Result && test1.Variable.Value != VariableManagerWait3.Value
                && VariableManagerWait.Result && test.Variable.Value == VariableManagerWait.Value && !VariableManagerWait1.Result && test.Variable.Value != VariableManagerWait1.Value;

            Assert.True(stopped && testSentVariables && testSentEvents);
        }

    }
}
