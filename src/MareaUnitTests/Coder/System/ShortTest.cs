using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Marea;
using NUnit.Framework;


namespace MareaUnitTests.Coder.System
{
    /**
    * C# Type->short 
    * .NET Framework Type->System.Int16
    **/
    [TestFixture]
    class ShortTest
    {

         private byte[] seralizedData = null;
        private long clock_freq, start, serializeTicks, deserializeTicks;

        public ShortTest()
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

        [TestCase, NUnit.Framework.Description("Coder(short, System.Int16)")]
        public void TestShortM2()
        {
            short oShort, rShort;
            oShort = short.MinValue;
            rShort = 0;

            for (int i = 0; i < CoderTestsConstants.CODIFICATIONS; i++)
            {
                start = PerformanceTimer.Ticks();
                seralizedData = AdaptedMareaCoder.Send(oShort);
                serializeTicks += PerformanceTimer.TicksDifference(start);

                start = PerformanceTimer.Ticks();
                rShort = (short)AdaptedMareaCoder.Receive(seralizedData);
                deserializeTicks += PerformanceTimer.TicksDifference(start);
            }

            Console.WriteLine(CoderTestsConstants.MAREA2);
            Results results = ResultsManager.GetResults(serializeTicks, deserializeTicks, clock_freq, CoderTestsConstants.CODIFICATIONS, seralizedData.Length, rShort.GetType().FullName);

            if (oShort == rShort)
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
