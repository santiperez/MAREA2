using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

using Marea;

namespace Altimeter
{
    public class Altimeter : Service, IAltimeter
    {
        protected bool running = false;
        protected Thread th;
        protected int value = 0;

        public override bool Start()
        {
            running = true;
            th = new Thread(Run);
            th.Start();
            return true;
        }

        public override bool Stop()
        {
            running = false;
            return true;
        }

        public void Run()
        {
            while (running)
            {
                altitude.Notify(id, value);
                value++;
                Thread.Sleep(1000);
            }
        }

        public Variable<int> altitude
        {
            get;
            private set;
        }

        public void Reset(bool b)
        {
            Console.WriteLine("Reseting Altimeter...");
            if (b) value = 0;
        }
    }
}
