using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Marea;
using NUnit.Framework;

namespace MareaUnitTests.Coder.System
{
    /**
    * C# Type-enum
    * .NET Framework Type->System.Enum
    **/
    [TestFixture]
    class EnumTest
    {
        private enum EnumExample{Green, Red, Blue};
        private byte[] seralizedData = null;
        private long clock_freq, start, serializeTicks, deserializeTicks;

        public EnumTest()
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

        [TestCase, NUnit.Framework.Description("Coder(enum, System.Enum)")]
        public void TestEnumM2()
        {
            EnumExample oEnum, rEnum;
            oEnum = EnumExample.Blue;
            rEnum = EnumExample.Green;

            for (int i = 0; i < CoderTestsConstants.CODIFICATIONS; i++)
            {
                start = PerformanceTimer.Ticks();
                seralizedData = AdaptedMareaCoder.Send(oEnum);
                serializeTicks += PerformanceTimer.TicksDifference(start);

                start = PerformanceTimer.Ticks();
                rEnum = (EnumExample)AdaptedMareaCoder.Receive(seralizedData);
                deserializeTicks += PerformanceTimer.TicksDifference(start);
            }

            Console.WriteLine(CoderTestsConstants.MAREA2);
            Results results = ResultsManager.GetResults(serializeTicks, deserializeTicks, clock_freq, CoderTestsConstants.CODIFICATIONS, seralizedData.Length, rEnum.GetType().FullName);
            
            if (oEnum == rEnum)
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
