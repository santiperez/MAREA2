using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using Marea;

namespace Napster
{
    public class Client : Service, IClient
    {
        private bool running = false;
        private Thread th;
        private Dictionary<String, String> contents;

        [LocateService("*/*/*/*/Napster.Server")]
        private IServer server;

        public override bool Start()
        {
            running = true;
            contents = new Dictionary<string, string>();
            th = new Thread(Run);
            th.Start();
            return true;
        }

        public void Run()
        {
            while (running)
            {
                Console.WriteLine("1. Añadir fichero");
                Console.WriteLine("2. Buscar fichero");
                Console.WriteLine("3. Descargar fichero");
                Console.Write(">");
                String line = Console.ReadLine();
                String file;
                String content;
                String[] ips;

                switch (line)
                {
                    case "1":                 
                        Console.Write("Fichero>");
                        file = Console.ReadLine();
                        Console.Write("Contenido>");
                        content = Console.ReadLine();
                        contents.Add(file, content);
                        server.IHaveFile(file, id.ToString()); 
                        Console.WriteLine("Contenido añadido correctamente.");
                        break;
                    case "2":
                        Console.Write("Fichero>");
                        file = Console.ReadLine();
                        ips = server.IWantFile(file);
                        if (ips != null)
                        {
                            foreach (String s in ips)
                            {
                                Console.WriteLine(s);
                            }
                        }
                        else
                        {
                            Console.WriteLine("No hay resultados.");
                        }
                        break;
                    case "3":
                        Console.Write("Fichero>");
                        file = Console.ReadLine();
                        ips = server.IWantFile(file);
                        if (ips==null || ips.Length < 0)
                        {
                            Console.WriteLine("No hay resultados.");
                        }
                        else
                        {
                            String ip = ips[0];
                            Console.WriteLine("El peer seleccionado es " + ip);
                            IClient peer = container.GetService<IClient>(ip);
                            content = peer.DownloadFile(file);
                            Console.WriteLine("El contenido es:");
                            Console.WriteLine(content);
                        }
                        break;             
                }
                Console.WriteLine("-----------------------------------------------------");
            }
        }

        public override bool Stop()
        {
            running = false;
            th.Abort();
            return true;
        }

        public string DownloadFile(string filename)
        {
            String content;
            Console.WriteLine("Devolviendo contenido " + filename);
            if (contents.TryGetValue(filename, out content)) {
                return content;
            }
            else
            {
                Console.WriteLine("ERROR! Este contenido no lo tengo.");
                return null;
            } 
        }
    }
}
