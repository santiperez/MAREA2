using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Marea;
using NUnit.Framework;

namespace MareaUnitTests.Coder.System.Array
{
    [TestFixture]
    class UShortArrayTest
    {
        private byte[] seralizedData = null;
        private long clock_freq, start, serializeTicks, deserializeTicks;
        private ushort[] oUShortArray;
        private ushort[] rUShortArray;
        private const int length = 256;

        public UShortArrayTest()
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

            oUShortArray= new ushort[length];
            rUShortArray = new ushort[length];

            int i;
            for (i = 0; i < length; i++)
            {
                oUShortArray[i] = (ushort)(ushort.MinValue +Convert.ToInt16(i));
                rUShortArray[i] = 0;
            }
        }

        [TestCase, NUnit.Framework.Description("Coder(ushort[] ,System.UInt16[])")]
        public void TestUShortArrayM2()
        {
            for (int i = 0; i < CoderTestsConstants.CODIFICATIONS; i++)
            {
                start = PerformanceTimer.Ticks();
                seralizedData = AdaptedMareaCoder.Send(oUShortArray);
                serializeTicks += PerformanceTimer.TicksDifference(start);

                start = PerformanceTimer.Ticks();
                rUShortArray = (ushort[])AdaptedMareaCoder.Receive(seralizedData);
                deserializeTicks += PerformanceTimer.TicksDifference(start);
            }

            Console.WriteLine(CoderTestsConstants.MAREA2);
            Results results = ResultsManager.GetResults(serializeTicks, deserializeTicks, clock_freq, CoderTestsConstants.CODIFICATIONS, seralizedData.Length, oUShortArray.GetType().FullName);

            if (oUShortArray[0] == rUShortArray[0] && oUShortArray[length / 2] == rUShortArray[length / 2] && oUShortArray[length-1] == rUShortArray[length-1]) 
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

        [TestCase, NUnit.Framework.Description("Coder(ushort[] ,System.UInt16[])")]
        public void TestUShortArrayM2Null()
        {
            oUShortArray = null;

            for (int i = 0; i < CoderTestsConstants.CODIFICATIONS; i++)
            {
                start = PerformanceTimer.Ticks();
                seralizedData = AdaptedMareaCoder.Send(oUShortArray);
                serializeTicks += PerformanceTimer.TicksDifference(start);

                start = PerformanceTimer.Ticks();
                rUShortArray = (ushort[])AdaptedMareaCoder.Receive(seralizedData);
                deserializeTicks += PerformanceTimer.TicksDifference(start);
            }

            Console.WriteLine(CoderTestsConstants.MAREA2);
            Results results;
            if (oUShortArray == null)
                results = ResultsManager.GetResults(serializeTicks, deserializeTicks, clock_freq, CoderTestsConstants.CODIFICATIONS, seralizedData.Length, typeof(ushort[]).FullName + "(null)");
            else
                results = ResultsManager.GetResults(serializeTicks, deserializeTicks, clock_freq, CoderTestsConstants.CODIFICATIONS, seralizedData.Length, oUShortArray.GetType().FullName);
            

            if (oUShortArray == rUShortArray)
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
