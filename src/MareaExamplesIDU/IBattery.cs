using Marea;

namespace Examples
{

    [ServiceDefinition("This service represents a battery")]
    public interface IBattery
    {
        [Description("...")]
        [Unit("V")]
        [Publish]
        Variable<double> voltage { get; }

        [Description("...")]
        [Unit("A")]
        [Publish]
        Variable<double> amperage { get; }

        [Description("...")]
        [Unit("º C")]
        [Publish]
        Event<double> temperature { get; }

        [Description("...")]
        [Publish]
        Event<None> lowBattery { get; }

        [Description("...")]
        [Publish]
        Event<bool> disconnected { get; }

        void Recharge(bool b);
        void RechargeExtension(bool b,char c);
    }
}
