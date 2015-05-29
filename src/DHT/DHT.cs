using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using Marea;
using DHT;

namespace DHT
{
    public class DHT : Service, IDHT 
    {
        protected bool isRunning = false;
        protected Thread th;
        protected Dictionary<string, string> contents;
        protected int myID;
        protected int lowID;
        protected int highID;

        protected IDHT predecessor;
        protected String predecessorIP;
        protected IDHT successor;
        protected String successorIP;

        public bool Start()
        {
            isRunning = true;
            contents = new Dictionary<string, string>();
            th = new Thread(Run);
            th.Start();
            return true;
        }

        public bool Stop()
        {
            isRunning = false;
            return true;
        }

        void Run()
        {
            Console.WriteLine("Starting DHT Node");            
            Console.Write("Address of a node in the DHT [Enter if you are the first]:");
            String ip = Console.ReadLine();
            if (ip != "")
            {
                IDHT node = container.GetService<IDHT>(ip);
                if (node == null)
                {                    
                    //TODO Hacer mas bonito
                    throw new Exception("Node not found!");
                }
                else
                {
                    node.Login(id.ToString());
                    highID = id.GetHashCode() % 1024;
                }
            }
            else
            {
                lowID = 0;
                highID = 1024;
                predecessor = this;
                successor = this;
                predecessorIP = id.ToString();
                successorIP = id.ToString();
            }

            myID = this.id.GetHashCode() % 1024;
            Console.WriteLine("Adress: ", id);
            Console.WriteLine("ID: ", myID);            
            
            while (isRunning)
            {
                Console.WriteLine("1. Add content");
                Console.WriteLine("2. Query content");
                Console.WriteLine("3. Status");
                Console.Write(">");
                String key;
                String value;
                String option = Console.ReadLine();
                switch (option)
                {
                    case "1":
                        Console.Write("Key>");
                        key = Console.ReadLine();
                        Console.Write("Value>");
                        value = Console.ReadLine();
                        SetValue(key, value);
                        break;
                    case "2":
                        Console.Write("Key>");
                        key = Console.ReadLine();
                        value = GetValue(key);
                        if (value == null)
                        {
                            Console.WriteLine("Content NOT found");
                        }
                        else
                        {
                            Console.Write("Value: ");
                            Console.WriteLine(value);
                        }
                        break;
                    case "3":
                        Console.WriteLine("My ID:   " + myID);
                        Console.WriteLine("Low ID:  " + lowID);
                        Console.WriteLine("High ID: " + highID);
                        Console.WriteLine("Contents:");
                        foreach (String k in contents.Keys)
                        {
                            Console.WriteLine("\t" + k + " => " + contents[k]);
                        }
                        break;
                    default:
                        Console.WriteLine("Please input a correct option");
                        break;
                }                
            }

        }

        private bool IsInRange(int hash)
        {
            if (lowID < highID)
            {
                return hash >= lowID && hash < highID;
            }
            else
            {
                return (0 < hash && hash < highID || lowID < hash && hash < 1024);
            }
        }


        public string GetValue(string key)
        {
            int hash = key.GetHashCode() % 1024;
            if (IsInRange(hash))
            {
                Console.WriteLine("Getting " + key);
                return contents[key];
            }
            else
            {
                Console.WriteLine("Sending GET " + key + " to successor.");
                return successor.GetValue(key);
            }
        }

        public void SetValue(string key, string value)
        {
            int hash = key.GetHashCode() % 1024;
            if (IsInRange(hash))
            {
                Console.WriteLine("Adding " + key + "=>" + value);
                contents.Add(key, value);
            }
            else
            {
                Console.WriteLine("Sending SET " + key + "=>" + value + " to successor.");
                successor.SetValue(key, value);
            }
        }

        public void Login(string newIP)
        {
            int hash = newIP.GetHashCode() % 1024;
            if (IsInRange(hash))
            {
                IDHT newNode = container.GetService<IDHT>(newIP);
                if (newNode == null)
                {
                    Console.WriteLine("ERROR: " + newIP + " not found!");
                    return;
                }
                
                predecessor.SetSucessor(newIP);
                newNode.SetSucessor(id.ToString());
                newNode.SetPredecessor(predecessorIP);
                this.SetPredecessor(newIP); 
                //EQUIVALENTE A 
                //this.predecessorIP = newIP;
                //this.predecessor = newNode;

                // Send Keys
                Dictionary<String, String> keysToSend = new Dictionary<string, string>();
                foreach (String k in contents.Keys)
                {
                    int h = k.GetHashCode() % 1024;
                    if(IsInRange(h)) { //TODO Falta el recalculo de low,high
                        keysToSend.Add(k,contents[k]);
                        contents.Remove(k);
                    }
                }
                predecessor.SetValues(keysToSend);
            }
            else
            {
                successor.Login(newIP);
            }
        }

        public void Logout(string ip)
        {
            throw new NotImplementedException();
        }

        public void SetPredecessor(string ip)
        {
            IDHT node = container.GetService<IDHT>(ip);
            if (node == null)
            {
                Console.WriteLine("Cannot set predecessor " + ip);
            }
            else
            {
                predecessorIP = ip;
                predecessor = node;
                int hash = predecessorIP.GetHashCode() % 1024;
                lowID = hash;
            }
        }

        public void SetSucessor(string ip)
        {
            IDHT node = container.GetService<IDHT>(ip);
            if (node == null)
            {
                Console.WriteLine("Cannot set successor " + ip);
            }
            else
            {
                predecessorIP = ip;
                predecessor = node;
            }
        }

        public void SetValues(Dictionary<String, String> values)
        {
            foreach (String k in values.Keys)
            {
                contents.Add(k, values[k]);
            }
        }
    }
}
