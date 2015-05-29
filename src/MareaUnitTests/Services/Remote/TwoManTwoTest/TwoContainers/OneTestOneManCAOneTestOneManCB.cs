using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;

using Marea;
using Examples;
using MareaUnitTests.Services.Local;
using MareaUnitTests.Services.Local.TwoManTwoTest;

namespace MareaUnitTests.Services.Remote.TwoManTwoTest.TwoContainers
{
    class OneTestOneManCAOneTestOneManCB : TwoManTwoTestBase
    {
        public OneTestOneManCAOneTestOneManCB(): base(2)
        {
            testContainerId = 0;
            managerContainerId = 0;

            testContainerId1 = 1;
            managerContainerId1 = 1;

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
            test = StartTest(testContainerId, 0);
            testManager = StartManager(managerContainerId, 0);
            test1 = StartTest(testContainerId1, 1);
            testManager1 = StartManager(managerContainerId1, 1);
        }

        [TearDown]
        public override void RunAfterAnyTest()
        {
            StopTest(testContainerId, 0);
            StopManager(managerContainerId, 0);
            StopTest(testContainerId1, 1);
            StopManager(managerContainerId1, 1);
        }
    }
}
