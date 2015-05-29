using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Marea;
using NUnit.Framework;

namespace MareaUnitTests.Coder.System.Array
{
    [TestFixture]
    class DoubleArrayTest
    {
        private byte[] seralizedData = null;
        private long clock_freq, start, serializeTicks, deserializeTicks;
        private double[] oDoubleArray;
        private double[] rDoubleArray;
        private const int length = 256;

        public DoubleArrayTest()
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

            oDoubleArray= new double[length];
            rDoubleArray = new double[length];

            int i;
            for (i = 0; i < length; i++)
            {
                oDoubleArray[i] = 0.100000234523 + i;
                rDoubleArray[i] = 0;
            }
        }

        [TestCase, NUnit.Framework.Description("Coder(double[], System.Double[])")]
        public void TestDoubleArrayM2()
        {
            for (int i = 0; i < CoderTestsConstants.CODIFICATIONS; i++)
            {
                start = PerformanceTimer.Ticks();
                seralizedData = AdaptedMareaCoder.Send(oDoubleArray);
                serializeTicks += PerformanceTimer.TicksDifference(start);

                start = PerformanceTimer.Ticks();
                rDoubleArray = (double[])AdaptedMareaCoder.Receive(seralizedData);
                deserializeTicks += PerformanceTimer.TicksDifference(start);
            }

            Console.WriteLine(CoderTestsConstants.MAREA2);
            Results results = ResultsManager.GetResults(serializeTicks, deserializeTicks, clock_freq, CoderTestsConstants.CODIFICATIONS, seralizedData.Length, oDoubleArray.GetType().FullName);

            if (oDoubleArray[0] == rDoubleArray[0] && oDoubleArray[length / 2] == rDoubleArray[length / 2] && oDoubleArray[length-1] == rDoubleArray[length-1]) 
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

        [TestCase, NUnit.Framework.Description("Coder(double[], System.Double[])[null]")]
        public void TestDoubleArrayM2Null()
        {
            oDoubleArray = null;

            for (int i = 0; i < CoderTestsConstants.CODIFICATIONS; i++)
            {
                start = PerformanceTimer.Ticks();
                seralizedData = AdaptedMareaCoder.Send(oDoubleArray);
                serializeTicks += PerformanceTimer.TicksDifference(start);

                start = PerformanceTimer.Ticks();
                rDoubleArray = (double[])AdaptedMareaCoder.Receive(seralizedData);
                deserializeTicks += PerformanceTimer.TicksDifference(start);
            }

            Console.WriteLine(CoderTestsConstants.MAREA2);
            Results results;
            if (oDoubleArray == null)
                results = ResultsManager.GetResults(serializeTicks, deserializeTicks, clock_freq, CoderTestsConstants.CODIFICATIONS, seralizedData.Length, typeof(double[]).FullName + "(null)");
            else
                results = ResultsManager.GetResults(serializeTicks, deserializeTicks, clock_freq, CoderTestsConstants.CODIFICATIONS, seralizedData.Length, oDoubleArray.GetType().FullName);
            

            if (oDoubleArray == rDoubleArray)
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
