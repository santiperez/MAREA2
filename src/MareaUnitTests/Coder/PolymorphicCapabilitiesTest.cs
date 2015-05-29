using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;

using Marea;

namespace MareaUnitTests.Coder
{
    [TestFixture]
    class PolymorphicCapabilitiesTest
    {
        private byte[] seralizedData = null;
        private long clock_freq, start, serializeTicks, deserializeTicks;
        private PA oPA, rPA;
        private QA qA;
        private QC oQC;

        private PB oPB,rPB;
        private QB qB;
        
        public PolymorphicCapabilitiesTest()
        {
            Init();
        }

        public void Init()
        {
            clock_freq = PerformanceTimer.Clock_freq();
            oPA = new PA();
            oPA.x = 65;
            oPA.y = 'x';

            qA = new QA();
            qA.a = 'a';
            qA.b = 'b';

            oQC = new QC();
            oQC.a = 'l';
            oQC.b = 'm';
            oQC.c = 'n';

            oPB = new PB();
            oPB.x = 132;
            oPB.y='d';

            qB= new QB();
            qB.a='t';
            qB.b='u';

            oPB.z = qB;

        }

        [SetUp]
        public void RunAfterAnyTest()
        {
            serializeTicks = 0;
            deserializeTicks = 0;

            rPA = null;
            rPB = null;
        }

        [TestCase, NUnit.Framework.Description("Coder(PA[z=null])")]
        public void TestClassPAzNullM2()
        {
            oPA.z = null;

            for (int i = 0; i < CoderTestsConstants.CODIFICATIONS; i++)
            {
                start = PerformanceTimer.Ticks();
                seralizedData = AdaptedMareaCoder.Send(oPA);
                serializeTicks += PerformanceTimer.TicksDifference(start);

                start = PerformanceTimer.Ticks();
                rPA = (PA)AdaptedMareaCoder.Receive(seralizedData);
                deserializeTicks += PerformanceTimer.TicksDifference(start);
            }

            Console.WriteLine(CoderTestsConstants.MAREA2);
            Results results = ResultsManager.GetResults(serializeTicks, deserializeTicks, clock_freq, CoderTestsConstants.CODIFICATIONS, seralizedData.Length, oPA.GetType().FullName);

            if (oPA.x == rPA.x && oPA.y == rPA.y && rPA.z==null && oPA.z==null)
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

        [TestCase, NUnit.Framework.Description("Coder(PA[z=QA])")]
        public void TestClassPAQAM2()
        {
            oPA.z = qA;

            for (int i = 0; i < CoderTestsConstants.CODIFICATIONS; i++)
            {
                start = PerformanceTimer.Ticks();
                seralizedData = AdaptedMareaCoder.Send(oPA);
                serializeTicks += PerformanceTimer.TicksDifference(start);

                start = PerformanceTimer.Ticks();
                rPA = (PA)AdaptedMareaCoder.Receive(seralizedData);
                deserializeTicks += PerformanceTimer.TicksDifference(start);
            }

            Console.WriteLine(CoderTestsConstants.MAREA2);
            Results results = ResultsManager.GetResults(serializeTicks, deserializeTicks, clock_freq, CoderTestsConstants.CODIFICATIONS, seralizedData.Length, oPA.GetType().FullName);

            if (oPA.x == rPA.x && oPA.y == rPA.y && rPA.z.a == oPA.z.a && rPA.z.b == oPA.z.b)
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

        [TestCase, NUnit.Framework.Description("Coder(QC[inheritance])")]
        public void TestClassQCInheritanceM2()
        {
            oPA.z = oQC;

            for (int i = 0; i < CoderTestsConstants.CODIFICATIONS; i++)
            {
                start = PerformanceTimer.Ticks();
                seralizedData = AdaptedMareaCoder.Send(oPA);
                serializeTicks += PerformanceTimer.TicksDifference(start);

                start = PerformanceTimer.Ticks();
                rPA = (PA)AdaptedMareaCoder.Receive(seralizedData);
                deserializeTicks += PerformanceTimer.TicksDifference(start);
            }

            Console.WriteLine(CoderTestsConstants.MAREA2);
            Results results = ResultsManager.GetResults(serializeTicks, deserializeTicks, clock_freq, CoderTestsConstants.CODIFICATIONS, seralizedData.Length, oPA.GetType().FullName);
            
            //Check inheritance (last condition)
            if (oPA.x == rPA.x && oPA.y == rPA.y && rPA.z.a == oPA.z.a && rPA.z.b == oPA.z.b && ((QC)rPA.z).c==oQC.c)
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

        [TestCase, NUnit.Framework.Description("Coder(QB[struct])")]
        public void TestStructQBM2()
        {
            oPB.z = qB;

            for (int i = 0; i < CoderTestsConstants.CODIFICATIONS; i++)
            {
                start = PerformanceTimer.Ticks();
                seralizedData = AdaptedMareaCoder.Send(oPB);
                serializeTicks += PerformanceTimer.TicksDifference(start);

                start = PerformanceTimer.Ticks();
                rPB = (PB)AdaptedMareaCoder.Receive(seralizedData);
                deserializeTicks += PerformanceTimer.TicksDifference(start);
            }

            Console.WriteLine(CoderTestsConstants.MAREA2);
            Results results = ResultsManager.GetResults(serializeTicks, deserializeTicks, clock_freq, CoderTestsConstants.CODIFICATIONS, seralizedData.Length, oPA.GetType().FullName);

            if (oPB.x == rPB.x && oPB.y == rPB.y && rPB.z.a == oPB.z.a && rPB.z.b == oPB.z.b)
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
