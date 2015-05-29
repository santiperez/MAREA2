using System;
using System.Collections.Generic;
using System.Threading;
using Marea;

namespace Examples
{
    /// <summary>
    /// This is the user implementation of the BatteryManager service.
    /// </summary>
    class BatteryManager : Service, IBatteryManager
    {
        [LocateService("*/*/*/*/Examples.Battery")]
        private IBattery bat;
        
        public BatteryManager()
        {
            bat = new Battery();
        }

        public override bool Start()
        {
            if (bat!= null)
            {
                bat.amperage.Subscribe(id, GetAmperage);
                bat.disconnected.Subscribe(id, GetState);
                bat.temperature.Subscribe(id, GetTemperature);
                bat.lowBattery.Subscribe(id, GetLowBattery);
                bat.voltage.Subscribe(id,GetVoltage);
                return true;
            }
            return false;
        }

        public void GetAmperage(String name, double amps) 
        {
            //Console.WriteLine();
            //Console.WriteLine("["+this.id+"]");
            //Console.WriteLine("\tPrimitive: "+name);
            //Console.WriteLine("\tValue: " +amps);
        }

        public void GetState(String name, bool state)
        {
            //Console.WriteLine();
            //Console.WriteLine("[" + this.id + "]");
            //Console.WriteLine("\tPrimitive: " + name);
            //Console.WriteLine("\tValue: " + state);
        }

        public void GetTemperature(String name, double temp)
        {
            //Console.WriteLine();
            //Console.WriteLine("[" + this.id + "]");
            //Console.WriteLine("\tPrimitive: " + name);
            //Console.WriteLine("\tValue: " + temp);
        }

        public void GetVoltage(String name, double volt)
        {
            Console.WriteLine();
            Console.WriteLine("[" + this.id + "]");
            Console.WriteLine("\tPrimitive: " + name);
            Console.WriteLine("\tValue: " + volt);
        }

        public void GetLowBattery(String name, None none)
        {
            //Console.WriteLine();
            //Console.WriteLine("[" + this.id + "]");
            //Console.WriteLine("\tPrimitive: " + name);
            //Console.WriteLine("\tValue: " +none);
        }

        public override bool Stop()
        {
            if (bat != null)
            {
                if (bat.amperage != null)
                    bat.amperage.Unsubscribe(id, this.GetAmperage);
                if (bat.disconnected != null)
                    bat.disconnected.Unsubscribe(id, this.GetState);
                if (bat.temperature != null)
                    bat.temperature.Unsubscribe(id, this.GetTemperature);
                if (bat.lowBattery!= null)
                    bat.lowBattery.Unsubscribe(id, GetLowBattery);
                if (bat.voltage != null)
                    bat.voltage.Unsubscribe(id, GetVoltage);
            }

            return true;

        }

        #region IBatteryManager Members

        public Event<string> globalBatteryWarning { get; private set; }
        
        #endregion
    }

    ///// <summary>
    ///// This is the santi-generated Battery proxy for the producers.
    ///// </summary>
    //class BatteryManagerProxy : IBatteryManager
    //{
    //    IServiceContainer container;
    //    ServiceAddress serviceAddress;

    //    BatteryManagerProxy(IServiceContainer container, ServiceAddress serviceAddress)
    //    {
    //        this.container = container;
    //        this.serviceAddress = serviceAddress;
    //        this.globalBatteryWarning = container.GetPrimitiveFromService<Event<string>>((IService)this, "globalBatteryWarning");
    //    }

    //    #region IBatteryManager Members

    //    public Event<string> globalBatteryWarning { get; private set; }

    //    #endregion

    //}
}
