using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Marea;
using NUnit.Framework;
using System.Globalization;

namespace MareaUnitTests.Coder.System.Array
{
    [TestFixture]
    class DateTimeArrayTest
    {
        private byte[] seralizedData = null;
        private long clock_freq, start, serializeTicks, deserializeTicks;
        private DateTime[] oDateTimeArray;
        private DateTime[] rDateTimeArray;
        private const int length = 256;

        public DateTimeArrayTest()
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

            oDateTimeArray= new DateTime[length];
            rDateTimeArray = new DateTime[length];

            string dateString = "Mon 16 Jun 8:30 AM 2008";
            string format = "ddd dd MMM h:mm tt yyyy";
            DateTime dateTime = DateTime.ParseExact(dateString, format, CultureInfo.InvariantCulture);

            int i;
            for (i = 0; i < length; i++)
            {
                oDateTimeArray[i] = DateTime.Now;
                rDateTimeArray[i] = dateTime;
            }
        }

        [TestCase, NUnit.Framework.Description("Coder(System.DateTime[])")]
        public void TestDateTimeArrayM2()
        {
            for (int i = 0; i < CoderTestsConstants.CODIFICATIONS; i++)
            {
                start = PerformanceTimer.Ticks();
                seralizedData = AdaptedMareaCoder.Send(oDateTimeArray);
                serializeTicks += PerformanceTimer.TicksDifference(start);

                start = PerformanceTimer.Ticks();
                rDateTimeArray = (DateTime[])AdaptedMareaCoder.Receive(seralizedData);
                deserializeTicks += PerformanceTimer.TicksDifference(start);
            }

            Console.WriteLine(CoderTestsConstants.MAREA2);
            Results results = ResultsManager.GetResults(serializeTicks, deserializeTicks, clock_freq, CoderTestsConstants.CODIFICATIONS, seralizedData.Length, oDateTimeArray.GetType().FullName);

            if (oDateTimeArray[0] == rDateTimeArray[0] && oDateTimeArray[length / 2] == rDateTimeArray[length / 2] && oDateTimeArray[length-1] == rDateTimeArray[length-1]) 
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

        [TestCase, NUnit.Framework.Description("Coder(System.DateTime[])[null]")]
        public void TestDateTimeArrayM2Null()
        {
            oDateTimeArray = null;

            for (int i = 0; i < CoderTestsConstants.CODIFICATIONS; i++)
            {
                start = PerformanceTimer.Ticks();
                seralizedData = AdaptedMareaCoder.Send(oDateTimeArray);
                serializeTicks += PerformanceTimer.TicksDifference(start);

                start = PerformanceTimer.Ticks();
                rDateTimeArray = (DateTime[])AdaptedMareaCoder.Receive(seralizedData);
                deserializeTicks += PerformanceTimer.TicksDifference(start);
            }

            Console.WriteLine(CoderTestsConstants.MAREA2);
            Results results;
            if (oDateTimeArray == null)
                results = ResultsManager.GetResults(serializeTicks, deserializeTicks, clock_freq, CoderTestsConstants.CODIFICATIONS, seralizedData.Length, typeof(DateTime[]).FullName + "(null)");
            else
                results = ResultsManager.GetResults(serializeTicks, deserializeTicks, clock_freq, CoderTestsConstants.CODIFICATIONS, seralizedData.Length, oDateTimeArray.GetType().FullName);
            

            if (oDateTimeArray == rDateTimeArray)
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
