using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using System.Net;
using System.IO;

namespace MareaGen
{
    /// <summary>
    /// Delegate used to serialize basic and MAREA types.
    /// </summary>
    public delegate void EncodeFunction(object o, byte[] buffer, ref int offset);

    /// <summary>
    /// Delegate used to deserialize a basic and MAREA types.
    /// </summary>
    public delegate object DecodeFunction(byte[] buffer, ref int offset);

    /// <summary>
    /// Stores an unique byte identifier reference to the type and a reference to its corresponding serialization method.
    /// </summary>
    public class EncodeData
    {
        public byte id;
        public EncodeFunction func;

        public EncodeData(byte id, EncodeFunction func)
        {
            this.id = id;
            this.func = func;
        }
    }

    /// <summary>
    /// Manages the different serializable basic and MAREA types.
    /// </summary>
    public class CoderTables
    {
        /// <summary>
        /// Dictionary<byte, string> used to manage the name of the different types and it corresponding unique identifier.
        /// </summary>
        public Dictionary<byte, string> M2Types
        {
            get { return m2Types; }
            set { m2Types = value; }
        }
        private Dictionary<byte, string> m2Types;

        /// <summary>
        /// DecodeFunction[] used to manage the different serialization methods.
        /// </summary>
        public DecodeFunction[] DecodeTable
        {
            get { return decodeTable; }
            set { decodeTable = value; }
        }
        private DecodeFunction[] decodeTable;

        /// <summary>
        /// Hashtable used to manage the different serialization methods.
        /// </summary>
        public Hashtable EncodeTable
        {
            get { return encodeTable; }
            set { encodeTable = value; }
        }
        private Hashtable encodeTable;

        /// <summary>
        /// byte used to control the list of types that have been added to the MAREA coder.
        /// Its value goes from 64 to 255.
        /// ID: 64-> NULL.
        /// ID: 65 to 126-> CODER TYPES (basic types).
        /// ID: 127-> NOT NULL.
        /// ID: 128-255-> OTHER (SERVICES).
        /// </summary>
        public byte Indexer
        {
            get { return indexer; }
            set { indexer = value; }
        }
        private  byte indexer;

        // <summary>
        /// byte used to control the list of MAREA MESSAGES that have been added to the MAREA coder.
        /// Its value goes from 0 to 63.
        /// ID: 0 to 63-> MAREA MESSAGES
        private byte mareaMessageIndexer = 0;

        public byte MareaMessageIndexer
        {
            get { return mareaMessageIndexer; }
            set { mareaMessageIndexer = value; }
        }

        /// <summary>
        /// CoderTables instance.
        /// </summary>
        private static CoderTables instance = null;

        /// <summary>
        /// Constructor. Registers all the basic types contained in CoderTypes class.
        /// </summary>
        private CoderTables()
        {
            decodeTable = new DecodeFunction[255];
            encodeTable = new Hashtable();
            m2Types = new Dictionary<byte, string>();
            
            //ID 64 is (0100 0000) used to represent a null object
            m2Types.Add((byte)CoderBytes.NULL, "NULL");

            //IDs from 65 to 126 are used to 126 are used by CoderTypes (basic types)
            m2Types.Add((byte)CoderBytes.GENERIC, "Generic Type Parameters");
            
            //update indexer
            indexer = Convert.ToByte(CoderBytes.GENERIC+1);

            m2Types.Add(indexer, "System.Object");
            AddClass(typeof(object), indexer++, CoderTypes.ToObject, CoderTypes.FromObject);

            m2Types.Add(indexer, "System.Int32");
            AddClass(typeof(int), indexer++, CoderTypes.ToInt, CoderTypes.FromInt);

            m2Types.Add(indexer, "System.UInt32");
            AddClass(typeof(uint), indexer++, CoderTypes.ToUInt, CoderTypes.FromUInt);

            m2Types.Add(indexer, "System.Int16");
            AddClass(typeof(short), indexer++, CoderTypes.ToShort, CoderTypes.FromShort);

            m2Types.Add(indexer, "System.UInt16");
            AddClass(typeof(ushort), indexer++, CoderTypes.ToUShort, CoderTypes.FromUShort);

            m2Types.Add(indexer, "System.Int64");
            AddClass(typeof(long), indexer++, CoderTypes.ToLong, CoderTypes.FromLong);

            m2Types.Add(indexer, "System.Single");
            AddClass(typeof(float), indexer++, CoderTypes.ToFloat, CoderTypes.FromFloat);

            m2Types.Add(indexer, "System.Double");
            AddClass(typeof(double), indexer++, CoderTypes.ToDouble, CoderTypes.FromDouble);

            m2Types.Add(indexer, "System.Char");
            AddClass(typeof(char), indexer++, CoderTypes.ToChar, CoderTypes.FromChar);

            m2Types.Add(indexer, "System.String");
            AddClass(typeof(string), indexer++, CoderTypes.ToString, CoderTypes.FromString);

            m2Types.Add(indexer, "System.DateTime");
            AddClass(typeof(DateTime), indexer++, CoderTypes.ToDateTime, CoderTypes.FromDateTime);

            m2Types.Add(indexer, "System.Byte");
            AddClass(typeof(byte), indexer++, CoderTypes.ToByte, CoderTypes.FromByte);

            m2Types.Add(indexer, "System.Boolean");
            AddClass(typeof(bool), indexer++, CoderTypes.ToBoolean, CoderTypes.FromBoolean);

            m2Types.Add(indexer, "System.Net.Enum");
            AddClass(typeof(Enum), indexer++, CoderTypes.ToEnum, CoderTypes.FromEnum);

            m2Types.Add(indexer, "System.Net.IPEndPoint");
            AddClass(typeof(IPEndPoint), indexer++, CoderTypes.ToIpEndPoint, CoderTypes.FromIpEndPoint);

            m2Types.Add(indexer, "System.Array");
            AddClass(typeof(Array), indexer++, CoderTypes.ToArray, CoderTypes.FromArray);

            m2Types.Add(indexer, "System.Int32[]");
            AddClass(typeof(int[]), indexer++, CoderTypes.ToIntArray, CoderTypes.FromIntArray);

            m2Types.Add(indexer, "System.UInt32[]");
            AddClass(typeof(uint[]), indexer++, CoderTypes.ToUIntArray, CoderTypes.FromUIntArray);

            m2Types.Add(indexer, "System.Int16[]");
            AddClass(typeof(short[]), indexer++, CoderTypes.ToShortArray, CoderTypes.FromShortArray);

            m2Types.Add(indexer, "System.UInt16[]");
            AddClass(typeof(ushort[]), indexer++, CoderTypes.ToUShortArray, CoderTypes.FromUShortArray);

            m2Types.Add(indexer, "System.Int64[]");
            AddClass(typeof(long[]), indexer++, CoderTypes.ToLongArray, CoderTypes.FromLongArray);

            m2Types.Add(indexer, "System.Single[]");
            AddClass(typeof(float[]), indexer++, CoderTypes.ToFloatArray, CoderTypes.FromFloatArray);

            m2Types.Add(indexer, "System.Double[]");
            AddClass(typeof(double[]), indexer++, CoderTypes.ToDoubleArray, CoderTypes.FromDoubleArray);

            m2Types.Add(indexer, "System.Char[]");
            AddClass(typeof(char[]), indexer++, CoderTypes.ToCharArray, CoderTypes.FromCharArray);

            m2Types.Add(indexer, "System.String[]");
            AddClass(typeof(string[]), indexer++, CoderTypes.ToStringArray, CoderTypes.FromStringArray);

            m2Types.Add(indexer, "System.DateTime[]");
            AddClass(typeof(DateTime[]), indexer++, CoderTypes.ToDateTimeArray, CoderTypes.FromDateTimeArray);

            m2Types.Add(indexer, "System.Byte[]");
            AddClass(typeof(byte[]), indexer++, CoderTypes.ToByteArray, CoderTypes.FromByteArray);

            m2Types.Add(indexer, "System.Boolean[]");
            AddClass(typeof(bool[]), indexer++, CoderTypes.ToBooleanArray, CoderTypes.FromBooleanArray);

            m2Types.Add(indexer, "System.Collections.Hashtable");
            AddClass(typeof(Hashtable), indexer++, CoderTypes.ToHashtable, CoderTypes.FromHashtable);

            m2Types.Add(indexer, "System.Collections.ArrayList");
            AddClass(typeof(ArrayList), indexer++, CoderTypes.ToArrayList, CoderTypes.FromArrayList);

            m2Types.Add(indexer, "System.Collections.Generic.Dictionary");
            AddClass(typeof(Dictionary<,>), indexer++, CoderTypes.ToDictionary, CoderTypes.FromDictionary);

            m2Types.Add(indexer, "System.Collections.Generic.List");
            AddClass(typeof(List<>), indexer++, CoderTypes.ToList, CoderTypes.FromList);

            m2Types.Add(indexer, "System.IO.Stream");
            AddClass(typeof(Stream), indexer++, CoderTypes.ToStream, CoderTypes.FromStream);

            m2Types.Add(indexer, "System.IO.MemoryStream");
            AddClass(typeof(MemoryStream), indexer++, CoderTypes.ToMemoryStream, CoderTypes.FromMemoryStream);

            m2Types.Add(indexer, "Marea.MareaAddress");
            AddClass(typeof(Marea.MareaAddress), indexer++, CoderTypes.ToMareaAddress, CoderTypes.FromMareaAddress);

            m2Types.Add((byte)CoderBytes.NOT_NULL, "NOT_NULL");

            //update indexer. Other types are generated with an ID equal or bigger than 128
            indexer = Convert.ToByte(CoderBytes.NOT_NULL + 1);
        }

        /// <summary>
        /// Manages CoderTables throught singleton pattern.
        /// </summary>
        public static CoderTables GetInstance()
        {
            if (instance == null)
                instance = new CoderTables();
            return instance;
        }

        /// <summary>
        /// Adds a new type into the MAREA coder.
        /// </summary>
        public void AddClass(Type t, byte id, DecodeFunction df, EncodeFunction ef)
        {
            decodeTable[id] = df;
            encodeTable.Add(t.FullName, new EncodeData(id, ef));
        }


        /// <summary>
        /// Adds a new MAREA Message class to the coder.
        /// </summary>
        public void AddMareaMessage(Type t, DecodeFunction df, EncodeFunction ef)
        {
            decodeTable[mareaMessageIndexer] = df;
            encodeTable.Add(t.FullName, new EncodeData(mareaMessageIndexer, ef));
        }
    }
}