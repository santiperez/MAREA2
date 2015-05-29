using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Marea;
using NUnit.Framework;

namespace MareaUnitTests.Coder.System.Collections.Generics
{
    [TestFixture]
    class ListTest
    {
        private byte[] seralizedData = null;
        private long clock_freq, start, serializeTicks, deserializeTicks;
        private const int length=120;
        private List<int> oIntList;
        private List<int> rIntList;

        public ListTest()
        {
            Init();
        }

        public void Init()
        {
            clock_freq = PerformanceTimer.Clock_freq();
            oIntList = new List<int>();

            for (int i = 0; i < length; i++)
                oIntList.Add(i);
        }

        [SetUp]
        public void RunAfterAnyTest()
        {
            serializeTicks = 0;
            deserializeTicks = 0;

            rIntList = new List<int>();

        }

        [TestCase, NUnit.Framework.Description("Coder(System.Collections.List<int>)[null])")]
        public void TestListIntNullM2()
        {
            oIntList = null;

            for (int i = 0; i < CoderTestsConstants.CODIFICATIONS; i++)
            {
                start = PerformanceTimer.Ticks();
                seralizedData = AdaptedMareaCoder.Send(oIntList);
                serializeTicks += PerformanceTimer.TicksDifference(start);

                start = PerformanceTimer.Ticks();
                rIntList = (List<int>)AdaptedMareaCoder.Receive(seralizedData);
                deserializeTicks += PerformanceTimer.TicksDifference(start);
            }

            Console.WriteLine(CoderTestsConstants.MAREA2);
            Results results=null;

            if (oIntList== null)
                results = ResultsManager.GetResults(serializeTicks, deserializeTicks, clock_freq, CoderTestsConstants.CODIFICATIONS, seralizedData.Length, typeof(List<int>).FullName + "(null)");
            else
                 results = ResultsManager.GetResults(serializeTicks, deserializeTicks, clock_freq, CoderTestsConstants.CODIFICATIONS, seralizedData.Length, oIntList.GetType().FullName);
            
            if (oIntList==rIntList && oIntList==null)
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

        [TestCase, NUnit.Framework.Description("Coder(System.Collections.List<int>)")]
        public void TestListIntM2()
        {
            for (int i = 0; i < CoderTestsConstants.CODIFICATIONS; i++)
            {
                start = PerformanceTimer.Ticks();
                seralizedData = AdaptedMareaCoder.Send(oIntList);
                serializeTicks += PerformanceTimer.TicksDifference(start);

                start = PerformanceTimer.Ticks();
                rIntList = (List<int>)AdaptedMareaCoder.Receive(seralizedData);
                deserializeTicks += PerformanceTimer.TicksDifference(start);
            }

            Console.WriteLine(CoderTestsConstants.MAREA2);
            Results results = ResultsManager.GetResults(serializeTicks, deserializeTicks, clock_freq, CoderTestsConstants.CODIFICATIONS, seralizedData.Length, oIntList.GetType().FullName);

            if (oIntList[0].Equals(rIntList[0]) && oIntList[oIntList.Count / 2].Equals(rIntList[oIntList.Count / 2]) && oIntList[oIntList.Count - 1].Equals(rIntList[oIntList.Count - 1]))
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
