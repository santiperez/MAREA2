﻿using System;
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
    class OneManOneTestOneMan:TwoManOneTestBase
    {
        public OneManOneTestOneMan(): base(1)
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
            testManager = StartManager(managerContainerId, 0);
            test = StartTest(testContainerId, 0);
            testManager1 = StartManager(managerContainerId1, 1);
        }

        [TearDown]
        public override void RunAfterAnyTest()
        {
            StopManager(managerContainerId, 0);
            StopTest(testContainerId, 0);
            StopManager(managerContainerId1, 1);
        }
    }
}
