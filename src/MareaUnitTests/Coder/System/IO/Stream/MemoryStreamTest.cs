using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using Marea;
using System.IO;

namespace MareaUnitTests.Coder.System.IO
{
    class MemoryStreamTest
    {

        private byte[] seralizedData = null;
        private const int length = 120;
        private MemoryStream oMemoryStream;
        private MemoryStream rMemoryStream;

        public MemoryStreamTest()
        {
            Init();
        }

        public void Init()
        {

            byte[] arrBytes = File.ReadAllBytes(@"MareaInterface.dll");
            oMemoryStream = new MemoryStream(arrBytes);
        }

        [SetUp]
        public void RunAfterAnyTest()
        {
            rMemoryStream = new MemoryStream();
        }

        [TestCase, NUnit.Framework.Description("Coder(System.IO.MemoryStream)")]
        public void TestMemoryStreamM2()
        {
            seralizedData = AdaptedMareaCoder.Send(oMemoryStream);
            rMemoryStream = (MemoryStream)AdaptedMareaCoder.Receive(seralizedData);
            Console.WriteLine(CoderTestsConstants.MAREA2);

            if (oMemoryStream.Capacity == rMemoryStream.Capacity && oMemoryStream.CanRead == rMemoryStream.CanRead && oMemoryStream.CanSeek == rMemoryStream.CanSeek
                && oMemoryStream.CanTimeout == rMemoryStream.CanTimeout && oMemoryStream.CanWrite == rMemoryStream.CanWrite && oMemoryStream.Length == rMemoryStream.Length
                && oMemoryStream.ToArray()[0] == rMemoryStream.ToArray()[0]
                && oMemoryStream.ToArray()[rMemoryStream.Length - 1] == rMemoryStream.ToArray()[oMemoryStream.Length - 1]
                && oMemoryStream.ToArray()[rMemoryStream.Length / 2 - 1] == rMemoryStream.ToArray()[oMemoryStream.Length / 2 - 1])
            {
                Assert.True(true);
                Console.WriteLine(CoderTestsConstants.OK_STATE);
            }
            else
            {
                Console.WriteLine(CoderTestsConstants.KO_STATE);
                Assert.True(false);
            }
        }
    }
}
