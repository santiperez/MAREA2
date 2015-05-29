using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;

using Marea;
using Examples;

namespace MareaUnitTests.Services.Local.OneManTwoTest
{
    [TestFixture]
    class OneManTwoTestBase : ServiceTestBase
    {
        public static int testLength = 2;
        public static int managersLength = 1;
        public static int containersLength = 1;

        public int managerContainerId = 0;
        public int testContainerId = 0;
        public int testContainerId1 = 0;

        protected Test test;
        protected Test test1;
        protected TestManager testManager;

        public OneManTwoTestBase() : base(containersLength, testLength, managersLength) { }

        public OneManTwoTestBase(int containersSize)
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
            test = StartTest(testContainerId, 0);
            test1 = StartTest(testContainerId1, 1);
        }

        [TearDown]
        public virtual void RunAfterAnyTest()
        {
            StopManager(managerContainerId, 0);
            StopTest(testContainerId, 0);
            StopTest(testContainerId1, 1);
        }

        [TestCase, NUnit.Framework.Description("Services.Local/Remote.OneManTwoTest(SendVariables)")]
        public void Test00SendVariables()
        {
            test.NotifyVariable();
            TestResult VariableManagerWait = testManager.WaitForVariable();

            bool testSent = VariableManagerWait.Result && test.Variable.Value == VariableManagerWait.Value;

            test1.NotifyVariable();
            TestResult VariableManagerWait1 = testManager.WaitForVariable();

            bool testSent1 = VariableManagerWait1.Result && test1.Variable.Value == VariableManagerWait1.Value;

            Assert.True(testSent && testSent1);
        }

        [TestCase, NUnit.Framework.Description("Services.Local/Remote.OneManTwoTest(SendEvents)")]
        public void Test01SendEvents()
        {
            test.NotifyEvent();
            TestResult EventManagerWait = testManager.WaitForEvent();

            bool testSent = EventManagerWait.Result && test.Event.Value == EventManagerWait.Value;

            test1.NotifyEvent();
            TestResult EventManagerWait1 = testManager.WaitForEvent();

            bool testSent1 = EventManagerWait1.Result && test1.Event.Value == EventManagerWait1.Value;

            Assert.True(testSent && testSent1);
        }

        [TestCase, NUnit.Framework.Description("Services.Local.OneManTwoTest(Reset Manager and resend primitives)")]
        public void Test02ResetManagerResendPrimitives()
        {
            bool stopped = StopManager(managerContainerId,0);
            testManager = StartManager(managerContainerId,0);

            test.NotifyEvent();
            TestResult EventManagerWait = testManager.WaitForEvent();
            bool sendEvent = EventManagerWait.Result && EventManagerWait.Value == test.Event.Value;

            test1.NotifyEvent();
            TestResult EventManagerWait1 = testManager.WaitForEvent();
            bool sendEvent1 = EventManagerWait1.Result && EventManagerWait1.Value == test1.Event.Value;

            test.NotifyVariable();
            TestResult VariableManagerWait = testManager.WaitForVariable();
            bool sendVariable = VariableManagerWait.Result && VariableManagerWait.Value == test.Variable.Value;

            test1.NotifyVariable();
            TestResult VariableManagerWait1 = testManager.WaitForVariable();
            bool sendVariable1 = VariableManagerWait1.Result && VariableManagerWait1.Value == test1.Variable.Value;

            Assert.True(stopped && sendEvent && sendEvent1 && sendVariable && sendVariable1);
        }


        [TestCase, NUnit.Framework.Description("Services.Local.OneManTwoTest(Reset Test 1 and resend primitives)")]
        public void Test03ResetTest1ResendPrimitives()
        {
            bool stopped = StopTest(testContainerId1,1);
            test1 = StartTest(testContainerId1,1);

            test.NotifyEvent();
            TestResult EventManagerWait = testManager.WaitForEvent();
            bool sendEvent = EventManagerWait.Result && EventManagerWait.Value == test.Event.Value;

            test1.NotifyEvent();
            TestResult EventManagerWait1 = testManager.WaitForEvent();
            bool sendEvent1 = EventManagerWait1.Result && EventManagerWait1.Value == test1.Event.Value;

            test.NotifyVariable();
            TestResult VariableManagerWait = testManager.WaitForVariable();
            bool sendVariable = VariableManagerWait.Result && VariableManagerWait.Value == test.Variable.Value;

            test1.NotifyVariable();
            TestResult VariableManagerWait1 = testManager.WaitForVariable();
            bool sendVariable1 = VariableManagerWait1.Result && VariableManagerWait1.Value == test1.Variable.Value;

            Assert.True(stopped && sendEvent && sendEvent1 && sendVariable && sendVariable1);
        }

        [TestCase, NUnit.Framework.Description("Services.Local.OneManTwoTest(Reset Test 0 and resend primitives)")]
        public void Test04ResetTest0ResendPrimitives()
        {
            bool stopped = StopTest(testContainerId, 0);
            test = StartTest(testContainerId, 0);

            test.NotifyEvent();
            TestResult EventManagerWait = testManager.WaitForEvent();
            bool sendEvent = EventManagerWait.Result && EventManagerWait.Value == test.Event.Value;

            test1.NotifyEvent();
            TestResult EventManagerWait1 = testManager.WaitForEvent();
            bool sendEvent1 = EventManagerWait1.Result && EventManagerWait1.Value == test1.Event.Value;


            test.NotifyVariable();
            TestResult VariableManagerWait = testManager.WaitForVariable();
            bool sendVariable = VariableManagerWait.Result && VariableManagerWait.Value == test.Variable.Value;

            test1.NotifyVariable();
            TestResult VariableManagerWait1 = testManager.WaitForVariable();
            bool sendVariable1 = VariableManagerWait1.Result && VariableManagerWait1.Value == test1.Variable.Value;

            Assert.True(stopped && sendEvent && sendEvent1 && sendVariable && sendVariable1);
        }


        [TestCase, NUnit.Framework.Description("Services.Local.OneManTwoTest(Stop one Test 1 and resend primitives)")]
        public void Test05StopTest1ResendPrimitives()
        {
            bool stopped = StopTest(testContainerId1,1);

            test.NotifyEvent();
            TestResult EventManagerWait = testManager.WaitForEvent();

            test.NotifyVariable();
            TestResult VariableManagerWait = testManager.WaitForVariable();

            Assert.True(stopped && EventManagerWait.Result && test.Event.Value == EventManagerWait.Value && VariableManagerWait.Result && test.Variable.Value == VariableManagerWait.Value);

        }

        [TestCase, NUnit.Framework.Description("Services.Local.OneManTwoTest(Stop one Test 0 and resend primitives)")]
        public void Test06StopTest0ResendPrimitives()
        {
            bool stopped = StopTest(testContainerId, 0);

            test1.NotifyEvent();
            TestResult EventManagerWait = testManager.WaitForEvent();

            test1.NotifyVariable();
            TestResult VariableManagerWait = testManager.WaitForVariable();

            Assert.True(stopped && EventManagerWait.Result && test1.Event.Value == EventManagerWait.Value && VariableManagerWait.Result && test1.Variable.Value == VariableManagerWait.Value);

        }


    }
}
