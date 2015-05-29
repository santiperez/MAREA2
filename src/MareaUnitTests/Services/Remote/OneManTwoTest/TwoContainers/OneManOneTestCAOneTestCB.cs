using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;

using Marea;
using Examples;
using MareaUnitTests.Services.Local;
using MareaUnitTests.Services.Local.OneManTwoTest;

namespace MareaUnitTests.Services.Remote.OneManTwoTest.TwoContainers
{
    class OneManOneTestCAOneTestCB : OneManTwoTestBase
    {
        public OneManOneTestCAOneTestCB(): base(2)
        {
            managerContainerId = 0;
            testContainerId = 0;

            testContainerId1 = 1;

            timeout = 5000;
        }

        [TestFixtureSetUp]
        public override void Init()
        {
            StartContainer(0);
            StartContainer(1);
        }

        [TestFixtureTearDown]
        public override void Dispose()
        {
            StopContainer(0);
            StopContainer(1);
        }

        [SetUp]
        public override void RunBeforeAnyTest()
        {
            testManager = StartManager(managerContainerId, 0);
            test = StartTest(testContainerId, 0);
            test1 = StartTest(testContainerId1, 1);
        }

        [TearDown]
        public override void RunAfterAnyTest()
        {
            StopManager(managerContainerId, 0);
            StopTest(testContainerId, 0);
            StopTest(testContainerId1, 1);
        }
    }
}
