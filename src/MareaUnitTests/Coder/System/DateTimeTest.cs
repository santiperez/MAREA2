using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Globalization;
using NUnit.Framework;
using Marea;


namespace MareaUnitTests.Coder.System
{
    [TestFixture]
    class DateTimeTest
    {
        private byte[] seralizedData = null;
        private long clock_freq, start, serializeTicks, deserializeTicks;

        public DateTimeTest()
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
        }

        [TestCase, NUnit.Framework.Description("Coder(System.DateTime)")]
        public void TestDateTimeM2()
        {
            DateTime oDateTime = DateTime.Now;
            string dateString = "Mon 16 Jun 8:30 AM 2008";
            string format = "ddd dd MMM h:mm tt yyyy";
            DateTime rDateTime = DateTime.ParseExact(dateString, format, CultureInfo.InvariantCulture);

            for (int i = 0; i < CoderTestsConstants.CODIFICATIONS; i++)
            {
                start = PerformanceTimer.Ticks();
                seralizedData = AdaptedMareaCoder.Send(oDateTime);
                serializeTicks += PerformanceTimer.TicksDifference(start);

                start = PerformanceTimer.Ticks();
                rDateTime = (DateTime)AdaptedMareaCoder.Receive(seralizedData);
                deserializeTicks += PerformanceTimer.TicksDifference(start);
            }

            Console.WriteLine(CoderTestsConstants.MAREA2);
            Results results = ResultsManager.GetResults(serializeTicks, deserializeTicks, clock_freq, CoderTestsConstants.CODIFICATIONS, seralizedData.Length, rDateTime.GetType().FullName);
            
            if (oDateTime == rDateTime)
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
