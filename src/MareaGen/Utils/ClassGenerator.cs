using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Reflection;
using Marea;

namespace MareaGen
{
    /// <summary>
    /// Enum used to specify the modifier of the methods that are going to be created by MAREAGen.
    /// </summary>
    public enum AccessModifier { _public, _private, _protected, _internal, _internal_protected, _none };

    /// <summary>
    /// Cenerates classes automatically with methods to serialize and deserialize MAREA 
    /// entities.
    /// </summary>
    public class ClassGenerator
    {
        /// <summary>
        /// Writes into a StreamWriter the header part of the class (namespaces imported, name of the namespace, type of class:modifiers, static... and name of the class)
        /// </summary>
        public static void AddClassHeader(List<String> importedNamespaces, string nameSpace, AccessModifier accessModifier, bool isStatic, string className, ref StreamWriter writer)
        {
            //Add imported namespaces
            foreach (string iNameSpace in importedNamespaces)
                writer.WriteLine("using " + iNameSpace + ";");

            writer.WriteLine("");
            writer.WriteLine("[assembly: InitializeOnLoad(typeof("+className+"))]");

            //Set namespace if exits
            writer.WriteLine("");
            if (nameSpace != "")
            {
                writer.WriteLine("namespace " + nameSpace);
                writer.WriteLine("{");
            }

            //Create class header
            writer.Write("\t");
            if (accessModifier != AccessModifier._none)
                writer.Write(accessModifier.ToString().Remove(0, 1) + " ");
            if (isStatic)
                writer.Write("static ");
            writer.WriteLine("class " + className);
            writer.WriteLine("\t{");
            writer.WriteLine("");
        }

        /// <summary>
        ///Writes into a StreamWriter the header constructor of the class.
        /// </summary>
        public static void AddClassConstructor(string className,string typeName,byte id, ref StreamWriter writer)
        {
            writer.WriteLine("\t\tstatic "+className+"()");
            writer.WriteLine("\t\t{");
            writer.WriteLine("\t\t\tCoderTables.GetInstance().AddClass(typeof(" + typeName + "), " + id + ", " + className + ".Decode, " + className + ".Encode);");
            writer.WriteLine("\t\t}");
            writer.WriteLine("");
        }

        /// <summary>
        ///  Writes into a StreamWriter the end of the class. Closes the class and the namespace.
        /// </summary>
        public static void AddClassBottom(bool hasNameSpace, ref StreamWriter writer)
        {
            writer.WriteLine("\t}");
            if (hasNameSpace)
                writer.WriteLine("}");
        }

        /// <summary>
        ///  Writes into a StreamWriter the fingerprint. MAREAGen provides fingerprints in each of 
        ///  every generated class, derived from the type definition, in order to provide an unequivocal 
        ///  and fast way to detect code that had not been recompiled.
        /// </summary>
        public static void AddClassFingerprint(Type type, ref StreamWriter writer)
        {
            writer.WriteLine("\t\tpublic static readonly ulong MAREAGEN_FINGERPRINT= " + type.GetHashCode() + ";");
            writer.WriteLine("");
        }

        /// <summary>
        ///Writes into a StreamWriter a method to seralize the given type.
        /// </summary>
        public static void AddEncodeMethod(Type type, ref StreamWriter writer, bool inlineMethod)
        {
                string objectName = type.Name.ToLowerInvariant();

                //Method Header
                if(inlineMethod)
                    writer.WriteLine("\t\t[MethodImpl(MethodImplOptions.AggressiveInlining)]");
                writer.WriteLine("\t\tpublic static void Encode(object the" + type.Name + ", byte[] buffer, ref int offset)");
                writer.WriteLine("\t\t{");

                //Start if-else to check if object is null
                if (!type.IsValueType)
                {
                    writer.WriteLine("\t\t\tif(the" + type.Name + "==null)");
                    writer.WriteLine("\t\t\t\tCoderTypes.FromByte((byte)CoderBytes.NULL,buffer, ref offset);");
                    writer.WriteLine("\t\t\telse");
                    writer.WriteLine("\t\t\t{");
                    writer.WriteLine("\t\t\t\tCoderTypes.FromByte((byte)CoderBytes.NOT_NULL,buffer, ref offset);");
                }

                if (!type.IsValueType)
                    writer.Write("\t");

                writer.WriteLine("\t\t\t" + type.FullName + " " + objectName + " = (" + type.FullName + ") the" + type.Name + ";");

                //Get all the fields (inherited included)
                List<FieldInfo> fieldInfos = new List<FieldInfo>();
                AssembliesManager.FindFields(fieldInfos, type);

                foreach (FieldInfo fieldInfo in fieldInfos)
                {
                    if(!type.IsValueType)
                        writer.Write("\t");

                    _AddEncodeType(objectName, fieldInfo.FieldType, fieldInfo.Name, false, ref  writer);
                }

                //End if-else to check if object is null
                  if (!type.IsValueType)
                      writer.WriteLine("\t\t\t}");
      

                //Method Bottom
                writer.WriteLine("\t\t}");
                writer.WriteLine("");
        }

        /// <summary>
        ///Writes into a StreamWriter a method to seralize the given abstract or inherited type.
        /// </summary>
        public static void AddEncodeAbstractOrInheritedPropertyMethod(Type type, ref StreamWriter writer, bool inlineMethod)
        {
            bool otherCase = false;
            string objectName = type.Name.ToLowerInvariant();
            int id;
            
            //Method Header
            if (inlineMethod)
                writer.WriteLine("\t\t[MethodImpl(MethodImplOptions.AggressiveInlining)]");
            writer.WriteLine("\t\tpublic static void Encode(object the" + type.Name + ", byte[] buffer, ref int offset)");
            writer.WriteLine("\t\t{");
            writer.WriteLine("");

            writer.WriteLine("\t\t\tif(the"+type.Name+"==null)");
            writer.WriteLine("\t\t\t{");
            writer.WriteLine("\t\t\t\tCoderTypes.FromByte((byte)CoderBytes.NULL,buffer, ref offset);");
            writer.WriteLine("\t\t\t}");

            writer.WriteLine("\t\t\telse");
            writer.WriteLine("\t\t\t{");

            List<Type> subClasses = AssembliesManager.Instance.GetAllSubClassesFrom(type.FullName);

            //Method body
            foreach (Type t in subClasses)
            {
                id = CoderTables.GetInstance().M2Types.FirstOrDefault(x => x.Value == t.FullName).Key;

                if (otherCase)
                    writer.WriteLine("\t\t\t\telse if(the" + type.Name + ".GetType() == typeof(" + t.FullName + "))");
                else
                {
                    writer.WriteLine("\t\t\t\tif(the" + type.Name + ".GetType() == typeof(" + t.FullName + "))");
                    otherCase = true;
                }

                writer.WriteLine("\t\t\t\t{");
                writer.WriteLine("\t\t\t\t\tCoderTypes.FromByte((byte)"+id+" ,buffer, ref offset);");
                _AddEncodeType("the" + type.Name, t, null, true, ref  writer);

                writer.WriteLine("\t\t\t\t}");
            }

            //If it is a class that has subClasses (Inheritance)
            if (!type.IsAbstract)
            {
                writer.WriteLine("\t\t\t\telse");
                id = CoderTables.GetInstance().M2Types.FirstOrDefault(x => x.Value == type.FullName).Key;
                writer.WriteLine("\t\t\t\t{");
                writer.WriteLine("\t\t\t\t\tCoderTypes.FromByte((byte)" + id + " ,buffer, ref offset);");
                writer.WriteLine("\t\t\t\t\t" + type.FullName + " " + objectName + " = (" + type.FullName + ") the" + type.Name + ";");

                //Get all the fields (inherited included)
                List<FieldInfo> fieldInfos = new List<FieldInfo>();
                AssembliesManager.FindFields(fieldInfos, type);

                foreach (FieldInfo fieldInfo in fieldInfos)
                {
                    writer.Write("\t\t");
                    _AddEncodeType(objectName, fieldInfo.FieldType, fieldInfo.Name, false, ref  writer);
                }
                
                writer.WriteLine("\t\t\t\t}");
            }

            //End else
            writer.WriteLine("\t\t\t}");
            //Method Bottom
            writer.WriteLine("\t\t}");
            writer.WriteLine("");
        }

        private static void _AddEncodeType(string variableName, Type fieldType, string fieldName, bool isAbstract, ref StreamWriter writer)
        {
            if (fieldType.IsEnum)
            {
                if(isAbstract)
                    writer.WriteLine("\t\t\t\tCoderTypes.FromEnum(" + variableName + ", buffer, ref offset);");
                else
                    writer.WriteLine("\t\t\tCoderTypes.FromEnum(" + variableName + "." + fieldName + ", buffer, ref offset);");
            }
            else
            {
                EncodeData ec = (EncodeData)CoderTables.GetInstance().EncodeTable[fieldType.FullName];

                if (ec != null)
                {
                    string funcName = ec.func.Method.GetBaseDefinition().Name;
                    if (!funcName.Contains("Encode"))
                        if (isAbstract)
                            writer.WriteLine("\t\t\t\t\tCoderTypes." + funcName + "(" + variableName + ", buffer, ref offset);");
                        else
                            writer.WriteLine("\t\t\tCoderTypes." + funcName + "(" + variableName + "." + fieldName + ", buffer, ref offset);");
                    else
                        if (isAbstract)
                            writer.WriteLine("\t\t\t\t\tMG_" + fieldType.Name + "." + funcName + "(" + variableName + ", buffer, ref offset);");
                        else
                            writer.WriteLine("\t\t\tMG_" + fieldType.Name + "." + funcName + "(" + variableName+ "." + fieldName + ", buffer, ref offset);");
                }

                else if (fieldType.IsArray)
                {
                    if(isAbstract)
                        writer.WriteLine("\t\t\t\t\tCoderTypes.FromArray(" + variableName + ", buffer, ref offset);");
                    else
                        writer.WriteLine("\t\t\tCoderTypes.FromArray(" + variableName + "." + fieldName + ", buffer, ref offset);");
                }

                else if (fieldType.Name.Contains("Dictionary"))
                {
                    if (isAbstract)
                        writer.WriteLine("\t\t\t\tCoderTypes.FromDictionary(" + variableName + ", buffer, ref offset);");
                    else
                        writer.WriteLine("\t\t\tCoderTypes.FromDictionary(" + variableName + "." + fieldName + ", buffer, ref offset);");
                }
                else if (fieldType.Name.Contains("List"))
                {
                    if (isAbstract)
                        writer.WriteLine("\t\t\t\tCoderTypes.FromList(" + variableName + ", buffer, ref offset);");
                    else
                        writer.WriteLine("\t\t\tCoderTypes.FromList(" + variableName + "." + fieldName + ", buffer, ref offset);");
                }

                else if (fieldType.FullName.Contains("Marea"))
                {
                    if (isAbstract)
                    {
                        if (!fieldType.IsGenericType)
                            writer.WriteLine("\t\t\t\t\tMG_" + fieldType.Name + ".Encode(" + variableName + ", buffer, ref offset);");
                        else
                            writer.WriteLine("\t\t\tCoderTypes.FromGeneric(" + variableName + ", buffer, ref offset);");
                    }
                    else
                    {
                        if (!fieldType.IsGenericType)
                            writer.WriteLine("\t\t\tMG_" + fieldType.Name + ".Encode(" + variableName + "." + fieldName + ", buffer, ref offset);");
                        else
                            writer.WriteLine("\t\t\tCoderTypes.FromGeneric(" + variableName + "." + fieldName + ", buffer, ref offset);");
                    }
                }
                else
                    throw new NotImplementedException { };
            }

        }

        /// <summary>
        ///Writes into a StreamWriter a method to deseralize the given type.
        /// </summary>
        public static void AddDecodeMethod(Type type, ref StreamWriter writer, bool inlineMethod)
        {
            string objectName = type.Name.ToLowerInvariant();

            //Method Header
            if (inlineMethod)
                writer.WriteLine("\t\t[MethodImpl(MethodImplOptions.AggressiveInlining)]");
            writer.WriteLine("\t\tpublic static object Decode(byte[] buffer, ref int offset)");
            writer.WriteLine("\t\t{");

            //Start if to check if object is null
            if (!type.IsValueType)
            {
                writer.WriteLine("\t\t\tbyte id = (byte)CoderTypes.ToByte(buffer, ref offset);");
                writer.WriteLine("");
                writer.WriteLine("\t\t\tif(id ==(byte)CoderBytes.NULL)");
                writer.WriteLine("\t\t\t\treturn null;");
                writer.WriteLine("\t\t\telse");
                writer.WriteLine("\t\t\t{");
            }

            if (!type.IsValueType)
                writer.Write("\t");

            writer.WriteLine("\t\t\t" + type.FullName + " " + objectName + " = new " + type.FullName + "();");

            //Get all the fields (inherited included)
            List<FieldInfo> fieldInfos = new List<FieldInfo>();
            AssembliesManager.FindFields(fieldInfos, type);

            foreach (FieldInfo fieldInfo in fieldInfos)
            {
                if (!type.IsValueType)
                    writer.Write("\t");

                _AddDecodeType(objectName, fieldInfo.FieldType, fieldInfo.Name, false, ref writer);
            }

            if (!type.IsValueType)
                writer.Write("\t");
            writer.WriteLine("\t\t\treturn " + objectName + ";");

            //End if-else to check if object is null
            if (!type.IsValueType)
                writer.WriteLine("\t\t\t}");

            //Method Bottom
            writer.WriteLine("\t\t}");
            writer.WriteLine("");
        }

        /// <summary>
        ///Writes into a StreamWriter a method to deseralize the given abstract or inherited type.
        /// </summary>
        public static void AddDecodeAbstractOrInheritedPropertyMethod(Type type, ref StreamWriter writer,bool inlineMethod)
        {
            bool otherCase = false;
            string objectName = type.Name.ToLowerInvariant();

            //Method Header
            if (inlineMethod)
                writer.WriteLine("\t\t[MethodImpl(MethodImplOptions.AggressiveInlining)]");
            writer.WriteLine("\t\tpublic static object Decode(byte[] buffer, ref int offset)");
            writer.WriteLine("\t\t{");
            
            writer.WriteLine("\t\t\tbyte id = (byte)CoderTypes.ToByte(buffer, ref offset);");
            writer.WriteLine("");
            writer.WriteLine("\t\t\tif(id ==(byte)CoderBytes.NULL)");
            writer.WriteLine("\t\t\t\treturn null;");
            writer.WriteLine("\t\t\telse");
            writer.WriteLine("\t\t\t{");
            writer.WriteLine("\t\t\t\t" + type.FullName + " " + objectName + "=null;");

            List<Type> subClasses = AssembliesManager.Instance.GetAllSubClassesFrom(type.FullName);
            
            //Method body
            foreach (Type t in subClasses)
            {
                int id = CoderTables.GetInstance().M2Types.FirstOrDefault(x => x.Value == t.FullName).Key;

                if (otherCase)
                    writer.WriteLine("\t\t\t\telse if(id == (byte)" + id + ")");
                else
                {
                    writer.WriteLine("\t\t\t\tif(id == (byte)" + id + ")");
                    otherCase = true;
                }

                writer.WriteLine("\t\t\t\t{");
                _AddDecodeType(objectName, t, null, true, ref writer);

                writer.WriteLine("\t\t\t\t}");
            }
            
            //If it is a class that has subClasses (Inheritance)
            if (!type.IsAbstract)
            {
                writer.WriteLine("\t\t\t\telse");
                writer.WriteLine("\t\t\t\t{");
                writer.WriteLine("\t\t\t\t\t" + objectName + "=new " + type.FullName+"();");

                //Get all the fields (inherited included)
                List<FieldInfo> fieldInfos = new List<FieldInfo>();
                AssembliesManager.FindFields(fieldInfos, type);

                foreach (FieldInfo fieldInfo in fieldInfos)
                {
                    writer.Write("\t\t");
                    _AddDecodeType(objectName, fieldInfo.FieldType, fieldInfo.Name, false, ref writer);
                }
                writer.WriteLine("\t\t\t\t}");
            }
        
            writer.WriteLine("\t\t\t\treturn " + objectName + ";");
            
            //end else
            writer.WriteLine("\t\t\t}");
            //Method Bottom
            writer.WriteLine("\t\t}");
            writer.WriteLine("");
        }

        private static void _AddDecodeType(string variableName, Type fieldType, string fieldName, bool isAbstract, ref StreamWriter writer)
        {
            if (fieldType.IsEnum)
            {
                if(isAbstract)
                    writer.WriteLine("\t\t\t\t\t" + variableName + " = (" + fieldType.Name + ")CoderTypes.ToEnum(buffer, ref offset);");
                else
                    writer.WriteLine("\t\t\t" + variableName + "." + fieldName + " = (" + fieldType.Name + ")CoderTypes.ToEnum(buffer, ref offset);");
            }
            else
            {
                EncodeData ec = (EncodeData)CoderTables.GetInstance().EncodeTable[fieldType.FullName];

                if (ec != null)
                {
                    DecodeFunction f = (DecodeFunction)CoderTables.GetInstance().DecodeTable[ec.id];
                    string funcName = f.Method.GetBaseDefinition().Name;
                    if (!funcName.Contains("Decode"))
                        if (isAbstract)
                            writer.WriteLine("\t\t\t\t\t" + variableName + " = (" + fieldType.Name + ")CoderTypes." + funcName + "(buffer, ref offset);");
                        else
                            writer.WriteLine("\t\t\t" + variableName + "." + fieldName + " = (" + fieldType.Name + ")CoderTypes." + funcName + "(buffer, ref offset);");
                    else
                        if (isAbstract)
                            writer.WriteLine("\t\t\t\t\t" + variableName + " = (" + fieldType.Name + ")MG_" + fieldType.Name + "." + funcName + "(buffer, ref offset);");
                        else
                            writer.WriteLine("\t\t\t" + variableName + "." + fieldName + " = (" + fieldType.Name + ")MG_" + fieldType.Name + "." + funcName + "(buffer, ref offset);");
                }

                else if (fieldType.IsArray)
                {
                    if (isAbstract)
                        writer.WriteLine("\t\t\t\t\t" + variableName + " = (" + fieldType.Name + ")CoderTypes.ToArray(buffer, ref offset);");
                    else
                        writer.WriteLine("\t\t\t" + variableName + "." + fieldName + " = (" + fieldType.Name + ")CoderTypes.ToArray(buffer, ref offset);");
                }

                else if (fieldType.Name.Contains("Dictionary"))
                {
                    if (isAbstract)
                        writer.WriteLine("\t\t\t\t\t" + variableName + " = (Dictionary<" + fieldType.GetGenericArguments()[0].FullName + "," + fieldType.GetGenericArguments()[1].FullName + ">)CoderTypes.ToDictionary(buffer, ref offset);");
                    else
                        writer.WriteLine("\t\t\t" + variableName + "." + fieldName + " = (Dictionary<" + fieldType.GetGenericArguments()[0].FullName + "," + fieldType.GetGenericArguments()[1].FullName + ">)CoderTypes.ToDictionary(buffer, ref offset);");
                }

                else if (fieldType.Name.Contains("List"))
                {
                    if (isAbstract)
                        writer.WriteLine("\t\t\t\t\t" + variableName + " = (List<" + fieldType.GetGenericArguments()[0].FullName + ">)CoderTypes.ToList(buffer, ref offset);");
                    else
                        writer.WriteLine("\t\t\t" + variableName + "." + fieldName + " = (List<" + fieldType.GetGenericArguments()[0].FullName + ">)CoderTypes.ToList(buffer, ref offset);");
                }

                else if (fieldType.FullName.Contains("Marea"))
                {
                    if (isAbstract)
                    {
                        if (!fieldType.IsGenericType)
                        writer.WriteLine("\t\t\t\t\t" + variableName + " = (" + fieldType.Name + ")MG_" + fieldType.Name + ".Decode (buffer, ref offset);");
                        else
                        {
                            string genericTypeName=fieldType.FullName.Remove(fieldType.FullName.IndexOf('`'));
                            writer.WriteLine("\t\t\t\t\t" + variableName + " = ("+genericTypeName+"<" + fieldType.GetGenericArguments()[0].FullName + ">)CoderTypes.ToGeneric(buffer, ref offset);");
                        }
                    }
                    else
                    {
                        if (!fieldType.IsGenericType)
                            writer.WriteLine("\t\t\t" + variableName + "." + fieldName + " = (" + fieldType.Name + ")MG_" + fieldType.Name + ".Decode (buffer, ref offset);");
                        else
                        {
                            string genericTypeName = fieldType.FullName.Remove(fieldType.FullName.IndexOf('`'));
                            writer.WriteLine("\t\t\t" + variableName + "." + fieldName + " = (" + genericTypeName + "<" + fieldType.GetGenericArguments()[0].FullName + ">)CoderTypes.ToGeneric(buffer, ref offset);");

                        }
                    }
                }
                else
                    throw new NotImplementedException { };
            }
        }
    }
}
