using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Marea;

namespace PerformanceTests
{
    [ServiceDefinition("This service represents a TestMonitor")]
    public interface IPerformanceTestMonitor
    {
        [Description("Variable to send a Packet<byte> response")]
        [Unit("None")]
        [Publish]
        Variable<Packet<byte>> v_packetReceived { get; }

        [Description("Variable to send a Packet<byte> response")]
        [Unit("None")]
        [Publish]
        Event<Packet<byte>> e_packetReceived { get; }
    }
}
