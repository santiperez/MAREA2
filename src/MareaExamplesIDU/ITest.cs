using System;
using Marea;

namespace Examples
{
    [ServiceDefinition("This service represents a Termometer")]
    public interface ITest
    {
        [Description("Variable")]
        [Unit("")]
        [Publish]
        Variable<int> Variable{ get; }

        [Description("Event")]
        [Unit("")]
        [Publish]
        Event<int> Event{ get; }
    }
}
