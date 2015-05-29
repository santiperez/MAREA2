using System;

namespace Marea
{
	//TODO How the authentication will be done?

    [ServiceDefinition("This is a Node manager")]
	public interface INodeManager
	{
        [Description("...")]
        [Publish]
        int Add(int a, int b);

        [Description("...")]
        [Publish]
		MareaAddress StartService(String serviceName);
        //MareaAddress StartService(String serviceName, String identifier, String subsystem);
        [Description("...")]
        [Publish]
		bool StopService(MareaAddress address);

        [Description("...")]
        [Publish]
		String[] GetAvailableServices();
        [Description("...")]
        [Publish]
		MareaAddress[] GetRunningServices();
        [Description("...")]
        [Publish]
		MareaAddress[] GetKnownServices();

		void Shutdown();
	}
}

