using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Marea;
using NUnit.Framework;

namespace MareaUnitTests.Coder.System.Collections.Generics
{
    [TestFixture]
    class DictionaryTest
    {

        private byte[] seralizedData = null;
        private long clock_freq, start, serializeTicks, deserializeTicks;
        private const int length=120;
        private Dictionary<int, char> oDictionary;
        private Dictionary<int, char> rDictionary;

        public DictionaryTest()
        {
            Init();
        }

        public void Init()
        {
            clock_freq = PerformanceTimer.Clock_freq();
            oDictionary = new  Dictionary<int, char>();

            for (int i = 0; i < length; i++)
                oDictionary.Add(i,Convert.ToChar((byte)i));
        }

        [SetUp]
        public void RunAfterAnyTest()
        {
            serializeTicks = 0;
            deserializeTicks = 0;

            rDictionary = new Dictionary<int, char>();
        }

        [TestCase, NUnit.Framework.Description("Coder(System.Collections.Dictionary<int,char>)")]
        public void TestDictionaryIntCharM2()
        {
            for (int i = 0; i < CoderTestsConstants.CODIFICATIONS; i++)
            {
                start = PerformanceTimer.Ticks();
                seralizedData = AdaptedMareaCoder.Send(oDictionary);
                serializeTicks += PerformanceTimer.TicksDifference(start);

                start = PerformanceTimer.Ticks();
                rDictionary = (Dictionary<int, char>)AdaptedMareaCoder.Receive(seralizedData);
                deserializeTicks += PerformanceTimer.TicksDifference(start);
            }

            Console.WriteLine(CoderTestsConstants.MAREA2);
            Results results = ResultsManager.GetResults(serializeTicks, deserializeTicks, clock_freq, CoderTestsConstants.CODIFICATIONS, seralizedData.Length, oDictionary.GetType().FullName);

            if (oDictionary[0].Equals(rDictionary[0]) && oDictionary[oDictionary.Count / 2].Equals(rDictionary[oDictionary.Count / 2]) && oDictionary[oDictionary.Count - 1].Equals(rDictionary[oDictionary.Count - 1]))
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
