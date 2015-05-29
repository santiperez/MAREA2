using Marea;
using System;

namespace Examples
{

	[Serializable]
	public struct Status {
		public bool beginOrEnd;
		public int intervalBetweenResults;
	} 

	[ServiceDefinition("This service represents a central server that manages a test and stores the results.")]
	public interface ICentralServer
	{
		void BeginTest(int intervalBetweenResults);
		void EndTest();

        [Description("...")]
        [Publish]
		Event<Status> TestStatus { get; }
	}
}
