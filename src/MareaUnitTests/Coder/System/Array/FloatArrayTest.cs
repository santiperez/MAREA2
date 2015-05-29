using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Marea;
using NUnit.Framework;

namespace MareaUnitTests.Coder.System.Array
{
    [TestFixture]
    class FloatArrayTest
    {
        private byte[] seralizedData = null;
        private long clock_freq, start, serializeTicks, deserializeTicks;
        private float[] oFloatArray;
        private float[] rFloatArray;
        private const int length = 256;

        public FloatArrayTest()
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

            oFloatArray= new float[length];
            rFloatArray = new float[length];

            int i;
            float f = 0.4234f;
            for (i = 0; i < length; i++)
            {
                oFloatArray[i] = f+Convert.ToSingle(i);
                rFloatArray[i] = 0;
            }
        }

        [TestCase, NUnit.Framework.Description("Coder(float[], System.Single[])")]
        public void TestFloatArrayM2()
        {
            for (int i = 0; i < CoderTestsConstants.CODIFICATIONS; i++)
            {
                start = PerformanceTimer.Ticks();
                seralizedData = AdaptedMareaCoder.Send(oFloatArray);
                serializeTicks += PerformanceTimer.TicksDifference(start);

                start = PerformanceTimer.Ticks();
                rFloatArray = (float[])AdaptedMareaCoder.Receive(seralizedData);
                deserializeTicks += PerformanceTimer.TicksDifference(start);
            }

            Console.WriteLine(CoderTestsConstants.MAREA2);
            Results results = ResultsManager.GetResults(serializeTicks, deserializeTicks, clock_freq, CoderTestsConstants.CODIFICATIONS, seralizedData.Length, oFloatArray.GetType().FullName);

            if (oFloatArray[0] == rFloatArray[0] && oFloatArray[length / 2] == rFloatArray[length / 2] && oFloatArray[length-1] == rFloatArray[length-1]) 
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

        [TestCase, NUnit.Framework.Description("Coder(float[], System.Single[])[null]")]
        public void TestFloatArrayM2Null()
        {
            oFloatArray = null;

            for (int i = 0; i < CoderTestsConstants.CODIFICATIONS; i++)
            {
                start = PerformanceTimer.Ticks();
                seralizedData = AdaptedMareaCoder.Send(oFloatArray);
                serializeTicks += PerformanceTimer.TicksDifference(start);

                start = PerformanceTimer.Ticks();
                rFloatArray = (float[])AdaptedMareaCoder.Receive(seralizedData);
                deserializeTicks += PerformanceTimer.TicksDifference(start);
            }

            Console.WriteLine(CoderTestsConstants.MAREA2);
            Results results;
            if (oFloatArray == null)
                results = ResultsManager.GetResults(serializeTicks, deserializeTicks, clock_freq, CoderTestsConstants.CODIFICATIONS, seralizedData.Length, typeof(float[]).FullName + "(null)");
            else
                results = ResultsManager.GetResults(serializeTicks, deserializeTicks, clock_freq, CoderTestsConstants.CODIFICATIONS, seralizedData.Length, oFloatArray.GetType().FullName);
            

            if (oFloatArray == rFloatArray)
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
