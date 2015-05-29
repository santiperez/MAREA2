using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Marea;
using NUnit.Framework;

namespace MareaUnitTests.Coder.System.Array
{
    [TestFixture]
    class ShortArrayTest
    {
        private byte[] seralizedData = null;
        private long clock_freq, start, serializeTicks, deserializeTicks;
        private short[] oShortArray;
        private short[] rShortArray;
        private const int length = 256;

        public ShortArrayTest()
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

            oShortArray= new short[length];
            rShortArray = new short[length];

            int i;
            for (i = 0; i < length; i++)
            {
                oShortArray[i] = (short)(short.MinValue +Convert.ToInt16(i));
                rShortArray[i] = 0;
            }
        }

        [TestCase, NUnit.Framework.Description("Coder(short[] ,System.Int16[])")]
        public void TestShortArrayM2()
        {
            for (int i = 0; i < CoderTestsConstants.CODIFICATIONS; i++)
            {
                start = PerformanceTimer.Ticks();
                seralizedData = AdaptedMareaCoder.Send(oShortArray);
                serializeTicks += PerformanceTimer.TicksDifference(start);

                start = PerformanceTimer.Ticks();
                rShortArray = (short[])AdaptedMareaCoder.Receive(seralizedData);
                deserializeTicks += PerformanceTimer.TicksDifference(start);
            }

            Console.WriteLine(CoderTestsConstants.MAREA2);
            Results results = ResultsManager.GetResults(serializeTicks, deserializeTicks, clock_freq, CoderTestsConstants.CODIFICATIONS, seralizedData.Length, oShortArray.GetType().FullName);

            if (oShortArray[0] == rShortArray[0] && oShortArray[length / 2] == rShortArray[length / 2] && oShortArray[length-1] == rShortArray[length-1]) 
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

        [TestCase, NUnit.Framework.Description("Coder(short[] ,System.Int16[])")]
        public void TestShortArrayM2Null()
        {
            oShortArray = null;

            for (int i = 0; i < CoderTestsConstants.CODIFICATIONS; i++)
            {
                start = PerformanceTimer.Ticks();
                seralizedData = AdaptedMareaCoder.Send(oShortArray);
                serializeTicks += PerformanceTimer.TicksDifference(start);

                start = PerformanceTimer.Ticks();
                rShortArray = (short[])AdaptedMareaCoder.Receive(seralizedData);
                deserializeTicks += PerformanceTimer.TicksDifference(start);
            }

            Console.WriteLine(CoderTestsConstants.MAREA2);
            Results results;
            if (oShortArray == null)
                results = ResultsManager.GetResults(serializeTicks, deserializeTicks, clock_freq, CoderTestsConstants.CODIFICATIONS, seralizedData.Length, typeof(short[]).FullName + "(null)");
            else
                results = ResultsManager.GetResults(serializeTicks, deserializeTicks, clock_freq, CoderTestsConstants.CODIFICATIONS, seralizedData.Length, oShortArray.GetType().FullName);
            

            if (oShortArray == rShortArray)
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
