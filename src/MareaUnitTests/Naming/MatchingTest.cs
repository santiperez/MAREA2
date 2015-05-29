using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Marea;
using NUnit.Framework;

namespace MareaUnitTests.Naming
{
    [TestFixture]
    class MatchingTest
    {

        public MatchingTest()
        {

        }

        [SetUp]
        public void RunAfterAnyTest()
        {

        }

        [TestCase, NUnit.Framework.Description("Naming(Matching00)")]
        public void TestMatching00()
        {
            Assert.True(QueryManager.MareaAddressMatchesWithQueryAddress(new MareaAddress("/EC-UPC/192.168.1.150:11500/0/Examples.Battery"), new MareaAddress("!/*/*/*/Examples.Battery")));
        }

        [TestCase, NUnit.Framework.Description("Naming(Matching01)")]
        public void TestMatching01()
        {
            Assert.True(QueryManager.MareaAddressMatchesWithQueryAddress(new MareaAddress("/EC-UPC/192.168.1.150:11500/0/Examples.Battery"), new MareaAddress("/*/*/0/*")));
        }

        [TestCase, NUnit.Framework.Description("Naming(Matching02)")]
        public void TestMatching02()
        {
            Assert.True(QueryManager.MareaAddressMatchesWithQueryAddress(new MareaAddress("/EC-UPC/192.168.1.150:11500/0/Examples.Battery"), new MareaAddress("/*/*/0/Examples.Battery")));
        }

        [TestCase, NUnit.Framework.Description("Naming(Matching03)")]
        public void TestMatching03()
        {
            Assert.True(QueryManager.MareaAddressMatchesWithQueryAddress(new MareaAddress("/EC-UPC/192.168.1.150:11500/0/Examples.Battery"), new MareaAddress("/*/192.168.1.150:11500/*/*")));
        }

        [TestCase, NUnit.Framework.Description("Naming(Matching04)")]
        public void TestMatching04()
        {
            Assert.True(QueryManager.MareaAddressMatchesWithQueryAddress(new MareaAddress("/EC-UPC/192.168.1.150:11500/0/Examples.Battery"), new MareaAddress("/*/192.168.1.150:11500/*/Examples.Battery")));
        }

        [TestCase, NUnit.Framework.Description("Naming(Matching05)")]
        public void TestMatching05()
        {
            Assert.True(QueryManager.MareaAddressMatchesWithQueryAddress(new MareaAddress("/EC-UPC/192.168.1.150:11500/0/Examples.Battery"), new MareaAddress("/*/192.168.1.150:11500/0/*")));
        }

        [TestCase, NUnit.Framework.Description("Naming(Matching06)")]
        public void TestMatching06()
        {
            Assert.True(QueryManager.MareaAddressMatchesWithQueryAddress(new MareaAddress("/EC-UPC/192.168.1.150:11500/0/Examples.Battery"), new MareaAddress("/*/192.168.1.150:11500/0/Examples.Battery")));
        }

        [TestCase, NUnit.Framework.Description("Naming(Matching07)")]
        public void TestMatching07()
        {
            Assert.True(QueryManager.MareaAddressMatchesWithQueryAddress(new MareaAddress("/EC-UPC/192.168.1.150:11500/0/Examples.Battery"), new MareaAddress("/EC-UPC/*/*/*")));
        }

        [TestCase, NUnit.Framework.Description("Naming(Matching08)")]
        public void TestMatching08()
        {
            Assert.True(QueryManager.MareaAddressMatchesWithQueryAddress(new MareaAddress("/EC-UPC/192.168.1.150:11500/0/Examples.Battery"), new MareaAddress("/EC-UPC/*/*/Examples.Battery")));
        }

        [TestCase, NUnit.Framework.Description("Naming(Matching09)")]
        public void TestMatching09()
        {
            Assert.True(QueryManager.MareaAddressMatchesWithQueryAddress(new MareaAddress("/EC-UPC/192.168.1.150:11500/0/Examples.Battery"), new MareaAddress("/EC-UPC/*/0/*")));
        }

        [TestCase, NUnit.Framework.Description("Naming(Matching10)")]
        public void TestMatching10()
        {
            Assert.True(QueryManager.MareaAddressMatchesWithQueryAddress(new MareaAddress("/EC-UPC/192.168.1.150:11500/0/Examples.Battery"), new MareaAddress("/EC-UPC/*/0/Examples.Battery")));
        }


        [TestCase, NUnit.Framework.Description("Naming(Matching11)")]
        public void TestMatching11()
        {
            Assert.True(QueryManager.MareaAddressMatchesWithQueryAddress(new MareaAddress("/EC-UPC/192.168.1.150:11500/0/Examples.Battery"), new MareaAddress("/EC-UPC/192.168.1.150:11500/*/*")));
        }

        [TestCase, NUnit.Framework.Description("Naming(Matching12)")]
        public void TestMatching12()
        {
            Assert.True(QueryManager.MareaAddressMatchesWithQueryAddress(new MareaAddress("/EC-UPC/192.168.1.150:11500/0/Examples.Battery"), new MareaAddress("/EC-UPC/192.168.1.150:11500/*/Examples.Battery")));
        }

        [TestCase, NUnit.Framework.Description("Naming(Matching13)")]
        public void TestMatching13()
        {
            Assert.True(QueryManager.MareaAddressMatchesWithQueryAddress(new MareaAddress("/EC-UPC/192.168.1.150:11500/0/Examples.Battery"), new MareaAddress("/EC-UPC/192.168.1.150:11500/0/*")));
        }

        [TestCase, NUnit.Framework.Description("Naming(Matching14)")]
        public void TestMatching14()
        {
            Assert.True(QueryManager.MareaAddressMatchesWithQueryAddress(new MareaAddress("/EC-UPC/192.168.1.150:11500/0/Examples.Battery"), new MareaAddress("/EC-UPC/192.168.1.150:11500/0/Examples.Battery")));
        }

        [TestCase, NUnit.Framework.Description("Naming(Matching15)")]
        public void TestMatching15()
        {
            Assert.True(QueryManager.MareaAddressMatchesWithQueryAddress(new MareaAddress("/EC-UPC/192.168.1.150:11500/0/Examples.Battery/primitive"), new MareaAddress("/*/192.168.1.150:11500/*/*/primitive")));
        }

        [TestCase, NUnit.Framework.Description("Naming(Matching16)")]
        public void TestMatching16()
        {
            Assert.True(QueryManager.MareaAddressMatchesWithQueryAddress(new MareaAddress("/EC-UPC/192.168.1.150:11500/0/Examples.Battery/primitive"), new MareaAddress("!/*/192.168.1.150:11500/*/*/primitive")));
        }
    }
}
