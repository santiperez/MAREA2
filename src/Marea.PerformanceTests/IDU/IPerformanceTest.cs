using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Marea;


namespace PerformanceTests
{
    [ServiceDefinition("This service represents a Test")]
    public interface IPerformanceTest
    {
        [Description("Variable to send a Packet<byte> request")]
        [Unit("None")]
        [Publish]
        Variable<Packet<byte>> v_packetSent { get; }

        [Description("Variable to send a Packet<byte> request")]
        [Unit("None")]
        [Publish]
        Event<Packet<byte>> e_packetSent { get; }
    }
}
