using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Marea;

namespace Altimeter
{
    public class Receiver : Service, IReceiver
    {
        [LocateService("*/*/*/*/Altimeter.Altimeter")]
        private IAltimeter alt;

        public override bool Start()
        {
            alt.altitude.Subscribe(id, ProcessAltitude);
            return true;
        }

        public void ProcessAltitude(String source, int altitude)
        {
            Console.WriteLine("{0} => {1}", source, altitude);
            
            /*if (altitude == 10)
            {
                source = "/EC-UPC/10.89.129.111:11000/0/Altimeter.Altimeter";
                //TODO IAltimeter s = container.GetService<Altimeter>(source.GetServiceAddress());
                s.Reset(true);
            }
            */
            
        }

        public override bool Stop()
        {
            alt.altitude.Unsubscribe(id, ProcessAltitude);
            return true;
        }

    }
}
