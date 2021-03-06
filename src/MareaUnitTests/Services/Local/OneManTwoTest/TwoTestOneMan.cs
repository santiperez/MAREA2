﻿using System;
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
    class TwoTestOneMan:OneManTwoTestBase
    {
        public TwoTestOneMan():base(1)
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
        }

        [TearDown]
        public override void RunAfterAnyTest()
        {
            StopTest(testContainerId, 0);
            StopTest(testContainerId1, 1);
            StopManager(managerContainerId, 0);
        }
    }
}
