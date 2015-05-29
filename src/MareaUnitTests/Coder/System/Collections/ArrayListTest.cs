using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Marea;
using System.Collections;
using NUnit.Framework;

namespace MareaUnitTests.Coder.System.Collections
{
    [TestFixture]
    class ArrayListTest
    {
        private byte[] seralizedData = null;
        private long clock_freq, start, serializeTicks, deserializeTicks;
        private ArrayList oArrayList;
        private ArrayList rArrayList;

        public ArrayListTest()
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

            oArrayList= new ArrayList();
            rArrayList = new ArrayList();

            int repeat = 100;
            int i = 0;

            for (i = 0; i < repeat;i++)
                oArrayList.Add(i);

            for ( i = 0; i < repeat; i++)
                oArrayList.Add("STRING["+i+"]");
            
            float f = 0.43422f;
            for (i = 0; i < repeat; i++)
                oArrayList.Add(f + Convert.ToSingle(i));

            double d = 0.100000234523;
            for (i = 0; i < repeat; i++)
                oArrayList.Add(d + Convert.ToDouble(i));

        }

        [TestCase, NUnit.Framework.Description("Coder(System.Collections.ArrayList)")]
        public void TestArrayListM2()
        {
            for (int i = 0; i < CoderTestsConstants.CODIFICATIONS; i++)
            {
                start = PerformanceTimer.Ticks();
                seralizedData = AdaptedMareaCoder.Send(oArrayList);
                serializeTicks += PerformanceTimer.TicksDifference(start);

                start = PerformanceTimer.Ticks();
                rArrayList = (ArrayList)AdaptedMareaCoder.Receive(seralizedData);
                deserializeTicks += PerformanceTimer.TicksDifference(start);
            }

            Console.WriteLine(CoderTestsConstants.MAREA2);
            Results results = ResultsManager.GetResults(serializeTicks, deserializeTicks, clock_freq, CoderTestsConstants.CODIFICATIONS, seralizedData.Length, oArrayList.GetType().FullName);

            if (oArrayList[0].Equals(rArrayList[0]) && oArrayList[oArrayList.Count / 2].Equals(rArrayList[oArrayList.Count / 2]) && oArrayList[oArrayList.Count - 1].Equals(rArrayList[oArrayList.Count - 1]))
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

        [TestCase, NUnit.Framework.Description("Coder(System.Collections.ArrayList)[null]")]
        public void TestArrayListM2Null()
        {
            oArrayList = null;

            for (int i = 0; i < CoderTestsConstants.CODIFICATIONS; i++)
            {
                start = PerformanceTimer.Ticks();
                seralizedData = AdaptedMareaCoder.Send(oArrayList);
                serializeTicks += PerformanceTimer.TicksDifference(start);

                start = PerformanceTimer.Ticks();
                rArrayList = (ArrayList)AdaptedMareaCoder.Receive(seralizedData);
                deserializeTicks += PerformanceTimer.TicksDifference(start);
            }

            Console.WriteLine(CoderTestsConstants.MAREA2);
            Results results;
            if (oArrayList == null)
                results = ResultsManager.GetResults(serializeTicks, deserializeTicks, clock_freq, CoderTestsConstants.CODIFICATIONS, seralizedData.Length, typeof(ArrayList).FullName + "(null)");
            else
                results = ResultsManager.GetResults(serializeTicks, deserializeTicks, clock_freq, CoderTestsConstants.CODIFICATIONS, seralizedData.Length, oArrayList.GetType().FullName);
            

            if (oArrayList == rArrayList)
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
