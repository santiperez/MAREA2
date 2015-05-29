using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using Marea;

namespace MareaUnitTests.Coder.System
{
    /**
    * C# Type->ushort
    * .NET Framework Type->System.UInt16
    **/
    [TestFixture]
    class UShortTest
    {
        private byte[] seralizedData = null;
        private long clock_freq, start, serializeTicks, deserializeTicks;

        public UShortTest()
        {
            Init();
        }

        public void Init()
        {
            clock_freq = PerformanceTimer.Clock_freq();
        }

        [SetUp]
        public void RunAfterAnyTest()
        {
            serializeTicks = 0;
            deserializeTicks = 0;
        }

        [TestCase, NUnit.Framework.Description("Coder(ushort, System.UInt16)")]
        public void TestUShortM2()
        {
            ushort oUShort, rUShort;
            oUShort = ushort.MaxValue;
            rUShort = 0;

            for (int i = 0; i < CoderTestsConstants.CODIFICATIONS; i++)
            {
                start = PerformanceTimer.Ticks();
                seralizedData = AdaptedMareaCoder.Send(oUShort);
                serializeTicks += PerformanceTimer.TicksDifference(start);

                start = PerformanceTimer.Ticks();
                rUShort = (ushort)AdaptedMareaCoder.Receive(seralizedData);
                deserializeTicks += PerformanceTimer.TicksDifference(start);
            }

            Console.WriteLine(CoderTestsConstants.MAREA2);
            Results results = ResultsManager.GetResults(serializeTicks, deserializeTicks, clock_freq, CoderTestsConstants.CODIFICATIONS, seralizedData.Length, rUShort.GetType().FullName);

            if (oUShort == rUShort)
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
