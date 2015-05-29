using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using System.Globalization;
using System.Net;
using System.Reflection;
using System.IO;
using Marea;
using System.Runtime.CompilerServices;

namespace MareaGen
{
    public class CoderTypes
    {
        /// <summary>
        /// Cache of types: Requests for types can be served faster in the future. 
        /// </summary>
        public static Dictionary<string, Type> typesCache = AssembliesManager.Instance.GetAllTypes();

        /// <summary>
        /// Constructor.
        /// </summary>
        public CoderTypes()
        {
        }

        /// <summary>
        /// Deserializes an int. 
        /// </summary>
        [MethodImpl((short)ExtendedMethodImplOptions.AggressiveInlining)]
        public static object ToInt(byte[] buffer, ref int offset)
        {
            return (int)(
               (buffer[offset++] & 0x000000FF) |
               (buffer[offset++] << 8 & 0x0000FF00) |
               (buffer[offset++] << 16 & 0x00FF0000) |
               (buffer[offset++] << 24)
               );
        }

        /// <summary>
        /// Serializes an int. 
        /// </summary>
        [MethodImpl((short)ExtendedMethodImplOptions.AggressiveInlining)]
        public static void FromInt(object theInt, byte[] buffer, ref int offset)
        {
            unchecked
            {
                buffer[offset++] = (byte)((int)theInt);
                buffer[offset++] = (byte)((int)theInt >> 8);
                buffer[offset++] = (byte)((int)theInt >> 16);
                buffer[offset++] = (byte)((int)theInt >> 24);
            }
        }

        /// <summary>
        /// Deserializes an uint. 
        /// </summary>
        [MethodImpl((short)ExtendedMethodImplOptions.AggressiveInlining)]
        public static object ToUInt(byte[] buffer, ref int offset)
        {
            return (uint)(
               (buffer[offset++] & 0x000000FF) |
               (buffer[offset++] << 8 & 0x0000FF00) |
               (buffer[offset++] << 16 & 0x00FF0000) |
               (buffer[offset++] << 24)
               );
        }

        /// <summary>
        /// Serializes an uint. 
        /// </summary>
        [MethodImpl((short)ExtendedMethodImplOptions.AggressiveInlining)]
        public static void FromUInt(object theUInt, byte[] buffer, ref int offset)
        {
            unchecked
            {
                buffer[offset++] = (byte)((uint)theUInt);
                buffer[offset++] = (byte)((uint)theUInt >> 8);
                buffer[offset++] = (byte)((uint)theUInt >> 16);
                buffer[offset++] = (byte)((uint)theUInt >> 24);
            }
        }

        /// <summary>
        /// Deserializes a long. 
        /// </summary>
        [MethodImpl((short)ExtendedMethodImplOptions.AggressiveInlining)]
        public static object ToLong(byte[] buffer, ref int offset)
        {
            long number = 0;
            for (int i = offset + 7; i > offset - 1; i--)
            {
                number = number << 8; // left shift bits in long already
                number += buffer[i]; // add in bottom 8 bits
            }
            offset += 8;
            return number;
        }

        /// <summary>
        /// Serializes a long. 
        /// </summary>
        [MethodImpl((short)ExtendedMethodImplOptions.AggressiveInlining)]
        public static void FromLong(object theLong, byte[] buffer, ref int offset)
        {
            unchecked
            {
                buffer[offset++] = (byte)((long)theLong);
                buffer[offset++] = (byte)((long)theLong >> 8);
                buffer[offset++] = (byte)((long)theLong >> 16);
                buffer[offset++] = (byte)((long)theLong >> 24);
                buffer[offset++] = (byte)((long)theLong >> 32);
                buffer[offset++] = (byte)((long)theLong >> 40);
                buffer[offset++] = (byte)((long)theLong >> 48);
                buffer[offset++] = (byte)((long)theLong >> 56);
            }
        }

        /// <summary>
        /// Deserializes a short. 
        /// </summary>
        [MethodImpl((short)ExtendedMethodImplOptions.AggressiveInlining)]
        public static object ToShort(byte[] buffer, ref int offset)
        {
            return (short)(
               (buffer[offset++] & 0x000000FF) |
               (buffer[offset++] << 8 & 0x0000FF00)
               );
        }

        /// <summary>
        /// Serializes a short. 
        /// </summary>
        [MethodImpl((short)ExtendedMethodImplOptions.AggressiveInlining)]
        public static void FromShort(object theShort, byte[] buffer, ref int offset)
        {
            unchecked
            {
                buffer[offset++] = (byte)((short)theShort);
                buffer[offset++] = (byte)((short)theShort >> 8);
            }
        }

        /// <summary>
        /// Deserializes an ushort. 
        /// </summary>
        [MethodImpl((short)ExtendedMethodImplOptions.AggressiveInlining)]
        public static object ToUShort(byte[] buffer, ref int offset)
        {
            return (ushort)(
               (buffer[offset++] & 0x000000FF) |
               (buffer[offset++] << 8 & 0x0000FF00)
               );
        }

        /// <summary>
        /// Serializes an ushort. 
        /// </summary>
        [MethodImpl((short)ExtendedMethodImplOptions.AggressiveInlining)]
        public static void FromUShort(object theUShort, byte[] buffer, ref int offset)
        {
            unchecked
            {
                buffer[offset++] = (byte)((ushort)theUShort);
                buffer[offset++] = (byte)((ushort)theUShort >> 8);
            }
        }

        /// <summary>
        /// Deserializes a float. 
        /// </summary>
        [MethodImpl((short)ExtendedMethodImplOptions.AggressiveInlining)]
        public static object ToFloat(byte[] buffer, ref int offset)
        {

            byte[] fArray = new byte[4];
            Buffer.BlockCopy(buffer, offset, fArray, 0, fArray.Length);
            offset += fArray.Length;
            return BitConverter.ToSingle(fArray, 0);

            //M1 float deserialization
            //string s= (string)ToString(buffer, ref offset);
            //return(float)double.Parse(s, CultureInfo.InvariantCulture);
        }


        /// <summary>
        /// Serializes a float. 
        /// </summary>
        [MethodImpl((short)ExtendedMethodImplOptions.AggressiveInlining)]
        public static void FromFloat(object theFloat, byte[] buffer, ref int offset)
        {
            byte[] fArray = BitConverter.GetBytes((float)theFloat);
            Buffer.BlockCopy(fArray, 0, buffer, offset, fArray.Length);
            offset += fArray.Length;

            //M1 float serialization
            //float _float=(float)theFloat;
            //FromString(_float.ToString(CultureInfo.InvariantCulture), buffer, ref offset);
        }

        /// <summary>
        /// Deserializes a double. 
        /// </summary>
        [MethodImpl((short)ExtendedMethodImplOptions.AggressiveInlining)]
        public static object ToDouble(byte[] buffer, ref int offset)
        {

            byte[] dfArray = new byte[8];
            Buffer.BlockCopy(buffer, offset, dfArray, 0, dfArray.Length);
            offset += dfArray.Length;
            return BitConverter.ToDouble(dfArray, 0);

            //M1 double deserialization
            //string _double = (string)ToString(buffer, ref offset);
            //double d = double.Parse(_double, CultureInfo.InvariantCulture);
            //return d;
        }

        /// <summary>
        /// Serializes a double. 
        /// </summary>
        [MethodImpl((short)ExtendedMethodImplOptions.AggressiveInlining)]
        public static void FromDouble(object theDouble, byte[] buffer, ref int offset)
        {
            byte[] fArray = BitConverter.GetBytes((double)theDouble);
            Buffer.BlockCopy(fArray, 0, buffer, offset, fArray.Length);
            offset += fArray.Length;

            //M1 double serialization
            //double _double = (double)theDouble;
            //FromString(theDouble.ToString(CultureInfo.InvariantCulture), buffer, ref offset);
        }

        /// <summary>
        /// Deserializes a char. 
        /// </summary>
        [MethodImpl((short)ExtendedMethodImplOptions.AggressiveInlining)]
        public static object ToChar(byte[] buffer, ref int offset)
        {
            byte[] bytes = new byte[2];

            bytes[0] = buffer[offset++];
            bytes[1] = buffer[offset++];
            return BitConverter.ToChar(bytes, 0);

            //M1 char deserialization
            //Caution! chars are in UTF8 so can be 2 bytes.
            //return(char)buffer[offset++];
        }

        /// <summary>
        /// Serializes a char. 
        /// </summary>
        [MethodImpl((short)ExtendedMethodImplOptions.AggressiveInlining)]
        public static void FromChar(object o, byte[] buffer, ref int offset)
        {
            byte[] bytes = BitConverter.GetBytes((char)o);

            for (int i = 0; i < bytes.Length; i++)
                buffer[offset++] = (byte)(bytes[i]);

            //M1 char serialization
            //Caution! chars are in UTF8 so can be 2 bytes.
            //buffer[offset++] = (byte)((char)o);
        }

        /// <summary>
        /// Deserializes a string. 
        /// </summary>
        [MethodImpl((short)ExtendedMethodImplOptions.AggressiveInlining)]
        public static object ToString(byte[] buffer, ref int offset)
        {
            byte value = buffer[offset++];

            if (value == (byte)CoderBytes.NULL)
                return null;
            else
            {
                // Strings are prefixed with an integer size
                int len = (int)ToInt(buffer, ref offset);

                // Encoding's GetChars() converts an entire buffer, so extract just the string part
                byte[] tmpBuffer = new byte[len];

                Array.Copy(buffer, offset, tmpBuffer, 0, len);
                String theString = new String(Encoding.UTF8.GetChars(tmpBuffer));
                offset += len;

                return theString;
            }
        }

        /// <summary>
        /// Serializes a string. 
        /// </summary>
        [MethodImpl((short)ExtendedMethodImplOptions.AggressiveInlining)]
        public static void FromString(object theString, byte[] buffer, ref int offset)
        {
            if (theString == null)
                buffer[offset++] = (byte)CoderBytes.NULL;
            else
            {
                buffer[offset++] = (byte)CoderBytes.NOT_NULL;


                byte[] tmpBuffer = Encoding.UTF8.GetBytes((string)theString);

                // Prefix string with int length
                FromInt(tmpBuffer.Length, buffer, ref offset);

                // Copy to output buffer
                Array.Copy(tmpBuffer, 0, buffer, offset, tmpBuffer.Length);
                offset += tmpBuffer.Length;
            }
        }

        /// <summary>
        /// Deserializes a DateTime. 
        /// </summary>
        [MethodImpl((short)ExtendedMethodImplOptions.AggressiveInlining)]
        public static object ToDateTime(byte[] buffer, ref int offset)
        {
            return new DateTime((long)ToLong(buffer, ref offset));
        }

        /// <summary>
        /// Serializes a DateTime. 
        /// </summary>
        [MethodImpl((short)ExtendedMethodImplOptions.AggressiveInlining)]
        public static void FromDateTime(object o, byte[] buffer, ref int offset)
        {
            FromLong(((DateTime)o).Ticks, buffer, ref offset);
        }

        /// <summary>
        /// Deserializes a byte. 
        /// </summary>
        [MethodImpl((short)ExtendedMethodImplOptions.AggressiveInlining)]
        public static object ToByte(byte[] buffer, ref int offset)
        {
            return buffer[offset++];
        }


        /// <summary>
        /// Serializes a byte. 
        /// </summary>
        [MethodImpl((short)ExtendedMethodImplOptions.AggressiveInlining)]
        public static void FromByte(object o, byte[] buffer, ref int offset)
        {
            buffer[offset++] = (byte)o;
        }

        /// <summary>
        /// Deserializes a bool. 
        /// </summary>
        [MethodImpl((short)ExtendedMethodImplOptions.AggressiveInlining)]
        public static object ToBoolean(byte[] buffer, ref int offset)
        {
            byte temp = (byte)ToByte(buffer, ref offset);
            return (temp != (byte)0);
        }


        /// <summary>
        /// Serializes a bool. 
        /// </summary>
       [MethodImpl((short)ExtendedMethodImplOptions.AggressiveInlining)]
        public static void FromBoolean(object theBool, byte[] buffer, ref int offset)
        {
            if ((bool)theBool)
            {
                FromByte((byte)1, buffer, ref offset);
            }
            else
            {
                FromByte((byte)0, buffer, ref offset);
            }
        }


       /// <summary>
       /// Deserializes an IPEndPoint. 
       /// </summary>
        [MethodImpl((short)ExtendedMethodImplOptions.AggressiveInlining)]
        public static object ToIpEndPoint(byte[] buffer, ref int offset)
        {
            Byte id = (byte)ToByte(buffer, ref offset);

            if (id == (byte)CoderBytes.NULL)
                return null;
            else
            {
                string address = (string)ToString(buffer, ref offset);

                short port = (short)ToShort(buffer, ref offset);
                IPAddress _ipa = IPAddress.Parse(address);
                return new System.Net.IPEndPoint(_ipa, port);
            }
        }

        /// <summary>
        /// Serializes an IPEndPoint. 
        /// </summary>
        [MethodImpl((short)ExtendedMethodImplOptions.AggressiveInlining)]
        public static void FromIpEndPoint(object theIPE, byte[] buffer, ref int offset)
        {
            if (theIPE == null)
                CoderTypes.FromByte((byte)CoderBytes.NULL, buffer, ref offset);
            else
            {
                CoderTypes.FromByte((byte)CoderBytes.NOT_NULL, buffer, ref offset);
                System.Net.IPEndPoint ipe = (System.Net.IPEndPoint)theIPE;
                FromString(ipe.Address.ToString(), buffer, ref offset);
                FromShort((short)ipe.Port, buffer, ref offset);
            }
        }

        /// <summary>
        /// Deserializes an enum. 
        /// </summary>
        [MethodImpl((short)ExtendedMethodImplOptions.AggressiveInlining)]
        public static object ToEnum(byte[] buffer, ref int offset)
        {
            return Convert.ToInt32(ToByte(buffer, ref offset));
        }

        /// <summary>
        /// Serializes an enum. 
        /// </summary>
        [MethodImpl((short)ExtendedMethodImplOptions.AggressiveInlining)]
        public static void FromEnum(object theEnum, byte[] buffer, ref int offset)
        {
            FromByte(Convert.ToByte(theEnum), buffer, ref offset);
        }

        /// <summary>
        /// Deserializes a Hashtable. 
        /// </summary>
        [MethodImpl((short)ExtendedMethodImplOptions.AggressiveInlining)]
        public static object ToHashtable(byte[] buffer, ref int offset)
        {
            Byte id = (byte)ToByte(buffer, ref offset);

            if (id == (byte)CoderBytes.NULL)
                return null;
            else
            {
                int length = (int)ToInt(buffer, ref offset);
                Hashtable hashtable = new Hashtable();

                for (int i = 0; i < length; i++)
                {
                    Byte idKey = (byte)ToByte(buffer, ref offset);
                    Byte idValue = (byte)ToByte(buffer, ref offset);
                    
                    DecodeFunction fKey = (DecodeFunction)CoderTables.GetInstance().DecodeTable[idKey];
                    object k=fKey(buffer, ref offset);
                    DecodeFunction fValue = (DecodeFunction)CoderTables.GetInstance().DecodeTable[idValue];
                    object v=fValue(buffer, ref offset);

                    hashtable.Add(k, v);
                }

                return hashtable;
            }
        }

        /// <summary>
        /// Serializes a Hashtable. 
        /// </summary>
        [MethodImpl((short)ExtendedMethodImplOptions.AggressiveInlining)]
        public static void FromHashtable(object theHashtable, byte[] buffer, ref int offset)
        {
            if (theHashtable == null)
                CoderTypes.FromByte((byte)CoderBytes.NULL, buffer, ref offset);
            else
            {
                CoderTypes.FromByte((byte)CoderBytes.NOT_NULL, buffer, ref offset);

                Hashtable hashtable = (Hashtable)theHashtable;

                FromInt(hashtable.Count, buffer, ref offset);

                
                foreach (System.Collections.DictionaryEntry dE in hashtable)
                {
                    EncodeData eDataKey = (EncodeData)CoderTables.GetInstance().EncodeTable[dE.Key.GetType().FullName];
                    EncodeData eDataValue = (EncodeData)CoderTables.GetInstance().EncodeTable[dE.Value.GetType().FullName];

                    FromByte(eDataKey.id, buffer, ref offset);
                    FromByte(eDataValue.id, buffer, ref offset);

                    eDataKey.func(dE.Key, buffer, ref offset);
                    eDataValue.func(dE.Value, buffer, ref offset);
                }
            }
        }

        /// <summary>
        /// Deserializes an ArrayList. 
        /// </summary>
        [MethodImpl((short)ExtendedMethodImplOptions.AggressiveInlining)]
        public static object ToArrayList(byte[] buffer, ref int offset)
        {
            Byte id = (byte)ToByte(buffer, ref offset);

            if (id == (byte)CoderBytes.NULL)
                return null;
            else
            {
                int length = (int)ToInt(buffer, ref offset);
                ArrayList arrayList = new ArrayList();

                for (int i = 0; i < length; i++)
                {
                    object o = ToObject(buffer, ref offset);
                    arrayList.Add(o);
                }

                return arrayList;
            }
        }

        /// <summary>
        /// Serializes an ArrayList. 
        /// </summary>
        [MethodImpl((short)ExtendedMethodImplOptions.AggressiveInlining)]
        public static void FromArrayList(object theArrayList, byte[] buffer, ref int offset)
        {
            if (theArrayList == null)
                CoderTypes.FromByte((byte)CoderBytes.NULL, buffer, ref offset);
            else
            {
                CoderTypes.FromByte((byte)CoderBytes.NOT_NULL, buffer, ref offset);

                ArrayList arrayList = (ArrayList)theArrayList;

                FromInt(arrayList.Count, buffer, ref offset);

                foreach (object _o in arrayList)
                    FromObject(_o, buffer, ref offset);
            }
        }

        /// <summary>
        /// Deserializes an array. 
        /// </summary>
        [MethodImpl((short)ExtendedMethodImplOptions.AggressiveInlining)]
        public static object ToArray(byte[] buffer, ref int offset)
        {
            //int length = (int)ToInt(buffer, ref offset);
            //Byte id = (byte)ToByte(buffer, ref offset);
            ////String typeName = (string)ToString(buffer, ref offset);


            ////M2DecodeFunction f = (M2DecodeFunction)CoderTables.decodeTable[id];
            ////String typeName=f.Method.DeclaringType.FullName.Replace("MG_","");
            //string typeName = ((M2Bytes)id).ToString().Replace("_", ".");

            //M2DecodeFunction f = (M2DecodeFunction)CoderTables.decodeTable[id];

            //if (f != null)
            //{
            //    dynamic theArray = Array.CreateInstance(typesCache[typeName], length);

            //    for (int i = 0; i < length; i++)
            //    {
            //        theArray[i] = (dynamic)f(buffer, ref offset);
            //    }

            //    return theArray;
            //}
            //return null;


            Byte id = (byte)ToByte(buffer, ref offset);

            if (id == (byte)CoderBytes.NULL)
                return null;
            else
            {
                string typeName = (string)ToString(buffer, ref offset);
                Type t = FindInAssemblies(typeName);

                int length = (int)ToInt(buffer, ref offset);
                Array array = Activator.CreateInstance(t, new object[] { length }) as Array;

                EncodeData eData = (EncodeData)CoderTables.GetInstance().EncodeTable[array.GetType().GetElementType().FullName];
                DecodeFunction f = (DecodeFunction)CoderTables.GetInstance().DecodeTable[eData.id];

                for (int i = 0; i < length; i++)
                {
                    object o = f(buffer, ref offset);
                    array.SetValue(o,i);
                }

                return array;
            }
        }

        /// <summary>
        /// Serializes an array. 
        /// </summary>
        [MethodImpl((short)ExtendedMethodImplOptions.AggressiveInlining)]
        public static void FromArray(object theArray, byte[] buffer, ref int offset)
        {
            //string typeName;
            //EncodeData eData;

            //dynamic array = theArray;

            //FromInt(array.Length, buffer, ref offset);

            //typeName = theArray.GetType().GetElementType().FullName;

            //eData = (EncodeData)CoderTables.encodeTable[typeName];

            //if (eData == null)
            //{
            //    //Only happens if the Type is an abstract class. In this case the abstract class is not included in the cache
            //    typeName = array[0].GetType().FullName;
            //    eData = (EncodeData)CoderTables.encodeTable[typeName];
            //}

            ////FromString(typeName, buffer, ref offset);
            //FromByte((byte)eData.id, buffer, ref offset);
            //Type type = typesCache[typeName];

            //foreach (object o in array)
            //{
            //    if (o != null)
            //    {
            //        if (type.IsArray || type.IsEnum)
            //            eData = (EncodeData)CoderTables.encodeTable[o.GetType().BaseType.FullName];
            //        else
            //            eData = (EncodeData)CoderTables.encodeTable[typeName];
            //        if (eData != null)
            //            eData.func(o, buffer, ref offset);
            //    }
            //    else
            //    {
            //        //TODO
            //    }
            //}

            if (theArray == null)
                CoderTypes.FromByte((byte)CoderBytes.NULL, buffer, ref offset);
            else
            {
                CoderTypes.FromByte((byte)CoderBytes.NOT_NULL, buffer, ref offset);

                FromString(theArray.GetType().FullName, buffer, ref offset);

                var array = theArray as Array;

                FromInt(array.Length, buffer, ref offset);

                EncodeData eDataValue = (EncodeData)CoderTables.GetInstance().EncodeTable[array.GetType().GetElementType().FullName];

                IEnumerator ide = array.GetEnumerator();
                while (ide.MoveNext())
                {
                    eDataValue.func(ide.Current, buffer, ref offset);
                }
            }
        }

        /// <summary>
        /// Deserializes an int array. 
        /// </summary>
        [MethodImpl((short)ExtendedMethodImplOptions.AggressiveInlining)]
        public static object ToIntArray(byte[] buffer, ref int offset)
        {
            Byte id = (byte)ToByte(buffer, ref offset);

            if (id == (byte)CoderBytes.NULL)
                return null;
            else
            {
                int positions = (int)ToInt(buffer, ref offset);

                int[] IntA = new int[positions];
                for (int i = 0; i < positions; i++)
                    IntA[i] = (int)ToInt(buffer, ref offset);

                return IntA;
            }
        }

        /// <summary>
        /// Serializes an int array. 
        /// </summary>
        [MethodImpl((short)ExtendedMethodImplOptions.AggressiveInlining)]
        public static void FromIntArray(object theInt32Array, byte[] buffer, ref int offset)
        {
            if (theInt32Array == null)
                CoderTypes.FromByte((byte)CoderBytes.NULL, buffer, ref offset);
            else
            {
                CoderTypes.FromByte((byte)CoderBytes.NOT_NULL, buffer, ref offset);
                int[] theIntA = (int[])theInt32Array;
                FromInt(theIntA.Length, buffer, ref offset);
                foreach (int i in theIntA)
                    FromInt(i, buffer, ref offset);
            }
        }

        /// <summary>
        /// Deserializes an uint array. 
        /// </summary>
        [MethodImpl((short)ExtendedMethodImplOptions.AggressiveInlining)]
        public static object ToUIntArray(byte[] buffer, ref int offset)
        {
            Byte id = (byte)ToByte(buffer, ref offset);

            if (id == (byte)CoderBytes.NULL)
                return null;
            else
            {
                int positions = (int)ToInt(buffer, ref offset);

                uint[] IntA = new uint[positions];
                for (int i = 0; i < positions; i++)
                    IntA[i] = (uint)ToUInt(buffer, ref offset);

                return IntA;
            }
        }

        /// <summary>
        /// Serializes an uint array. 
        /// </summary>
        [MethodImpl((short)ExtendedMethodImplOptions.AggressiveInlining)]
        public static void FromUIntArray(object theUInt32Array, byte[] buffer, ref int offset)
        {
            if (theUInt32Array == null)
                CoderTypes.FromByte((byte)CoderBytes.NULL, buffer, ref offset);
            else
            {
                CoderTypes.FromByte((byte)CoderBytes.NOT_NULL, buffer, ref offset);
                uint[] theUIntA = (uint[])theUInt32Array;
                FromInt(theUIntA.Length, buffer, ref offset);
                foreach (uint i in theUIntA)
                    FromUInt(i, buffer, ref offset);
            }
        }

        /// <summary>
        /// Deserializes a short array. 
        /// </summary>
        [MethodImpl((short)ExtendedMethodImplOptions.AggressiveInlining)]
        public static object ToShortArray(byte[] buffer, ref int offset)
        {
            Byte id = (byte)ToByte(buffer, ref offset);

            if (id == (byte)CoderBytes.NULL)
                return null;
            else
            {
                int positions = (int)ToInt(buffer, ref offset);
                short[] ShortA = new short[positions];
                for (int i = 0; i < positions; i++)
                    ShortA[i] = (short)ToShort(buffer, ref offset);
                return ShortA;
            }
        }

        /// <summary>
        /// Serializes a short array. 
        /// </summary>
        [MethodImpl((short)ExtendedMethodImplOptions.AggressiveInlining)]
        public static void FromShortArray(object theShortArray, byte[] buffer, ref int offset)
        {
            if (theShortArray == null)
                CoderTypes.FromByte((byte)CoderBytes.NULL, buffer, ref offset);
            else
            {
                CoderTypes.FromByte((byte)CoderBytes.NOT_NULL, buffer, ref offset);
                short[] theShortA = (short[])theShortArray;
                FromInt(theShortA.Length, buffer, ref offset);

                foreach (short sh in theShortA)
                    FromShort(sh, buffer, ref offset);
            }
        }

        /// <summary>
        /// Deserializes an ushort array. 
        /// </summary>
        [MethodImpl((short)ExtendedMethodImplOptions.AggressiveInlining)]
        public static object ToUShortArray(byte[] buffer, ref int offset)
        {
            Byte id = (byte)ToByte(buffer, ref offset);

            if (id == (byte)CoderBytes.NULL)
                return null;
            else
            {
                int positions = (int)ToInt(buffer, ref offset);

                ushort[] uShortA = new ushort[positions];
                for (int i = 0; i < positions; i++)
                    uShortA[i] = (ushort)ToUShort(buffer, ref offset);

                return uShortA;
            }
        }

        /// <summary>
        /// Serializes an ushort array. 
        /// </summary>
        [MethodImpl((short)ExtendedMethodImplOptions.AggressiveInlining)]
        public static void FromUShortArray(object theUShortArray, byte[] buffer, ref int offset)
        {
            if (theUShortArray == null)
                CoderTypes.FromByte((byte)CoderBytes.NULL, buffer, ref offset);
            else
            {
                CoderTypes.FromByte((byte)CoderBytes.NOT_NULL, buffer, ref offset);
                ushort[] theUShortA = (ushort[])theUShortArray;
                FromInt(theUShortA.Length, buffer, ref offset);

                foreach (ushort ush in theUShortA)
                    FromUShort(ush, buffer, ref offset);
            }
        }

        /// <summary>
        /// Deserializes a long array. 
        /// </summary>
        [MethodImpl((short)ExtendedMethodImplOptions.AggressiveInlining)]
        public static object ToLongArray(byte[] buffer, ref int offset)
        {
            Byte id = (byte)ToByte(buffer, ref offset);

            if (id == (byte)CoderBytes.NULL)
                return null;
            else
            {
                int positions = (int)ToInt(buffer, ref offset);

                long[] LongA = new long[positions];
                for (int i = 0; i < positions; i++)
                    LongA[i] = (long)ToLong(buffer, ref offset);

                return LongA;
            }
        }

        /// <summary>
        /// Serializes a long array. 
        /// </summary>
        [MethodImpl((short)ExtendedMethodImplOptions.AggressiveInlining)]
        public static void FromLongArray(object theLongArray, byte[] buffer, ref int offset)
        {
            if (theLongArray == null)
                CoderTypes.FromByte((byte)CoderBytes.NULL, buffer, ref offset);
            else
            {
                CoderTypes.FromByte((byte)CoderBytes.NOT_NULL, buffer, ref offset);
                long[] theLongA = (long[])theLongArray;
                FromInt(theLongA.Length, buffer, ref offset);

                foreach (long l in theLongA)
                    FromLong(l, buffer, ref offset);
            }
        }

        /// <summary>
        /// Deserializes a float array. 
        /// </summary>
        [MethodImpl((short)ExtendedMethodImplOptions.AggressiveInlining)]
        public static object ToFloatArray(byte[] buffer, ref int offset)
        {
            Byte id = (byte)ToByte(buffer, ref offset);

            if (id == (byte)CoderBytes.NULL)
                return null;
            else
            {
                int positions = (int)ToInt(buffer, ref offset);

                float[] FloatA = new float[positions];
                for (int i = 0; i < positions; i++)
                    FloatA[i] = (float)ToFloat(buffer, ref offset);

                return FloatA;
            }
        }

        /// <summary>
        /// Serializes a long array. 
        /// </summary>
        [MethodImpl((short)ExtendedMethodImplOptions.AggressiveInlining)]
        public static void FromFloatArray(object theFloatArray, byte[] buffer, ref int offset)
        {
            if (theFloatArray == null)
                CoderTypes.FromByte((byte)CoderBytes.NULL, buffer, ref offset);
            else
            {
                CoderTypes.FromByte((byte)CoderBytes.NOT_NULL, buffer, ref offset);
                float[] theFloatA = (float[])theFloatArray;
                FromInt(theFloatA.Length, buffer, ref offset);

                foreach (float f in theFloatA)
                    FromFloat(f, buffer, ref offset);
            }
        }

        /// <summary>
        /// Deserializes a double array. 
        /// </summary>
        [MethodImpl((short)ExtendedMethodImplOptions.AggressiveInlining)]
        public static object ToDoubleArray(byte[] buffer, ref int offset)
        {
            Byte id = (byte)ToByte(buffer, ref offset);

            if (id == (byte)CoderBytes.NULL)
                return null;
            else
            {
                int positions = (int)ToInt(buffer, ref offset);

                double[] DoubleA = new double[positions];
                for (int i = 0; i < positions; i++)
                    DoubleA[i] = (double)ToDouble(buffer, ref offset);

                return DoubleA;
            }
        }

        /// <summary>
        /// Serializes a double array. 
        /// </summary>
        [MethodImpl((short)ExtendedMethodImplOptions.AggressiveInlining)]
        public static void FromDoubleArray(object theDoubleArray, byte[] buffer, ref int offset)
        {
            if (theDoubleArray == null)
                CoderTypes.FromByte((byte)CoderBytes.NULL, buffer, ref offset);
            else
            {
                CoderTypes.FromByte((byte)CoderBytes.NOT_NULL, buffer, ref offset);
                double[] theDoubleA = (double[])theDoubleArray;
                FromInt(theDoubleA.Length, buffer, ref offset);

                foreach (Double d in theDoubleA)
                    FromDouble(d, buffer, ref offset);
            }
        }

        /// <summary>
        /// Deserializes a char array. 
        /// </summary>
        [MethodImpl((short)ExtendedMethodImplOptions.AggressiveInlining)]
        public static object ToCharArray(byte[] buffer, ref int offset)
        {
            Byte id = (byte)ToByte(buffer, ref offset);

            if (id == (byte)CoderBytes.NULL)
                return null;
            else
            {
                int positions = (int)ToInt(buffer, ref offset);

                char[] CharA = new char[positions];
                for (int i = 0; i < positions; i++)
                    CharA[i] = (char)ToChar(buffer, ref offset);

                return CharA;
            }
        }

        /// <summary>
        /// Serializes a char array. 
        /// </summary>
        [MethodImpl((short)ExtendedMethodImplOptions.AggressiveInlining)]
        public static void FromCharArray(object theCharArray, byte[] buffer, ref int offset)
        {
            if (theCharArray == null)
                CoderTypes.FromByte((byte)CoderBytes.NULL, buffer, ref offset);
            else
            {
                CoderTypes.FromByte((byte)CoderBytes.NOT_NULL, buffer, ref offset);
                char[] theCharA = (char[])theCharArray;
                FromInt(theCharA.Length, buffer, ref offset);

                foreach (char c in theCharA)
                    FromChar(c, buffer, ref offset);
            }
        }

        /// <summary>
        /// Deserializes a string array. 
        /// </summary>
        [MethodImpl((short)ExtendedMethodImplOptions.AggressiveInlining)]
        public static object ToStringArray(byte[] buffer, ref int offset)
        {
            Byte id = (byte)ToByte(buffer, ref offset);

            if (id == (byte)CoderBytes.NULL)
                return null;
            else
            {
                int positions = (int)ToInt(buffer, ref offset);

                string[] StringA = new string[positions];
                for (int i = 0; i < positions; i++)
                    StringA[i] = (string)ToString(buffer, ref offset);

                return StringA;
            }
        }

        /// <summary>
        /// Serializes a string array. 
        /// </summary>
        [MethodImpl((short)ExtendedMethodImplOptions.AggressiveInlining)]
        public static void FromStringArray(object theStringArray, byte[] buffer, ref int offset)
        {
            if (theStringArray == null)
                CoderTypes.FromByte((byte)CoderBytes.NULL, buffer, ref offset);
            else
            {
                CoderTypes.FromByte((byte)CoderBytes.NOT_NULL, buffer, ref offset);
                string[] theStringA = (string[])theStringArray;
                FromInt(theStringA.Length, buffer, ref offset);

                foreach (string s in theStringA)
                    FromString(s, buffer, ref offset);
            }
        }

        /// <summary>
        /// Deserializes a DateTime array. 
        /// </summary>
        [MethodImpl((short)ExtendedMethodImplOptions.AggressiveInlining)]
        public static object ToDateTimeArray(byte[] buffer, ref int offset)
        {
            Byte id = (byte)ToByte(buffer, ref offset);

            if (id == (byte)CoderBytes.NULL)
                return null;
            else
            {
                int positions = (int)ToInt(buffer, ref offset);

                DateTime[] DateTimeA = new DateTime[positions];
                for (int i = 0; i < positions; i++)
                    DateTimeA[i] = (DateTime)ToDateTime(buffer, ref offset);

                return DateTimeA;
            }
        }

        /// <summary>
        /// Serializes a DateTime array. 
        /// </summary>
        [MethodImpl((short)ExtendedMethodImplOptions.AggressiveInlining)]
        public static void FromDateTimeArray(object theDateTimeArray, byte[] buffer, ref int offset)
        {
            if (theDateTimeArray == null)
                CoderTypes.FromByte((byte)CoderBytes.NULL, buffer, ref offset);
            else
            {
                CoderTypes.FromByte((byte)CoderBytes.NOT_NULL, buffer, ref offset);
                DateTime[] theDateTimeA = (DateTime[])theDateTimeArray;
                FromInt(theDateTimeA.Length, buffer, ref offset);

                foreach (DateTime d in theDateTimeA)
                    FromDateTime(d, buffer, ref offset);
            }
        }

        /// <summary>
        /// Deserializes a byte array. 
        /// </summary>
        [MethodImpl((short)ExtendedMethodImplOptions.AggressiveInlining)]
        public static object ToByteArray(byte[] buffer, ref int offset)
        {
            Byte id = (byte)ToByte(buffer, ref offset);

            if (id == (byte)CoderBytes.NULL)
                return null;
            else
            {
                byte[] byteArray = null;
                int length = (int)ToInt(buffer, ref offset);

                byteArray = new byte[length];

                for (int i = 0; i < length; i++)
                    byteArray[i] = buffer[offset++];

                return byteArray;
            }

        }

        /// <summary>
        /// Serializes a byte array. 
        /// </summary>
        [MethodImpl((short)ExtendedMethodImplOptions.AggressiveInlining)]
        public static void FromByteArray(object theByteArray, byte[] buffer, ref int offset)
        {
            if (theByteArray == null)
                CoderTypes.FromByte((byte)CoderBytes.NULL, buffer, ref offset);
            else
            {
                CoderTypes.FromByte((byte)CoderBytes.NOT_NULL, buffer, ref offset);

                byte[] byteArray = (byte[])theByteArray;

                FromInt(byteArray.Length, buffer, ref offset);

                foreach (byte b in byteArray)
                    buffer[offset++] = b;
            }
        }

        /// <summary>
        /// Deserializes a bool array. 
        /// </summary>
        [MethodImpl((short)ExtendedMethodImplOptions.AggressiveInlining)]
        public static object ToBooleanArray(byte[] buffer, ref int offset)
        {
            Byte id = (byte)ToByte(buffer, ref offset);

            if (id == (byte)CoderBytes.NULL)
                return null;
            else
            {
                bool[] boolArray = null;
                int length = (int)ToInt(buffer, ref offset);

                boolArray = new bool[length];

                for (int i = 0; i < length; i++)
                    boolArray[i] = (bool)ToBoolean(buffer, ref offset);

                return boolArray;
            }

        }

        /// <summary>
        /// Serializes a bool array. 
        /// </summary>
        [MethodImpl((short)ExtendedMethodImplOptions.AggressiveInlining)]
        public static void FromBooleanArray(object theBooleanArray, byte[] buffer, ref int offset)
        {
            if (theBooleanArray == null)
                CoderTypes.FromByte((byte)CoderBytes.NULL, buffer, ref offset);
            else
            {
                CoderTypes.FromByte((byte)CoderBytes.NOT_NULL, buffer, ref offset);
                bool[] boolArray = (bool[])theBooleanArray;

                FromInt(boolArray.Length, buffer, ref offset);

                foreach (bool b in boolArray)
                    FromBoolean(b, buffer, ref offset);
            }
        }

        /// <summary>
        /// Deserializes a generic type. 
        /// </summary>
        [MethodImpl((short)ExtendedMethodImplOptions.AggressiveInlining)]
        public static object ToGeneric(byte[] buffer, ref int offset)
        {

            string typeName = (string)ToString(buffer, ref offset);
            Type type = FindInAssemblies(typeName);

            object o = null;
            o = Activator.CreateInstance(type);

            //OLD WAY
            //FieldInfo[] fieldInfos = type.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);

            //NEW WAY
            List<FieldInfo> fieldInfos = new List<FieldInfo>();
            AssembliesManager.FindFields(fieldInfos, type);

            foreach (FieldInfo fieldInfo in fieldInfos)
            {
                Type t = fieldInfo.FieldType;
                
                EncodeData eData = (EncodeData)CoderTables.GetInstance().EncodeTable[t.FullName];
                DecodeFunction f = (DecodeFunction)CoderTables.GetInstance().DecodeTable[eData.id];
                fieldInfo.SetValue(o, f(buffer, ref offset));
            }
            return o;
        }

        /// <summary>
        /// Serializes a generic type. 
        /// </summary>
        [MethodImpl((short)ExtendedMethodImplOptions.AggressiveInlining)]
        public static void FromGeneric(object theGeneric, byte[] buffer, ref int offset)
        {
            FromString(theGeneric.GetType().FullName, buffer, ref offset);

            FieldInfo[] fieldInfos = theGeneric.GetType().GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);

            foreach (FieldInfo fieldInfo in fieldInfos)
            {
                object value=fieldInfo.GetValue(theGeneric);

                    EncodeData eData = null;

                    Type type = value.GetType();

                    if (type.IsEnum)
                        eData = (EncodeData)CoderTables.GetInstance().EncodeTable[type.BaseType.FullName];
                    else
                        eData = (EncodeData)CoderTables.GetInstance().EncodeTable[type.FullName];

                    if (eData != null)
                    {
                        eData.func(value, buffer, ref offset);
                    }
                    else
                    {
                        if (type.IsArray)
                        {
                            eData = (EncodeData)CoderTables.GetInstance().EncodeTable[type.BaseType.FullName];
                            eData.func(value, buffer, ref offset);
                        }

                        //GenericType
                        else if (type.IsGenericType && !(value is IEnumerable))
                        {
                            FromGeneric(value, buffer, ref offset);
                        }

                        //System.Collections.Generic
                        //stackoverflow.com/questions/2388081/check-if-an-object-is-a-generic-collection
                        else if (type.IsGenericType && (value is IEnumerable))
                        {

                            eData = (EncodeData)CoderTables.GetInstance().EncodeTable[type.GetGenericTypeDefinition().FullName];
                            if (eData != null)
                            {
                                eData.func(value, buffer, ref offset);
                            }
                            else
                                throw new NotSupportedException(type.FullName + " type is not supported");
                        }
                        //else it's not suposed to happen
                        else
                            throw new NotSupportedException(type.FullName + " type is not supported");
                    }
            }
        }

        /// <summary>
        /// Deserializes a Dictionary<>. 
        /// </summary>
        [MethodImpl((short)ExtendedMethodImplOptions.AggressiveInlining)]
        public static object ToDictionary(byte[] buffer, ref int offset)
        {
            Byte idKey = (byte)ToByte(buffer, ref offset);

            if (idKey == (byte)CoderBytes.NULL)
                return null;
            else
            {
                Type d1 = typeof(Dictionary<,>);

                Type[] typeArgs = new Type[2];
                typeArgs[0] = FindInAssemblies(CoderTables.GetInstance().M2Types[idKey]);

                Byte idValue = (byte)ToByte(buffer, ref offset);
                typeArgs[1] = FindInAssemblies(CoderTables.GetInstance().M2Types[idValue]);

                Type makeme = d1.MakeGenericType(typeArgs);

                int length = (int)ToInt(buffer, ref offset);
                IDictionary dictionary = Activator.CreateInstance(makeme, new object[] { length }) as IDictionary;

                EncodeData eDataKey, eDataValue = null;
                eDataKey = (EncodeData)CoderTables.GetInstance().EncodeTable[typeArgs[0].FullName];
                eDataValue = (EncodeData)CoderTables.GetInstance().EncodeTable[typeArgs[1].FullName];

                DecodeFunction fKey = (DecodeFunction)CoderTables.GetInstance().DecodeTable[eDataKey.id];
                DecodeFunction fValue = (DecodeFunction)CoderTables.GetInstance().DecodeTable[eDataValue.id];


                for (int i = 0; i < length; i++)
                {
                    object k = fKey(buffer, ref offset);
                    object v = fValue(buffer, ref offset);
                    dictionary.Add(k, v);
                }

                return dictionary;
            }
        }


        /// <summary>
        /// Serializes a Dictionary<>. 
        /// </summary>
        [MethodImpl((short)ExtendedMethodImplOptions.AggressiveInlining)]
        public static void FromDictionary(object theDictionary, byte[] buffer, ref int offset)
        {
            if (theDictionary == null)
                CoderTypes.FromByte((byte)CoderBytes.NULL, buffer, ref offset);
            else
            {
                var iDict = theDictionary as IDictionary;

                EncodeData eDataKey = (EncodeData)CoderTables.GetInstance().EncodeTable[theDictionary.GetType().GetGenericArguments()[0].FullName];
                FromByte((byte)eDataKey.id, buffer, ref offset);

                EncodeData eDataValue = (EncodeData)CoderTables.GetInstance().EncodeTable[theDictionary.GetType().GetGenericArguments()[1].FullName];
                FromByte((byte)eDataValue.id, buffer, ref offset);
                
                FromInt(iDict.Count, buffer, ref offset);

                IDictionaryEnumerator ide = iDict.GetEnumerator();
                while (ide.MoveNext())
                {
                    eDataKey.func(ide.Key, buffer, ref offset);
                    eDataValue.func(ide.Value, buffer, ref offset);
                }
            }
        }

        /// <summary>
        /// Deserializes a List<>. 
        /// </summary>
        [MethodImpl((short)ExtendedMethodImplOptions.AggressiveInlining)]
        public static object ToList(byte[] buffer, ref int offset)
        {
            Byte id = (byte)ToByte(buffer, ref offset);

            if (id == (byte)CoderBytes.NULL)
                return null;
            else
            {
                Type d1 = typeof(List<>);

                Type typeArg = FindInAssemblies(CoderTables.GetInstance().M2Types[id]);

                Type makeme = d1.MakeGenericType(typeArg);

                int length = (int)ToInt(buffer, ref offset);
                IList list = Activator.CreateInstance(makeme, new object[] { length }) as IList;

                DecodeFunction f = (DecodeFunction)CoderTables.GetInstance().DecodeTable[id];

                for (int i = 0; i < length; i++)
                {
                    list.Add(f(buffer, ref offset));
                }

                return list;
            }
        }

        /// <summary>
        /// Serializes a List<>
        [MethodImpl((short)ExtendedMethodImplOptions.AggressiveInlining)]
        public static void FromList(object theList, byte[] buffer, ref int offset)
        {
            if (theList == null)
                CoderTypes.FromByte((byte)CoderBytes.NULL, buffer, ref offset);
            else
            {
                IList iList = theList as IList;
                
                EncodeData eData = (EncodeData)CoderTables.GetInstance().EncodeTable[iList.GetType().GetGenericArguments()[0].FullName];
                FromByte((byte)eData.id, buffer, ref offset);

                FromInt(iList.Count, buffer, ref offset);

                IEnumerator ide = iList.GetEnumerator();
                while (ide.MoveNext())
                    eData.func(ide.Current, buffer, ref offset);
            }
        }

        /// <summary>
        /// Deserializes a MemoryStream. 
        /// </summary>
        [MethodImpl((short)ExtendedMethodImplOptions.AggressiveInlining)]
        public static object ToMemoryStream(byte[] buffer, ref int offset)
        {
            Byte id = (byte)ToByte(buffer, ref offset);

            if (id == (byte)CoderBytes.NULL)
                return null;
            else
            {
                string typeName = (string)ToString(buffer, ref offset);
                Type t = FindInAssemblies(typeName);

                //bool canRead = (bool)
				ToBoolean(buffer, ref offset);
                //bool canSeek = (bool)
				ToBoolean(buffer, ref offset);
                //bool canTimeout = (bool)
				ToBoolean(buffer, ref offset);
                bool canWrite= (bool)ToBoolean(buffer, ref offset);
                

                long length = (long)ToLong(buffer, ref offset);
                long position = (long)ToLong(buffer, ref offset);
                
                Stream stream = Activator.CreateInstance(t, new object[] { buffer,offset,(int)length,canWrite }) as Stream;
                stream.Position = position;
                offset += (int)length;

                return stream;
            }
        }

        /// <summary>
        /// Serializes a MemoryStream. 
        /// </summary>
        [MethodImpl((short)ExtendedMethodImplOptions.AggressiveInlining)]
        public static void FromMemoryStream(object theStream, byte[] buffer, ref int offset)
        {
            if (theStream == null)
                CoderTypes.FromByte((byte)CoderBytes.NULL, buffer, ref offset);
            else
            {
                CoderTypes.FromByte((byte)CoderBytes.NOT_NULL, buffer, ref offset);

                FromString(theStream.GetType().FullName, buffer, ref offset);

                Stream stream = theStream as Stream;
                long count = 0;

                FromBoolean(stream.CanRead, buffer, ref offset);
                FromBoolean(stream.CanSeek, buffer, ref offset);
                FromBoolean(stream.CanTimeout, buffer, ref offset);
                FromBoolean(stream.CanWrite, buffer, ref offset);
                FromLong(stream.Length, buffer, ref offset);
                FromLong(stream.Position, buffer, ref offset);

                while (count < stream.Length)
                {
                    buffer[offset++] = (byte)stream.ReadByte();
                    count++;
                }
            }
        }

        /// <summary>
        /// Deserializes a Stream. 
        /// </summary>
        [MethodImpl((short)ExtendedMethodImplOptions.AggressiveInlining)]
        public static object ToStream(byte[] buffer, ref int offset)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Serializes a Stream. 
        /// </summary>
        [MethodImpl((short)ExtendedMethodImplOptions.AggressiveInlining)]
        public static void FromStream(object theStream, byte[] buffer, ref int offset)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Deserializes a Marea.MareaAddress. 
        /// </summary>
        [MethodImpl((short)ExtendedMethodImplOptions.AggressiveInlining)]
        public static object ToMareaAddress(byte[] buffer, ref int offset)
        {
            return new MareaAddress((String)ToString(buffer, ref offset));
        }

        /// <summary>
        /// Serializes a Marea.MareaAddress. 
        /// </summary>
        [MethodImpl((short)ExtendedMethodImplOptions.AggressiveInlining)]
        public static void FromMareaAddress(object theMareaAddress, byte[] buffer, ref int offset)
        {
            MareaAddress mad = (MareaAddress)theMareaAddress;
            FromString(mad.ToString(), buffer, ref offset);
        }

        /// <summary>
        /// Deserializes an object. 
        /// </summary>
        [MethodImpl((short)ExtendedMethodImplOptions.AggressiveInlining)]
        public static object ToObject(byte[] buffer, ref int offset)
        {
            Byte id = (byte)ToByte(buffer, ref offset);

            if (id == (byte)CoderBytes.NULL)
                return null;
            else
            {
                if (id == (byte)CoderBytes.GENERIC)

                    return ToGeneric(buffer, ref offset);


                DecodeFunction f = (DecodeFunction)CoderTables.GetInstance().DecodeTable[id];
                return f(buffer, ref offset);
            }
        }

        /// <summary>
        /// Serializes an object. 
        /// </summary>
        [MethodImpl((short)ExtendedMethodImplOptions.AggressiveInlining)]
        public static void FromObject(object theObject, byte[] buffer, ref int offset)
        {
            if (theObject == null)
                CoderTypes.FromByte((byte)CoderBytes.NULL, buffer, ref offset);
            else
            {
                EncodeData eData = null;

                Type type = theObject.GetType();

                if (type.IsEnum)
                    eData = (EncodeData)CoderTables.GetInstance().EncodeTable[type.BaseType.FullName];
                else
                    eData = (EncodeData)CoderTables.GetInstance().EncodeTable[type.FullName];
                
                if (eData != null)
                {
                    FromByte(eData.id, buffer, ref offset);
                    eData.func(theObject, buffer, ref offset);
                }
                else
                {
                    if (type.IsArray)
                    {
                        eData = (EncodeData)CoderTables.GetInstance().EncodeTable[type.BaseType.FullName];
                        FromByte(eData.id, buffer, ref offset);
                        eData.func(theObject, buffer, ref offset);
                    }

                    //GenericType
                    else if (type.IsGenericType && !(theObject is IEnumerable))
                    {
                        FromByte((byte)CoderBytes.GENERIC, buffer, ref offset);
                        FromGeneric(theObject, buffer, ref offset);
                    }

                    //System.Collections.Generic
                    //stackoverflow.com/questions/2388081/check-if-an-object-is-a-generic-collection
                    else if (type.IsGenericType && (theObject is IEnumerable))
                    {

                        eData = (EncodeData)CoderTables.GetInstance().EncodeTable[type.GetGenericTypeDefinition().FullName];
                        if (eData != null)
                        {
                            FromByte(eData.id, buffer, ref offset);
                            eData.func(theObject, buffer, ref offset);
                        }
                        else
                            throw new NotSupportedException(type.FullName + " type is not supported");
                    }
                    //else it's not suposed to happen
                    else
                        throw new NotSupportedException(type.FullName + " type is not supported");
                }

            }
        }

        /// <summary>
        /// Finds a type by a given name. 
        /// </summary>
        [MethodImpl((short)ExtendedMethodImplOptions.AggressiveInlining)]
        private static Type FindInAssemblies(string strName)
        {
            Type type = null;

            if (typesCache.ContainsKey(strName))
                type = (Type)typesCache[strName];

            else
            {
                // First ocurrence, search and add to cache
                foreach (Assembly assembly in AppDomain.CurrentDomain.GetAssemblies())
                {
                    type = assembly.GetType(strName,false);
                    if (type != null) break;
                }
                typesCache[strName] = type; 
            }
            return type;
        }
    }
}