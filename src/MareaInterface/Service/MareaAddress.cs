using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Globalization;

namespace Marea
{
    public enum NameResolutionType { All, Locked, None, Static }

    //public class MareaAddress
    //{
    //    protected String address;

    //    private NameResolutionType NamingResolutionType;

    //    public MareaAddress() { address = "/*/*/*/*"; }

    //    public MareaAddress(string address)
    //    {
    //        CheckFormat(address);
    //        this.address = address;
    //    }
    //    public MareaAddress(MareaAddress m)
    //    {
    //        CheckFormat(m.address);
    //        this.address = m.address;
    //    }

    //    private void CheckFormat(string mareaAddress)
    //    {
    //        int slashCounter;

    //        //Check the begining of the MareaAddress and set the naming resolution type
    //        SetNamingResolutionTypefromAddress(mareaAddress);

    //        //Count number of '/' characters
    //        //It must be 4 or five depending if its a service or primitive address
    //        slashCounter = mareaAddress.Count(c => c == '/');
    //        if (slashCounter != 4 && slashCounter != 5)
    //            throw new FormatException("MareaAddress FormatException") { };

    //        //Check Node Info
    //        List<String> s = mareaAddress.Split('/').ToList();
    //        if (s[2] != "*")
    //            CreateIPEndPoint(s[2]);
    //    }

    //    public MareaAddress(String subsys, String ip, int port, String instance, String service)
    //    {
    //        SetMareaAddressFields(subsys, ip, port, instance, service, NameResolutionType.None);

    //    }

    //    public MareaAddress(String subsys, String ip, int port, String instance, String service, NameResolutionType type)
    //    {
    //        SetMareaAddressFields(subsys, ip, port, instance, service, type);
    //    }

    //    public MareaAddress(String subsys, IPAddress ip, int port, String instance, String service)
    //    {
    //        SetMareaAddressFields(subsys, ip.ToString(), port, instance, service, NameResolutionType.None);
    //    }


    //    public MareaAddress(String subsys, IPAddress ip, int port, String instance, String service, NameResolutionType type)
    //    {
    //        SetMareaAddressFields(subsys, ip.ToString(), port, instance, service, type);
    //    }

    //    public MareaAddress(String subsys, IPEndPoint endpoint, String instance, String service)
    //    {
    //        SetMareaAddressFields(subsys, endpoint.Address.ToString(), endpoint.Port, instance, service, NameResolutionType.None);
    //    }


    //    public MareaAddress(String subsys, IPEndPoint endpoint, String instance, String service, NameResolutionType type)
    //    {
    //        SetMareaAddressFields(subsys, endpoint.Address.ToString(), endpoint.Port, instance, service, type);
    //    }

    //    private void SetMareaAddressFields(String subsys, String ip, int port, String instance, String service, NameResolutionType type)
    //    {
    //        NamingResolutionType = type;

    //        switch (NamingResolutionType)
    //        {
    //            case NameResolutionType.All:
    //                address = String.Concat('*', '/', subsys, '/', ip + ":" + port, '/', instance, '/', service);
    //                break;
    //            case NameResolutionType.Locked:
    //                address = String.Concat('!', '/', subsys, '/', ip + ":" + port, '/', instance, '/', service);
    //                break;
    //            case NameResolutionType.Static:
    //                address = String.Concat('#', '/', subsys, '/', ip + ":" + port, '/', instance, '/', service);
    //                break;
    //            case NameResolutionType.None:
    //                address = String.Concat('/', subsys, '/', ip + ":" + port, '/', instance, '/', service);
    //                break;
    //        }

    //    }

    //    protected String GetField(int i)
    //    {
    //        //Old way
    //        //if (i >= address.Count(c => c == '/'))
    //        //    return null;
    //        //else
    //        //    return address.Split('/')[i + 1];

    //        String[] fields = address.Split('/');
    //        int count = fields.Length;

    //        if (i + 1 >= count)
    //            return null;
    //        else
    //            return fields[i + 1];
    //    }

    //    protected void SetField(int i, String txt)
    //    {
    //        if (txt.Contains("/"))
    //            throw new FormatException { };

    //        List<String> s = address.Split('/').ToList();
    //        s = s.Where(x => x != "").ToList();

    //        if (NamingResolutionType != NameResolutionType.None)
    //            i = i + 1;

    //        if (s.Count < i + 1)
    //            s.Add(txt);
    //        else
    //            s[i] = txt;

    //        if (s.Count == 5)
    //        {
    //            if (NamingResolutionType == NameResolutionType.None)
    //                address = String.Concat('/', s[0], '/', s[1], '/', s[2], '/', s[3], '/', s[4]);
    //            else
    //                address = String.Concat(s[0], '/', s[1], '/', s[2], '/', s[3], '/', s[4]);
    //        }
    //        else if (s.Count == 6)
    //        {
    //            if (NamingResolutionType == NameResolutionType.None)
    //                address = String.Concat('/', s[0], '/', s[1], '/', s[2], '/', s[3], '/', s[4], '/', s[5]);
    //            else
    //                address = String.Concat(s[0], '/', s[1], '/', s[2], '/', s[3], '/', s[4], '/', s[5]);
    //        }
    //        else if (s.Count == 4)
    //        {

    //            if (NamingResolutionType == NameResolutionType.None)
    //                address = String.Concat('/', s[0], '/', s[1], '/', s[2], '/', s[3]);
    //            else
    //                address = String.Concat(s[0], '/', s[1], '/', s[2], '/', s[3]);
    //        }
    //    }

    //    public void SetNamingResolutionType(NameResolutionType type)
    //    {
    //        string typeString = "";

    //        switch (type)
    //        {
    //            case NameResolutionType.All:
    //                typeString = "*";
    //                break;
    //            case NameResolutionType.Locked:
    //                typeString = "!";
    //                break;
    //            case NameResolutionType.Static:
    //                typeString = "#";
    //                break;
    //            case NameResolutionType.None:
    //                typeString = "";
    //                break;
    //        }
    //        if (NamingResolutionType == NameResolutionType.None && address.StartsWith("/"))
    //        {
    //            address = typeString + address;

    //        }
    //        else if (NamingResolutionType != NameResolutionType.None && !address.StartsWith("/"))
    //        {
    //            SetField(-1, typeString);
    //            address.Remove(0, 1);
    //        }
    //        else
    //            throw new NotSupportedException() { };

    //        NamingResolutionType = type;
    //    }

    //    public NameResolutionType GetNamingResolutionType()
    //    {
    //        return NamingResolutionType;
    //    }

    //    private void SetNamingResolutionTypefromAddress(string address)
    //    {
    //        if (address.StartsWith("*/"))
    //            NamingResolutionType = NameResolutionType.All;
    //        else if (address.StartsWith("!/"))
    //            NamingResolutionType = NameResolutionType.Locked;
    //        else if (address.StartsWith("#/"))
    //            NamingResolutionType = NameResolutionType.Static;
    //        else if (address.StartsWith("/"))
    //            NamingResolutionType = NameResolutionType.None;
    //        else
    //            throw new FormatException("MareaAddress FormatException") { };
    //    }

    //    public String GetSubsystem()
    //    { return GetField(0); }

    //    public void SetSubsystem(String subsys)
    //    { SetField(0, subsys); }

    //    public String GetNode()
    //    { return GetField(1); }

    //    public IPEndPoint GetNodeAsIPEndPoint()
    //    { return CreateIPEndPoint(GetField(1)); }

    //    public void SetNode(String node)
    //    {
    //        CreateIPEndPoint(node);
    //        SetField(1, node);
    //    }

    //    public void SetNode(IPEndPoint node)
    //    { SetField(1, node.ToString()); }


    //    public String GetInstance()
    //    { return GetField(2); }

    //    public void SetInstance(String instance)
    //    { SetField(2, instance); }

    //    public String GetService()
    //    { return GetField(3); }

    //    public void SetService(String service)
    //    { SetField(3, service); }

    //    public String GetPrimitive()
    //    {
    //        return GetField(4);
    //    }

    //    public void SetPrimitive(String primitive)
    //    {
    //        SetField(4, primitive);
    //    }

    //    public String GetServiceAddress()
    //    {
    //        List<String> s = address.Split('/').ToList();
    //        s = s.Where(x => x != "").ToList();

    //        if (NamingResolutionType == NameResolutionType.None)
    //            return String.Concat('/', s[0], '/', s[1], '/', s[2], '/', s[3]);

    //        return String.Concat(s[0], '/', s[1], '/', s[2], '/', s[3], '/', s[4]);
    //    }

    //    public String GetPrimitiveAddress()
    //    {
    //        if (GetPrimitive() != null)
    //            return address;
    //        else
    //            return null;
    //    }

    //    // Handles IPv4 and IPv6 notation.
    //    public static IPEndPoint CreateIPEndPoint(string endPoint)
    //    {
    //        string[] ep = endPoint.Split(':');
    //        if (ep.Length < 2) throw new FormatException("Invalid endpoint format");
    //        IPAddress ip;
    //        if (ep.Length > 2)
    //        {
    //            if (!IPAddress.TryParse(string.Join(":", ep, 0, ep.Length - 1), out ip))
    //            {
    //                throw new FormatException("Invalid ip-adress");
    //            }
    //        }
    //        else
    //        {
    //            if (!IPAddress.TryParse(ep[0], out ip))
    //            {
    //                throw new FormatException("Invalid ip-adress");
    //            }
    //        }
    //        int port;
    //        if (!int.TryParse(ep[ep.Length - 1], NumberStyles.None, NumberFormatInfo.CurrentInfo, out port))
    //        {
    //            throw new FormatException("Invalid port");
    //        }
    //        return new IPEndPoint(ip, port);
    //    }

    //    public override String ToString()
    //    {
    //        string txt;
    //        txt = this.GetPrimitiveAddress();
    //        if (txt == null)
    //            txt = this.GetServiceAddress();
    //        return txt;
    //    }

    //    public override bool Equals(object obj)
    //    {
    //        if (!(obj is MareaAddress)) return false;
    //        return address.Equals(((MareaAddress)obj).address);
    //    }

    //    public override int GetHashCode()
    //    {
    //        return address.GetHashCode();
    //    }

    //    public static bool isMareaAddress(string txt)
    //    {
    //        int count = txt.Count(f => f == '/');
    //        if ((count == 4 || count == 5))
    //            return true;
    //        else
    //            return false;
    //    }

    //    public bool isRemote(string system, IPEndPoint node)
    //    {
    //        if (GetSubsystem() != system || !GetNodeAsIPEndPoint().Equals(node))
    //            return true;
    //        return false;
    //    }

    //    public bool isRemote(string system, string node)
    //    {
    //        if (GetSubsystem() != system || GetNode() != node)
    //            return true;
    //        return false;
    //    }

    //    public bool isQueryAddress()
    //    {
    //        return address.Contains("*/");
    //    }
    //}

    public class MareaAddress
    {
        private NameResolutionType NamingResolutionType;

        private string modifier;

        private string subsystem;

        private string node;

        private string instance;

        private string service;

        private string primitive;

        public MareaAddress(string address)
        {
            Process(address);
        }

        public MareaAddress(MareaAddress m)
        {
            string[] ipEndPoint= m.node.Split(':');
            SetMareaAddressFields(m.subsystem, ipEndPoint[0], Convert.ToInt32(ipEndPoint[1]), m.instance, m.service, m.NamingResolutionType, m.primitive);
        }

        public MareaAddress(String subsys, String ip, int port, String instance, String service)
        {
            SetMareaAddressFields(subsys, ip, port, instance, service, NameResolutionType.None,null);
        }

        public MareaAddress(String subsys, String ip, int port, String instance, String service, NameResolutionType type)
        {
            SetMareaAddressFields(subsys, ip, port, instance, service, type,null);
        }

        public MareaAddress(String subsys, IPAddress ip, int port, String instance, String service)
        {
            SetMareaAddressFields(subsys, ip.ToString(), port, instance, service, NameResolutionType.None,null);
        }


        public MareaAddress(String subsys, IPAddress ip, int port, String instance, String service, NameResolutionType type)
        {
            SetMareaAddressFields(subsys, ip.ToString(), port, instance, service, type,null);
        }

        public MareaAddress(String subsys, IPEndPoint endpoint, String instance, String service)
        {
            SetMareaAddressFields(subsys, endpoint.Address.ToString(), endpoint.Port, instance, service, NameResolutionType.None,null);
        }


        public MareaAddress(String subsys, IPEndPoint endpoint, String instance, String service, NameResolutionType type)
        {
            SetMareaAddressFields(subsys, endpoint.Address.ToString(), endpoint.Port, instance, service, type,null);
        }

        private void SetMareaAddressFields(String subsys, String ip, int port, String instance, String service, NameResolutionType type, String primitive)
        {
            SetNamingResolutionType(type);

            this.subsystem = subsys;
            this.node = ip + ":" + port;
            this.instance = instance;
            this.service = service;
            this.primitive = primitive;
        }

        private void Process(string address)
        {
            SetNamingResolutionTypefromAddress(address);

            int field = 0;
            int from = 1;

            if (NamingResolutionType != NameResolutionType.None)
                from = 2;

            for (int i = from; i < address.Length; i++)
            {
                if (address[i] == '/')
                {
                    switch (field)
                    {
                        case 0:
                            subsystem = address.Substring(from, i - from);
                            break;
                        case 1:
                            node = address.Substring(from, i - from);
                            break;
                        case 2:
                            instance = address.Substring(from, i - from);
                            break;
                        case 3:
                            service = address.Substring(from, i - from);
                            break;
                        default:
                            throw new FormatException("MareaAddress FormatException") { };
                    }
                    from = i + 1;

                    //Check if there is a double dash and throw an exception
                    if (address[from] == '/')
                        throw new FormatException("MareaAddress FormatException") { };
                    field++;
                }

                if (i == address.Length - 1)
                {
                    switch (field)
                    {
                        case 3:
                            service = address.Substring(from, i + 1 - from);
                            break;
                        case 4:
                            primitive = address.Substring(from, i + 1 - from);
                            break;
                        default:
                            throw new FormatException("MareaAddress FormatException") { };
                    }

                    //Check if the address finishes with a dash and throw and exception
                    if (address[i] == '/')
                        throw new FormatException("MareaAddress FormatException") { };
                }

            }

        }

        private void SetNamingResolutionTypefromAddress(string address)
        {
            char first = address[0];
            char second = address[1];

            if (first == '*' && second == '/')
                NamingResolutionType = NameResolutionType.All;
            else if (first == '!' && second == '/')
                NamingResolutionType = NameResolutionType.Locked;
            else if (first == '#' && second == '/')
                NamingResolutionType = NameResolutionType.Static;
            else if (first == '/' && second != '/')
                NamingResolutionType = NameResolutionType.None;
            else
                throw new FormatException("MareaAddress FormatException") { };

            if (NamingResolutionType != NameResolutionType.None)
                modifier = Convert.ToString(first);
            else
                modifier = "";
        }

        public NameResolutionType GetNamingResolutionType()
        {
            return NamingResolutionType;
        }

        public void SetNamingResolutionType(NameResolutionType type)
        {
            NamingResolutionType = type;

            switch (type)
            {
                case NameResolutionType.All:
                    modifier = "*";
                    break;
                case NameResolutionType.Locked:
                    modifier = "!";
                    break;
                case NameResolutionType.Static:
                    modifier = "#";
                    break;
                case NameResolutionType.None:
                    modifier = "";
                    break;
            }
        }

        public String GetSubsystem()
        { return subsystem; }

        public void SetSubsystem(String subsys)
        { this.subsystem = subsys; }

        public String GetNode()
        { return node; }

        public void SetNode(String node)
        {
            CreateIPEndPoint(node);
            this.node = node; 
        }

        public IPEndPoint GetNodeAsIPEndPoint()
        { return CreateIPEndPoint(node); }

        public void SetNode(IPEndPoint node)
        { this.node = node.ToString(); }

        public String GetInstance()
        { return instance; }

        public void SetInstance(String instance)
        { this.instance = instance; }

        public String GetService()
        { return service; }

        public void SetService(String service)
        { this.service = service; }

        public String GetPrimitive()
        { return primitive; }

        public void SetPrimitive(String primitive)
        { this.primitive = primitive; }

        public String GetServiceAddress()
        {
            return string.Join("/", new string[] { modifier, subsystem, node, instance, service });
        }

        public String GetPrimitiveAddress()
        {
            if (primitive != null)
                return string.Join("/", new string[] { modifier, subsystem, node, instance, service, primitive });
            else
                return null;
        }

        // Handles IPv4 and IPv6 notation.
        public static IPEndPoint CreateIPEndPoint(string endPoint)
        {
            string[] ep = endPoint.Split(':');
            if (ep.Length < 2) throw new FormatException("Invalid endpoint format");
            IPAddress ip;
            if (ep.Length > 2)
            {
                if (!IPAddress.TryParse(string.Join(":", ep, 0, ep.Length - 1), out ip))
                {
                    throw new FormatException("Invalid ip-adress");
                }
            }
            else
            {
                if (!IPAddress.TryParse(ep[0], out ip))
                {
                    throw new FormatException("Invalid ip-adress");
                }
            }
            int port;
            if (!int.TryParse(ep[ep.Length - 1], NumberStyles.None, NumberFormatInfo.CurrentInfo, out port))
            {
                throw new FormatException("Invalid port");
            }
            return new IPEndPoint(ip, port);
        }

        public override String ToString()
        {
            string txt;
            txt = this.GetPrimitiveAddress();
            if (txt == null)
                txt = this.GetServiceAddress();
            return txt;
        }

        public override bool Equals(object obj)
        {
            if (!(obj is MareaAddress)) return false;
            MareaAddress mad=(MareaAddress)obj;
            return mad.modifier == modifier && mad.NamingResolutionType == NamingResolutionType && mad.subsystem == subsystem && mad.node == node && mad.instance == instance && mad.service == service && mad.primitive == primitive;
        }

        public override int GetHashCode()
        {
            if (primitive == null)
                return GetServiceAddress().GetHashCode();
            else
                return GetPrimitiveAddress().GetHashCode();
        }

        public bool isRemote(string system, IPEndPoint node)
        {
            if (GetSubsystem() != system || !GetNodeAsIPEndPoint().Equals(node))
                return true;
            return false;
        }

        public bool isRemote(string system, string node)
        {
            if (GetSubsystem() != system || GetNode() != node)
                return true;
            return false;
        }

        public bool isQueryAddress()
        {
            return NamingResolutionType != NameResolutionType.None;
        }
    }
}
