using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using Marea;
using Examples;
using System.Threading;

namespace MareaUnitTests.Services.BasicTests
{
    [TestFixture]
    class ServiceContainerTest : ServiceTestBase
    {
        public static int testLength = 1;
        public static int managersLength = 1;
        public static int containersLength = 1;

        public ServiceContainerTest() : base(containersLength,testLength, managersLength) { }

        [TestCase, NUnit.Framework.Description("Services.ServiceContainerTest(Create Service Container)")]
        public void Test00CreateContainer()
        {
            StartContainer(0);
            Assert.True(StopContainer(0));
        }
		 
    }
}
