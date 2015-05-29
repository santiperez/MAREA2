using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;

using Marea;
using Examples;

namespace MareaUnitTests.Services.BasicTests
{
    [TestFixture]
    class ServiceTest : ServiceTestBase
    {        
        public static int testLength = 1;
        public static int managersLength = 1;
        public static int containersLength = 1;

        Test test;
        TestManager testManager;
        int containerId = 0;

        public ServiceTest() : base(containersLength,testLength, managersLength) { }

        [TestFixtureSetUp]
        public void Init()
        {
            StartContainer(containerId);
        }

        [TestFixtureTearDown]
        public void Dispose()
        {
            StopContainer(containerId);
        }

        [TestCase, NUnit.Framework.Description("Services.ServiceTest(Start Test Service)")]
        public void Test00StartServiceTest()
        {
            test = StartTest(containerId,0);
            Assert.True(true && test.Id!=null);
        }

        [TestCase, NUnit.Framework.Description("Services.ServiceTest(Start TestManager Service)")]
        public void Test01StartServiceTestManager()
        {
            testManager = StartManager(containerId,0);
            Assert.True(true && testManager.Id!=null);
        }

        [TestCase, NUnit.Framework.Description("Services.ServiceTest(StopTest Service)")]
        public void Test03StopServiceTest()
        {
            bool state=StopTest(containerId,0);
            Assert.True(state);
        }

        [TestCase, NUnit.Framework.Description("Services.ServiceTest(Stop TestManager Service)")]
        public void Test04StopServiceTestManager()
        {
            bool state = StopManager(containerId, 0);
            Assert.True(state);
        }
    }
}
