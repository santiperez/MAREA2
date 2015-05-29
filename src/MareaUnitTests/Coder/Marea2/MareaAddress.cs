using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using Marea;

namespace MareaUnitTests.Coder.Marea2
{
	/**
    * C# Type->bool
    * .NET Framework Type->System.Boolean
    **/
	[TestFixture]
	class MareaAddressTest
	{
		private byte[] serialized = null;
		private long clock_freq, start, serializeTicks, deserializeTicks;

		public MareaAddressTest()
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

		[TestCase(), NUnit.Framework.Description("Coder(Marea.MareaAddress)")]
		public void TestMareaAddressM2()
		{
			MareaAddress oMad = new MareaAddress("/EC-UPC/127.0.0.1:1234/0/Examples.Battery");
			MareaAddress rMad = null;

			for (int i = 0; i < CoderTestsConstants.CODIFICATIONS; i++)
			{
				start = PerformanceTimer.Ticks();
				serialized = AdaptedMareaCoder.Send(oMad);
				serializeTicks += PerformanceTimer.TicksDifference(start);

				start = PerformanceTimer.Ticks();
				rMad = (MareaAddress)AdaptedMareaCoder.Receive(serialized);
				deserializeTicks += PerformanceTimer.TicksDifference(start);
			}

			Console.WriteLine(CoderTestsConstants.MAREA2);
			Results results = ResultsManager.GetResults(serializeTicks, deserializeTicks, clock_freq, CoderTestsConstants.CODIFICATIONS, serialized.Length, oMad.GetType().FullName);

			if (oMad.Equals(rMad))
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
