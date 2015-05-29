using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;

using Marea;
using Examples;
using MareaUnitTests.Services.Local;
using MareaUnitTests.Services.Local.TwoManOneTest;

namespace MareaUnitTests.Services.Remote.TwoManOneTest.ThreeContainers
{
    class OneTestCAOneManCBOneManCC:TwoManOneTestBase
    {
        public OneTestCAOneManCBOneManCC(): base(3)
        {
            testContainerId = 0;
            
            managerContainerId = 1;
            
            managerContainerId1 = 2;

            timeout = 5000;
        }

        [TestFixtureSetUp]
        public override void Init()
        {
            StartContainer(0);
            StartContainer(1);
            StartContainer(2);
        }

        [TestFixtureTearDown]
        public override void Dispose()
        {
            StopContainer(0);
            StopContainer(1);
            StopContainer(2);
        }

        [SetUp]
        public override void RunBeforeAnyTest()
        {
            test = StartTest(testContainerId, 0);
            testManager = StartManager(managerContainerId, 0);
            testManager1 = StartManager(managerContainerId1, 1);
        }

        [TearDown]
        public override void RunAfterAnyTest()
        {
            StopTest(testContainerId, 0);
            StopManager(managerContainerId, 0);
            StopManager(managerContainerId1, 1);
        }
    }
}
