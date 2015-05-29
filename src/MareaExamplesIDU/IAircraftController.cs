using Marea;
using System;

namespace Examples
{
	[Serializable]
	public struct Response {
		public int SecondsFromTestBeginning; 
		public int Value;      // 1..5 (0 if not respond)
		public long Timeout;  // ms that needs to answer
	}

	[ServiceDefinition("This service represents an aircraft controller that is under stress test.")]
	public interface IAircraftController
	{
        [Description("...")]
        [Publish]
		void SendResponse(int Value); // dispara el evento Response con el timeout y el valor que toca

        [Description("...")]
        [Publish]
		Event<None> Beep { get; }

        [Description("...")]
        [Publish]
		Event<Response> Response { get; } 
	}
}