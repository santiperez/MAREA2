using System;
using Marea;

namespace Examples
{

    [ServiceDefinition("This is a battery manager")]
    public interface IBatteryManager
    {
        [Description("Warns when any of the batteries of the system is low.")]
        Event<String> globalBatteryWarning {get;}
    }
}