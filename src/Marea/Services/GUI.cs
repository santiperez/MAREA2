#if !__MonoCS__

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using log4net;

namespace Marea
{
	//TODO mmm... Santi, este LocateService para que esta?

    [LocateService("Marea.GUI")]
    public class GUI : Service
    {
        private static readonly ILog log = LogManager.GetLogger("Marea.Services.GUI");

        public override bool Start()
        {
            log.Info("NodeManager service started");

            Thread t = new Thread(new ThreadStart(StartCLIGUI));
            t.SetApartmentState(ApartmentState.STA);
            t.Start();
            return true;
        }

        //Now it's only a GUI
        public void StartCLIGUI()
        {
            Application app = new Application();
            try
            {

                app.Run(new MareaGui((ServiceContainer)container));
            }
            catch (InvalidOperationException) { }
        }
    }
}

#endif
