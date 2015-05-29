using System;
using System.Collections.Generic;
using System.Threading;
using Marea;

namespace Examples
{
    /// <summary>
    /// This is the user implementation of the Battery service.
    /// </summary>
    public class Battery : Service, IBattery
    {
        protected Thread th;
        protected bool isStarted = false;

        public override bool Start()
        {
            th = new Thread(new ThreadStart(Run));
            isStarted = true;
            th.Start();
            return true;
        }

        protected void Run()
        {
            Random r = new Random();
            bool state = false;

            while (isStarted)
            {
                Thread.Sleep(3000);
                voltage.Notify(id, r.NextDouble());
                amperage.Notify(id, r.NextDouble());
                temperature.Notify(id, r.NextDouble());

                if (state)
                    state = false;
                else
                    state = true;
                disconnected.Notify(id, state);

                if (amperage.Value < 0.5)
                {
                    lowBattery.Notify(id, None.Instance);
                }

            }
        }

        public override bool Stop()
        {
            isStarted = false;
            return true;
        }

        public void Recharge(bool t)
        {
            //DO SOMETHING
        }
        public void RechargeExtension(bool t, char c)
        {
            //DO SOMETHING
        }


        #region IBattery Members

        public Variable<double> voltage { get; private set; }

        public Variable<double> amperage { get; private set; }

        public Event<None> lowBattery { get; private set; }

        public Event<bool> disconnected { get; private set; }

        public Event<double> temperature { get; private set; }

        #endregion
    }

    ///// <summary>
    ///// This is the santi-generated Battery proxy for the consumers.
    ///// </summary>
    //class BatteryProxy : RemoteProducer, IBattery
    //{
    //    public BatteryProxy(IServiceContainer container, ServiceAddress serviceAddress, TransportAddress control)
    //        : base(control)
    //    {
    //        this.container = container;
    //        this.id = serviceAddress;
    //    }

    //    #region IBattery Members

    //    private Variable<double> _voltage;
    //    public Variable<double> voltage
    //    {
    //        get
    //        {
    //            if (_voltage == null)
    //            {
    //                _voltage = container.CreatePrimitive<Variable<double>>(id, "voltage");
    //                //(VariableImp)_voltage.addSubscriber+=AddSubscriber);
    //                //(VariableImp)_voltage.removeSubscriber+=RemoveSubscriber);
    //            }
    //            return _voltage;
    //        }
    //    }

    //    private Variable<double> _amperage;
    //    public Variable<double> amperage
    //    {
    //        get
    //        {
    //            if (_amperage == null)
    //            {
    //                _amperage = container.CreatePrimitive<Variable<double>>(id, "amperage");
    //                //(VariableImp)_voltage.addSubscriber+=AddSubscriber);
    //                //(VariableImp)_voltage.removeSubscriber+=RemoveSubscriber);
    //            }
    //            return _amperage;
    //        }
    //    }

    //    public Event<None> _lowBattery;
    //    public Event<None> lowBattery
    //    {
    //        get
    //        {
    //            if (_lowBattery == null)
    //            {
    //                _lowBattery = container.CreatePrimitive<Event<None>>(id, "lowBattery");
    //                //(VariableImp)_voltage.addSubscriber+=AddSubscriber);
    //                //(VariableImp)_voltage.removeSubscriber+=RemoveSubscriber);
    //            }
    //            return _lowBattery;
    //        }
    //    }

    //    public Event<bool> _disconnected;
    //    public Event<bool> disconnected
    //    {
    //        get
    //        {
    //            if (_disconnected == null)
    //            {
    //                _disconnected = container.CreatePrimitive<Event<bool>>(id, "disconnected");
    //                //(VariableImp)_voltage.addSubscriber+=AddSubscriber);
    //                //(VariableImp)_voltage.removeSubscriber+=RemoveSubscriber);
    //            }
    //            return _disconnected;
    //        }
    //    }


    //    private Event<double> _temperature;
    //    public Event<double> temperature
    //    {
    //        get
    //        {
    //            if (_temperature == null)
    //            {
    //                _temperature = container.CreatePrimitive<Event<double>>(id, "temperature");
    //                //(VariableImp)_voltage.addSubscriber+=AddSubscriber);
    //                //(VariableImp)_voltage.removeSubscriber+=RemoveSubscriber);
    //            }
    //            return _temperature;
    //        }
    //    }

    //    public void Recharge(bool b)
    //    {
    //        //container.CallFunction("//Battery.Recharge", b);
    //    }


    //    //TODO Esto molaria que estuviera en algun sitio (clase padre) para no tenerlo que poner en todos
    //    //     los proxys, aunque los generes con el ProxyGen
    //    public void AddSubscriber<T>(Variable<T> variable, bool isAdding, ServiceAddress id, NotifyFunc<T> notifyFunc, int totalSubscribers)
    //    {

    //        if (totalSubscribers == 1)
    //        {
    //            //TODO Lllamar pub/sub y hacer el subscribe por la red
    //        }
    //    }


    //    public void RemoveSubscriber<T>(Variable<T> variable, bool isAdding, ServiceAddress id, NotifyFunc<T> notifyFunc, int totalSubscribers)
    //    {

    //        if (totalSubscribers == 0)
    //        {
    //            //TODO Lllamar pub/sub y hacer el unsubscribe por la red
    //        }
    //    }
    //    #endregion

    //    public void RechargeExtension(bool t, char c)
    //    {
    //        //DO SOMETHING
    //    }
    //}

    /// <summary>
    /// This is the santi-generated Battery query for the consumers.
    /// </summary>
    //class BatteryQuery : QueryService, IBattery
    //{
    //    public BatteryQuery(IServiceContainer container, ServiceAddress serviceAddress) : base(container, serviceAddress) { }

    //    public override void AddMatchingService(ServiceAddress serviceAddress, IService service)
    //    {
    //        IBattery battery = (IBattery)service;

    //        if (_amperage != null)
    //        {
    //            if (_amperage.GetTotalSubscriptions() > 0)
    //                //if (battery.amperage.GetTotalSubscriptions() == 0) 
    //                battery.amperage.Subscribe(id, this.FireAmperage);
    //        }

    //        if (_disconnected != null)
    //        {
    //            if (_disconnected.GetTotalSubscriptions() > 0)
    //                //if (battery.disconnected.GetTotalSubscriptions() == 0)
    //                battery.disconnected.Subscribe(id, this.FireDisconnected);
    //        }

    //        if (_lowBattery != null)
    //        {
    //            if (_lowBattery.GetTotalSubscriptions() > 0)
    //                //if (battery.lowBattery.GetTotalSubscriptions() == 0)
    //                battery.lowBattery.Subscribe(id, this.FireLowBattery);
    //        }

    //        if (_temperature != null)
    //        {
    //            if (_temperature.GetTotalSubscriptions() > 0)
    //                //if (battery.temperature.GetTotalSubscriptions() == 0)
    //                battery.temperature.Subscribe(id, this.FireTemperature);
    //        }

    //        if (_voltage != null)
    //        {
    //            if (_voltage.GetTotalSubscriptions() > 0)
    //                //if (battery.voltage.GetTotalSubscriptions() == 0)
    //                battery.voltage.Subscribe(id, this.FireVoltage);
    //        }

    //        AddMatchingServiceAddress(serviceAddress);
    //    }

    //    public override void RemoveMatchingService(ServiceAddress serviceAddress, IService service)
    //    {
    //        IBattery battery = (IBattery)service;

    //        if (_amperage.GetTotalSubscriptions() == 0)
    //            //if (battery.amperage.GetTotalSubscriptions() == 1)
    //            battery.amperage.Unsubscribe(id, this.FireAmperage);

    //        if (_disconnected.GetTotalSubscriptions() == 0)
    //            //if (battery.disconnected.GetTotalSubscriptions() == 1)
    //            battery.disconnected.Unsubscribe(id, this.FireDisconnected);

    //        if (_lowBattery.GetTotalSubscriptions() == 0)
    //            //if (battery.lowBattery.GetTotalSubscriptions() == 1)
    //            battery.lowBattery.Unsubscribe(id, this.FireLowBattery);

    //        if (_temperature.GetTotalSubscriptions() == 0)
    //            //if (battery.temperature.GetTotalSubscriptions() == 1)
    //            battery.temperature.Unsubscribe(id, this.FireTemperature);

    //        if (_voltage.GetTotalSubscriptions() == 0)
    //            //if (battery.voltage.GetTotalSubscriptions() == 1)
    //            battery.voltage.Unsubscribe(id, this.FireVoltage);

    //        //if (amperage.GetTotalSubscriptions() == 0 && disconnected.GetTotalSubscriptions() == 0 && lowBattery.GetTotalSubscriptions() == 0 && temperature.GetTotalSubscriptions() == 0 && voltage.GetTotalSubscriptions() == 0)


    //        int n = 5;
    //        int[] totalSubscriptions = new int[n];
    //        totalSubscriptions[0] = _amperage.GetTotalSubscriptions();
    //        totalSubscriptions[1] = _disconnected.GetTotalSubscriptions();
    //        totalSubscriptions[2] = _lowBattery.GetTotalSubscriptions();
    //        totalSubscriptions[3] = _temperature.GetTotalSubscriptions();
    //        totalSubscriptions[4] = _voltage.GetTotalSubscriptions();

    //        while (--n > 0 && totalSubscriptions[n] == totalSubscriptions[0]) ;

    //        if (n == 0)
    //            RemoveMatchingServiceAddress(serviceAddress);
    //    }

    //    public void FireAmperage(string name, double amps)
    //    {
    //        _amperage.Notify(new ServiceAddress(name), amps);
    //    }

    //    public void FireDisconnected(string name, bool isDisconnected)
    //    {
    //        _disconnected.Notify(new ServiceAddress(name), isDisconnected);
    //    }

    //    public void FireLowBattery(string name, None lowBattery)
    //    {
    //        this._lowBattery.Notify(new ServiceAddress(name), lowBattery);
    //    }

    //    public void FireTemperature(string name, double temperature)
    //    {
    //        _temperature.Notify(new ServiceAddress(name), temperature);
    //    }

    //    public void FireVoltage(string name, double volts)
    //    {
    //        _voltage.Notify(new ServiceAddress(name), volts);
    //    }

    //    #region IBattery Members

    //    private Variable<double> _voltage;
    //    public Variable<double> voltage
    //    {
    //        get
    //        {
    //            if (_voltage == null)
    //            {
    //                _voltage = container.CreatePrimitive<Variable<double>>(id, "voltage");
    //                ((Primitive)_voltage).AddSubscriber(AddOrRemoveSubscriber);
    //            }
    //            return _voltage;
    //        }
    //    }

    //    private Variable<double> _amperage;
    //    public Variable<double> amperage
    //    {
    //        get
    //        {
    //            if (_amperage == null)
    //            {
    //                _amperage = container.CreatePrimitive<Variable<double>>(id, "amperage");
    //                ((Primitive)_amperage).AddSubscriber(AddOrRemoveSubscriber);
    //            }
    //            return _amperage;
    //        }
    //    }

    //    public Event<None> _lowBattery;
    //    public Event<None> lowBattery
    //    {
    //        get
    //        {
    //            if (_lowBattery == null)
    //            {
    //                _lowBattery = container.CreatePrimitive<Event<None>>(id, "lowBattery");
    //                ((Primitive)_lowBattery).AddSubscriber(AddOrRemoveSubscriber);
    //            }
    //            return _lowBattery;
    //        }
    //    }

    //    public Event<bool> _disconnected;
    //    public Event<bool> disconnected
    //    {
    //        get
    //        {
    //            if (_disconnected == null)
    //            {
    //                _disconnected = container.CreatePrimitive<Event<bool>>(id, "disconnected");
    //                ((Primitive)_disconnected).AddSubscriber(AddOrRemoveSubscriber);
    //            }
    //            return _disconnected;
    //        }
    //    }


    //    private Event<double> _temperature;
    //    public Event<double> temperature
    //    {
    //        get
    //        {
    //            if (_temperature == null)
    //            {
    //                _temperature = container.CreatePrimitive<Event<double>>(id, "temperature");
    //                ((Primitive)_temperature).AddSubscriber(AddOrRemoveSubscriber);
    //            }
    //            return _temperature;
    //        }
    //    }

    //    public void Recharge(bool b)
    //    {
    //        //container.CallFunction("//Battery.Recharge", b);
    //    }

    //    public void RechargeExtension(bool t, char c)
    //    {
    //        //DO SOMETHING
    //    }
    //    #endregion
    //}
}


