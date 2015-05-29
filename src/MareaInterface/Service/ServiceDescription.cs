using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Marea
{
    public class ServiceDescription
    {
        public String Name;
        public String Description;
        public VariableDescription[] variables;
        public EventDescription[] events;
        public FunctionDescription[] functions;
        public FileDescription[] files;
        

        public ServiceDescription() 
        {
            variables = new VariableDescription[0];
            events = new EventDescription[0];
            functions = new FunctionDescription[0];
            files = new FileDescription[0];
        }

        public bool isEmpty()
        {
            if (variables.Length == 0 & events.Length==0 & functions.Length == 0 & files.Length == 0)
                return true;
            return false;
        }

        public bool HasVariable(String type, String name)
        {
            //variables.Any();
            foreach (VariableDescription v in variables)
            {
                if (v.Name == name)
                {
                    if (v.Type == type)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
            }
            return false;
        }

        public bool HasEvent(String type, String name)
        {            
            foreach (EventDescription e in events)
            {
                if (e.Name == name)
                {
                    if (e.Type == type)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
            }
            return false;
        }

        public bool HasFile(String type, String name)
        {
            foreach (FileDescription f in files)
            {
                if (f.Name == name)
                {
                    if (f.Type == type)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
            }
            return false;
        }

        public PrimitiveType GetTypeFromPrimitve(string primitiveName)
        {
            PrimitiveType primitiveType=PrimitiveType.Variable;
            if (events.Where(e => e.Name == primitiveName).Count() > 0)
                primitiveType = PrimitiveType.Event;
            if (files.Where(f => f.Name == primitiveName).Count() > 0)
                primitiveType = PrimitiveType.File;
            if (functions.Where(f => f.Name == primitiveName).Count() > 0)
                primitiveType = PrimitiveType.Invocation;

                return primitiveType;
        }
    }

    public class VariableDescription
    {
        public String Name;
        public String Description;
        public String Type;
        public String Unit;
        public bool Publish;
    }

    public class EventDescription
    {
        public String Name;
        public String Description;
        public String Type;
        public String Unit;
        public bool Publish;
    }

    public class FunctionDescription
    {
        public String Name;
        public String Description;
        public String ReturnType;
        public ParameterDescription[] parameters;
    }

    public class ParameterDescription
    {
        public String Name;
        public String Description;
        public String Type;
    }

    public class FileDescription
    {
        public String Name;
        public String Description;
        public String Type;
    }
}

