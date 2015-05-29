using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;

using Marea;
using Examples;

namespace MareaUnitTests.Services.Local.TwoManTwoTest
{
    [TestFixture]
    class TwoTestTwoMan:TwoManTwoTestBase
    {
        public TwoTestTwoMan(): base(1)
        {
        }

        [TestFixtureSetUp]
        public override void Init()
        {
            StartContainer(0);
        }

        [TestFixtureTearDown]
        public override void Dispose()
        {
            StopContainer(0);
        }


        [SetUp]
        public override void RunBeforeAnyTest()
        {
            test = StartTest(testContainerId, 0);
            test1 = StartTest(testContainerId1, 1);
            testManager = StartManager(managerContainerId, 0);
            testManager1 = StartManager(managerContainerId1, 1);
        }

        [TearDown]
        public override void RunAfterAnyTest()
        {
            StopTest(testContainerId, 0);
            StopTest(testContainerId1, 1);
            StopManager(managerContainerId, 0);
            StopManager(managerContainerId1, 1);
        }
    }
}
