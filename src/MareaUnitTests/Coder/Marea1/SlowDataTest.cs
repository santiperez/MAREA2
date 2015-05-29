using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Marea;
using NUnit.Framework;

namespace MareaUnitTests.Coder.Marea1
{
    [TestFixture]
    class DataTest
    {
        private byte[] serializedData = null;
        private long clock_freq, start, serializeTicks, deserializeTicks;
        private const int length=250;
        private DataMarea1 oData;
        private DataMarea1 rData;
        private Packet<byte> packetGeneric;

        public DataTest()
        {
            Init();
        }

        public void Init()
        {
			clock_freq = Stopwatch.Frequency;

            this.packetGeneric = new Packet<byte>();
            this.packetGeneric.data= new byte[length];
            for (int i = 0; i < length; i++)
                this.packetGeneric.data[i] = (byte)i;
            this.packetGeneric.time = PerformanceTimer.Ticks();
            this.packetGeneric.id = 1;
            this.packetGeneric.number = 2;

            this.oData = new DataMarea1("name", packetGeneric, Marea1Primitive.Variable);
            
        }

        [SetUp]
        public void RunAfterAnyTest()
        {
            serializeTicks = 0;
            deserializeTicks = 0;
            rData = new DataMarea1();

        }

        [TestCase, NUnit.Framework.Description("Coder(Marea.Data)")]
        public void TestMareaDataM2()
        {
            for (int i = 0; i < CoderTestsConstants.CODIFICATIONS; i++)
            {
                start = Stopwatch.GetTimestamp();
                serializedData = AdaptedMareaCoder.Send(oData);
                serializeTicks += PerformanceTimer.TicksDifference(start);

                start = PerformanceTimer.Ticks();
                rData = (DataMarea1)AdaptedMareaCoder.Receive(serializedData);
                deserializeTicks += PerformanceTimer.TicksDifference(start);
            }

            Console.WriteLine(CoderTestsConstants.MAREA2);
            Results results = ResultsManager.GetResults(serializeTicks, deserializeTicks, clock_freq, CoderTestsConstants.CODIFICATIONS, serializedData.Length, oData.GetType().FullName);

            Packet<byte> oP = (Packet<byte>)oData.data;
            Packet<byte> rP = (Packet<byte>)rData.data;

            if (oP.data[length / 2] == rP.data[length / 2] && oP.id==rP.id && oP.number==rP.number  && oData.name == rData.name && rData.primitive == oData.primitive)
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
