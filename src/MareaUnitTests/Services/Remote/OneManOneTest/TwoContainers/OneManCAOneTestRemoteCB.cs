using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;

using Marea;
using Examples;
using System.Threading;
using MareaUnitTests.Services.Local;
using MareaUnitTests.Services.Local.OneManOneTest;

namespace MareaUnitTests.Services.Remote.OneManOneTest.TwoContainers
{
    [TestFixture]
    class OneManCAOneTestRemoteCB : OneManOneTestBase
    {
        public OneManCAOneTestRemoteCB() : base(2)
        {
            managerContainerId = 0;
            testContainerId = 1;

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
        }

        [TearDown]
        public override void RunAfterAnyTest()
        {
            StopManager(managerContainerId, 0);
            StopTest(testContainerId, 0);
        }
    }
}
