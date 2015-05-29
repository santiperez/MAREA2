using Marea;
using System;
using System.IO;
using System.Threading;
using System.Reflection;
using Marea.Terminal;

namespace Marea
{
    /**
     * SDU
     */
    public class Console : Service, IConsole
    {

        [LocateService("*/*/*/*/Marea.NodeManager")]
        private INodeManager node=null;

        protected bool running = false;

        protected LineEditor le = new LineEditor(null);

        public override bool Start()
        {
            running = true;
            Thread th = new Thread(this.Run);
            th.Start();
            return true;
        }

        public override bool Stop()
        {
            running = false;
            le.InterruptEdit();
            return true;
        }

        public void Run()
        {

            while (running)
            {
                //Read command
                String command = le.Edit("Marea.Console> ", "");

                //Process command
                if (command != null && !command.Equals(""))
                {
                    String[] args = command.Split(' ');
                    Thorn.Runner.Configure(config =>
                    {
                        config.Scan(Assembly.GetExecutingAssembly());
                        config.SetDefaultType<Marea.Commands>();
                        config.UseCallbackToInstantiateExports(
                            (Type t) => { return new Commands(this, node); }
                        );
                    }).Run(args);
                }
            }
            System.Console.WriteLine("Marea.Console *CLOSED*");
        }

        public static void Main()
        {
            Console c = new Console();
            c.Start(null, null);
        }


        /// <summary>
        /// This is the santi-generated Battery proxy for the producers.
        /// </summary>
        class ConsoleProxy : RemoteProducer, IConsole
        {

            public ConsoleProxy(IServiceContainer container, ServiceAddress serviceAddress, TransportAddress control)
                : base(control)
            {
                this.container = container;
                this.id = serviceAddress;
            }

        }

    }
}