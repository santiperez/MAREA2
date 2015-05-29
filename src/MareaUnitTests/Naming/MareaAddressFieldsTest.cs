using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using System.Net;

using Marea;

namespace MareaUnitTests.Naming
{
    [TestFixture]
    class MareaAddressFieldsTest
    {
        private MareaAddress mad;
        string subsystem;
        string node;
        string instance;
        string service;
        string primitive;

        public MareaAddressFieldsTest()
        {
            mad = new MareaAddress("!/*/*/*/*");
            subsystem = "EC_UPC";
            node = "127.0.0.1:2323";
            instance = "0";
            service = "service";
            primitive = "primitive";
        }

        [SetUp]
        public void RunAfterAnyTest()
        {

        }

        [TestCase, NUnit.Framework.Description("Naming(TestSubsystem)")]
        public void Test00Subsystem()
        {

            mad.SetSubsystem(subsystem);
            Assert.AreEqual(subsystem, mad.GetSubsystem());
        }

        [TestCase, NUnit.Framework.Description("Naming(TestNodeAsString)")]
        public void Test01NodeAsString()
        {
            mad.SetNode(node);
            Assert.AreEqual(node, mad.GetNode());
        }

        [TestCase, NUnit.Framework.Description("Naming(TestNode)")]
        public void Test02Node()
        {
            Assert.AreEqual(node, mad.GetNodeAsIPEndPoint().ToString());
        }

        [TestCase, NUnit.Framework.Description("Naming(TestNodeAsStringException)")]
        public void Test03NodeAsStringException()
        {
            bool exception = false;
            try
            {
                mad.SetNode("12.32.1e:2323");
            }
            catch (FormatException)
            {
                mad.SetNode(node);
                exception = true;
            }
            Assert.True(exception);
        }

        [TestCase, NUnit.Framework.Description("Naming(TestInstance)")]
        public void Test04Instance()
        {
            mad.SetInstance(instance);
            Assert.AreEqual(instance, mad.GetInstance());
        }

        [TestCase, NUnit.Framework.Description("Naming(TestService)")]
        public void Test05Service()
        {
            mad.SetService(service);
            Assert.AreEqual(service, mad.GetService());
        }

        [TestCase, NUnit.Framework.Description("Naming(TestServiceAddress)")]
        public void Test06ServiceAddress()
        {
            Assert.AreEqual("!/" + subsystem + "/" + node + "/" + instance + "/" + service, mad.GetServiceAddress());
        }

        [TestCase, NUnit.Framework.Description("Naming(TestPrimitiveNull)")]
        public void Test07Primitive()
        {
            Assert.IsNull(mad.GetPrimitive());
        }

        [TestCase, NUnit.Framework.Description("Naming(TestPrimitive)")]
        public void Test08Primitive()
        {
            mad.SetPrimitive(primitive);
            Assert.AreEqual(primitive, mad.GetPrimitive());
        }

        [TestCase, NUnit.Framework.Description("Naming(TestPrimitiveAddress)")]
        public void Test09PrimitiveAddress()
        {
            Assert.AreEqual("!/" + subsystem + "/" + node + "/" + instance + "/" + service + "/" + primitive, mad.GetPrimitiveAddress());
        }

        [TestCase, NUnit.Framework.Description("Naming(TesNameResolutionTypeLock)")]
        public void Test10NameResolutionTypeLock()
        {
            Assert.AreEqual(mad.GetNamingResolutionType(), NameResolutionType.Locked);
        }

        [TestCase, NUnit.Framework.Description("Naming(TesNameResolutionTypeStatic)")]
        public void Test11NameResolutionTypeStatic()
        {
            mad.SetNamingResolutionType(NameResolutionType.Static);
            Assert.AreEqual("#/" + subsystem + "/" + node + "/" + instance + "/" + service + "/" + primitive, mad.GetPrimitiveAddress());
        }

        [TestCase, NUnit.Framework.Description("Naming(TesNameResolutionTypeNone)")]
        public void Test12NameResolutionTypeNone()
        {
            mad.SetNamingResolutionType(NameResolutionType.None);
            Assert.AreEqual("/" + subsystem + "/" + node + "/" + instance + "/" + service + "/" + primitive, mad.GetPrimitiveAddress());
        }

        [TestCase, NUnit.Framework.Description("Naming(TesNameResolutionTypeAll)")]
        public void Test13NameResolutionTypeNone()
        {
            mad.SetNamingResolutionType(NameResolutionType.All);
            Assert.AreEqual("*/" + subsystem + "/" + node + "/" + instance + "/" + service + "/" + primitive, mad.GetPrimitiveAddress());
        }
    }
}
