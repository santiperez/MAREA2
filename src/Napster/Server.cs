using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Marea;

namespace Napster
{
    class Server : Service, IServer
    {
        public Dictionary<String, List<String>> peers = new Dictionary<string, List<string>>();

        public void IHaveFile(string fileName, string ip)
        {
            List<string> nodes;

            Console.WriteLine("Añadiendo peer " + ip + " para el fichero " + fileName);
            if (peers.TryGetValue(fileName, out nodes))
            {
                nodes.Add(ip);
                //Console.WriteLine("Listado de peers");
                //foreach(var s in nodes) {
                //    Console.WriteLine(s);
                //}
            }
            else
            {
                Console.WriteLine("Es el primer peer para este contenido");
                nodes = new List<string>();
                nodes.Add(ip);
                peers.Add(fileName, nodes);
            }
        }

        public string[] IWantFile(string fileName)
        {
            List<String> nodes = null;
            Console.WriteLine("Buscando peers para el fichero " + fileName);
            if (peers.TryGetValue(fileName, out nodes))
            {
                Console.WriteLine("Listado de peers");
                foreach (var s in nodes)
                {
                    Console.WriteLine(s);
                }
                return nodes.ToArray<String>();
            }
            else
            {
                return null;
            }
        }
    }
}
