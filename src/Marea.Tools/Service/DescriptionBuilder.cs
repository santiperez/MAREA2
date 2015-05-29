using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;

namespace Marea
{
    public class DescriptionBuilder
    {

        //Here we return idu information+ LocateService from SDU from the SDU type
        public ServiceDescription CreateDescription(Type SDUType)
        {
            ServiceDescription desc = new ServiceDescription();
            object[] objs;

            desc.Name = SDUType.FullName;
                        
            Type[] tt = SDUType.FindInterfaces(new TypeFilter(Filter), null);
            foreach (Type i in tt)
            {               
                objs = i.GetCustomAttributes(true);
                foreach(object o in objs) 
                {
                    if (o.GetType().FullName == "Marea.ServiceDefinitionAttribute")
                    {
                        BuildInterface(desc, i);
                        String txt = ((ServiceDefinitionAttribute)o).Text;
                        if (txt != null) desc.Description = txt;
                    }
                }
            }

            // Service implementation overriding
            objs = SDUType.GetCustomAttributes(true);
            foreach (object o in objs)
            {
                if (o.GetType().FullName == "Marea.DescriptionAttribute") // NOT SERVICE DEFINITION !!!
                {
                    String txt = ((DescriptionAttribute)o).Text;
                    if (txt != null) desc.Description = txt;
                }
            }

            return desc;
        }

        protected static bool Filter(Type t, object o)
        {
            return true;
        }

        protected void BuildInterface(ServiceDescription desc, Type i) 
        {
            List<VariableDescription> variables = new List<VariableDescription>();
            List<EventDescription> events = new List<EventDescription>();
            List<FunctionDescription> functions = new List<FunctionDescription>();
            List<FileDescription> files = new List<FileDescription>();

            PropertyInfo[] props = i.GetProperties();
            foreach (PropertyInfo prop in props)
            {               
                switch (GetPropertyName(prop))
                {
                    case "Marea.Variable":
                    case "Marea.Variable`1":
                        variables.Add(BuildVariable(prop));
                        break;                    
                    case "Marea.Event":
                    case "Marea.Event`1":
                        events.Add(BuildEvent(prop));
                        break;
                    case "Marea.File":
                    case "Marea.File`1":
                        files.Add(BuildFile(prop));
                        break;
                    default:
                        //TODO Podriamos hacer algo generico
                        break;
                }                
            }
            MethodInfo[] methods = i.GetMethods();
            foreach (MethodInfo method in methods)
            {
                if (!method.IsSpecialName)
                {
                    functions.Add(BuildFunction(desc, method));
                }
            }

            variables.AddRange(desc.variables);
            events.AddRange(desc.events);
            functions.AddRange(desc.functions);
            files.AddRange(desc.files);

            desc.variables = variables.ToArray();
            desc.events = events.ToArray();
            desc.functions = functions.ToArray();
            desc.files = files.ToArray();
        }

        protected String GetPropertyName(PropertyInfo prop)
        {
            if(prop.PropertyType.IsGenericType) {
                return prop.PropertyType.GetGenericTypeDefinition().FullName;
            } else {
                return prop.PropertyType.FullName;
            }
        }

        protected VariableDescription BuildVariable(PropertyInfo prop) 
        {
            VariableDescription v = new VariableDescription();
            v.Name = prop.Name;
            if (prop.PropertyType.IsGenericType)
            {
                v.Type = prop.PropertyType.GetGenericArguments()[0].FullName;                
            } 
            object[] objs = prop.GetCustomAttributes(true);

            v.Publish = false;

            foreach (object o in objs)
            {
                switch (o.GetType().FullName)
                {
                    case "Marea.DescriptionAttribute":
                        v.Description = ((DescriptionAttribute)o).Text;
                        break;
                    case "Marea.UnitAttribute":
                        v.Unit = ((UnitAttribute)o).Text;
                        break;
                    case "Marea.TypeAttribute":
                        if (v.Type == null)
                        {
                            v.Type = ((Marea.TypeAttribute)o).Text;
                        }
                        else
                        {
                            throw new Exception("Trying to override a compile-time Variable type with Type attribute");
                        }
                        break;
                    case "Marea.PublishAttribute":
                        v.Publish = true;
                        break;
                    default:
                        break;
                }
            }
            return v;
        }

        protected EventDescription BuildEvent(PropertyInfo prop)
        {
            EventDescription e = new EventDescription();
            e.Name = prop.Name;
            if (prop.PropertyType.IsGenericType)
            {
                e.Type = prop.PropertyType.GetGenericArguments()[0].FullName;
            } 
            object[] objs = prop.GetCustomAttributes(true);
            
            e.Publish=false;
            
            foreach (object o in objs)
            {
                switch (o.GetType().FullName)
                {
                    case "Marea.DescriptionAttribute":
                        e.Description = ((DescriptionAttribute)o).Text;
                        break;
                    case "Marea.UnitAttribute":
                        e.Unit = ((UnitAttribute)o).Text;
                        break;
                    case "Marea.TypeAttribute":
                        if (e.Type == null)
                        {
                            e.Type = ((Marea.TypeAttribute)o).Text;
                        }
                        else
                        {
                            throw new Exception("Trying to override a compile-time Event type with Type attribute");
                        }
                        break;
                    case "Marea.PublishAttribute":
                        e.Publish = true;
                        break;
                    default:
                        break;
                }
            }
            return e;
        }

        protected FileDescription BuildFile(PropertyInfo prop)
        {
            FileDescription f = new FileDescription();
            f.Name = prop.Name;
            if (prop.PropertyType.IsGenericType)
            {
                f.Type = prop.PropertyType.GetGenericArguments()[0].FullName;
            }            
            object[] objs = prop.GetCustomAttributes(true);

            foreach (object o in objs)
            {
                switch (o.GetType().FullName)
                {
                    case "Marea.DescriptionAttribute":
                        f.Description = ((DescriptionAttribute)o).Text;
                        break;
                    case "Marea.TypeAttribute":
                        if (f.Type == null)
                        {
                            f.Type = ((Marea.TypeAttribute)o).Text;
                        }
                        else
                        {
                            throw new Exception("Trying to override a compile-time File type with Type attribute");
                        }
                        break;
                    default:
                        break;
                }
            }
            return f;
        }


        protected FunctionDescription BuildFunction(ServiceDescription desc, MethodInfo method)
        {
            FunctionDescription f = new FunctionDescription();
            List<ParameterDescription> list = new List<ParameterDescription>();

            f.Name = method.Name;
            f.ReturnType = method.ReturnParameter.ParameterType.FullName;

            object[] objs = method.GetCustomAttributes(true);
            foreach (object o in objs)
            {
                switch (o.GetType().FullName)
                {
                    case "Marea.DescriptionAttribute":
                        f.Description = ((DescriptionAttribute)o).Text;
                        break;
                    default:
                        break;
                }
            }

            ParameterInfo[] param = method.GetParameters();
            foreach (ParameterInfo p in param)
            {
                list.Add(BuildParameter(p));
            }
            f.parameters = list.ToArray();

            return f;
        }

        protected ParameterDescription BuildParameter(ParameterInfo p)
        {
            ParameterDescription desc = new ParameterDescription();
            desc.Name = p.Name;
            desc.Type = p.ParameterType.FullName;

            object[] objs = p.GetCustomAttributes(true);
            foreach (object o in objs)
            {
                switch (o.GetType().FullName)
                {
                    case "Marea.DescriptionAttribute":
                        desc.Description = ((DescriptionAttribute)o).Text;
                        break;
                    default:
                        break;
                }
            }

            return desc;
        }
    }
}
