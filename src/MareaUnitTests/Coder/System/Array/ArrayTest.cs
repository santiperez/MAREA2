using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using Marea;
using NUnit.Framework;

namespace MareaUnitTests.Coder.System.Array
{
    [TestFixture]
    class ArrayTest
    {
        private byte[] seralizedData = null;
        private long clock_freq, start, serializeTicks, deserializeTicks;
        private IpTransportAddress[] oIPA;
        private IpTransportAddress[] rIPA;
        private int length = 100;

        public ArrayTest()
        {
            Init();
        }

        public void Init()
        {
            clock_freq = PerformanceTimer.Clock_freq();
            oIPA = new IpTransportAddress[length];
            rIPA = new IpTransportAddress[length];
        }

        [SetUp]
        public void RunAfterAnyTest()
        {
            serializeTicks = 0;
            deserializeTicks = 0;

            for (int i = 0; i < length; i++)
            {
                oIPA[i] = new IpTransportAddress(IPAddress.Parse("127.0.0.1"),i, TransportMode.UDP);
                rIPA[i] = new IpTransportAddress();
            }

        }

        [TestCase, NUnit.Framework.Description("Coder(System.Collections.Array)")]
        public void TestArrayM2()
        {
            for (int i = 0; i < CoderTestsConstants.CODIFICATIONS; i++)
            {
                start = PerformanceTimer.Ticks();
                seralizedData = AdaptedMareaCoder.Send(oIPA);
                serializeTicks += PerformanceTimer.TicksDifference(start);

                start = PerformanceTimer.Ticks();
                rIPA = (IpTransportAddress[])AdaptedMareaCoder.Receive(seralizedData);
                deserializeTicks += PerformanceTimer.TicksDifference(start);
            }

            Console.WriteLine(CoderTestsConstants.MAREA2);
            Results results = ResultsManager.GetResults(serializeTicks, deserializeTicks, clock_freq, CoderTestsConstants.CODIFICATIONS, seralizedData.Length, oIPA.GetType().FullName);

            if (oIPA[0].Equals(rIPA[0]) && oIPA[oIPA.Length / 2].Equals(rIPA[oIPA.Length / 2]) && oIPA[oIPA.Length - 1].Equals(rIPA[oIPA.Length - 1]))
            {
                Assert.True(true);
                Console.WriteLine(CoderTestsConstants.OK_STATE);
                Console.WriteLine(results.ToString());
            }
            else
            {
                Console.WriteLine(CoderTestsConstants.KO_STATE);
                Assert.True(false);
            }
        }

        [TestCase, NUnit.Framework.Description("Coder(System.Collections.Array)[null]")]
        public void TestArrayM2Null()
        {
            oIPA = null;

            for (int i = 0; i < CoderTestsConstants.CODIFICATIONS; i++)
            {
                start = PerformanceTimer.Ticks();
                seralizedData = AdaptedMareaCoder.Send(oIPA);
                serializeTicks += PerformanceTimer.TicksDifference(start);

                start = PerformanceTimer.Ticks();
                rIPA = (IpTransportAddress[])AdaptedMareaCoder.Receive(seralizedData);
                deserializeTicks += PerformanceTimer.TicksDifference(start);
            }

            Console.WriteLine(CoderTestsConstants.MAREA2);
            Results results;
            if (oIPA == null)
                results = ResultsManager.GetResults(serializeTicks, deserializeTicks, clock_freq, CoderTestsConstants.CODIFICATIONS, seralizedData.Length, typeof(IPAddress[]).FullName + "(null)");
            else
                results = ResultsManager.GetResults(serializeTicks, deserializeTicks, clock_freq, CoderTestsConstants.CODIFICATIONS, seralizedData.Length, oIPA.GetType().FullName);


            if (oIPA == rIPA)
            {
                Assert.True(true);
                Console.WriteLine(CoderTestsConstants.OK_STATE);
                Console.WriteLine(results.ToString());
            }
            else
            {
                Console.WriteLine(CoderTestsConstants.KO_STATE);
                Assert.True(false);
            }
        }
    }
}
