using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Marea;
using NUnit.Framework;
using System.Collections;

namespace MareaUnitTests.Coder.System.Collections
{
    [TestFixture]
    class HashtableTest
    {
        private byte[] seralizedData = null;
        private long clock_freq, start, serializeTicks, deserializeTicks;
        private Hashtable oHashtable;
        private Hashtable rHashtable;

        public HashtableTest()
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

            oHashtable= new Hashtable();
            rHashtable = new Hashtable();

            int repeat = 100;
            int i = 0;

            for (i = 0; i < repeat;i++)
                oHashtable.Add(i,"dasdasdas");

            for ( i = 0; i < repeat; i++)
                oHashtable.Add(Convert.ToChar(i),0.43432f+i);
            
            float f = 0.43422f;
            for (i = 0; i < repeat; i++)
                oHashtable.Add(i+repeat,f + Convert.ToSingle(i));

            double d = 0.100000234523;
            for (i = 0; i < repeat; i++)
                oHashtable.Add(Convert.ToChar(i+repeat), d + Convert.ToDouble(i));

        }

         [TestCase, NUnit.Framework.Description("Coder(System.Collections.Hashtable)")]
        public void TestHashtableM2()
        {
            for (int i = 0; i < CoderTestsConstants.CODIFICATIONS; i++)
            {
                start = PerformanceTimer.Ticks();
                seralizedData = AdaptedMareaCoder.Send(oHashtable);
                serializeTicks += PerformanceTimer.TicksDifference(start);

                start = PerformanceTimer.Ticks();
                rHashtable = (Hashtable)AdaptedMareaCoder.Receive(seralizedData);
                deserializeTicks += PerformanceTimer.TicksDifference(start);
            }

            Console.WriteLine(CoderTestsConstants.MAREA2);
            Results results = ResultsManager.GetResults(serializeTicks, deserializeTicks, clock_freq, CoderTestsConstants.CODIFICATIONS, seralizedData.Length, oHashtable.GetType().FullName);

            bool state=true;
            foreach (DictionaryEntry entry in oHashtable)
            {
                if(rHashtable[entry.Key]==null)
                        state = false;
                if (!rHashtable[entry.Key].Equals(oHashtable[entry.Key]))
                    state = false;
            }

            if (state)
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

         [TestCase, NUnit.Framework.Description("Coder(System.Collections.Hashtable)[null]")]
        public void TestHashtableM2Null()
        {
            oHashtable = null;

            for (int i = 0; i < CoderTestsConstants.CODIFICATIONS; i++)
            {
                start = PerformanceTimer.Ticks();
                seralizedData = AdaptedMareaCoder.Send(oHashtable);
                serializeTicks += PerformanceTimer.TicksDifference(start);

                start = PerformanceTimer.Ticks();
                rHashtable = (Hashtable)AdaptedMareaCoder.Receive(seralizedData);
                deserializeTicks += PerformanceTimer.TicksDifference(start);
            }

            Console.WriteLine(CoderTestsConstants.MAREA2);
            Results results;
            if (oHashtable == null)
                results = ResultsManager.GetResults(serializeTicks, deserializeTicks, clock_freq, CoderTestsConstants.CODIFICATIONS, seralizedData.Length, typeof(Hashtable).FullName + "(null)");
            else
                results = ResultsManager.GetResults(serializeTicks, deserializeTicks, clock_freq, CoderTestsConstants.CODIFICATIONS, seralizedData.Length, oHashtable.GetType().FullName);
            

            if (oHashtable == rHashtable)
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
