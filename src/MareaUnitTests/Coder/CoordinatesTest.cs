using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;

using Marea;
using MareaUnitTests;

namespace MareaUnitTests.Coder
{
    [TestFixture]
    class CoordinatesTest
    {
        private byte[] serializedData = null;
        private long clock_freq, start, serializeTicks, deserializeTicks;
        private Coordinates oCor, rCor;


        public CoordinatesTest()
        {
            Init();
        }

        public void Init()
        {
            clock_freq = PerformanceTimer.Clock_freq();
            deserializeTicks += PerformanceTimer.TicksDifference(start);
            oCor = new Coordinates(23.235667, 54.548989);
        }

        [SetUp]
        public void RunAfterAnyTest()
        {
            serializeTicks = 0;
            deserializeTicks = 0;
            rCor = null;
            
        }

        [TestCase, NUnit.Framework.Description("Coder(CoordinatesM2)")]
        public void TestClassCoordinatesM2()
        {
            
            for (int i = 0; i < CoderTestsConstants.CODIFICATIONS; i++)
            {
                start = PerformanceTimer.Ticks();
                serializedData = AdaptedMareaCoder.Send(oCor);
                serializeTicks += PerformanceTimer.TicksDifference(start);

                start = PerformanceTimer.Ticks();
                rCor = (Coordinates)AdaptedMareaCoder.Receive(serializedData);
                deserializeTicks += PerformanceTimer.TicksDifference(start);
            }

            Console.WriteLine(CoderTestsConstants.MAREA2);
            Results results = ResultsManager.GetResults(serializeTicks, deserializeTicks, clock_freq, CoderTestsConstants.CODIFICATIONS, serializedData.Length, oCor.GetType().FullName);

            if (oCor.x == rCor.x && oCor.y == rCor.y)
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
