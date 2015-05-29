using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Marea
{
    public class PrimitiveServiceCache
    {
        public Variable<Object>[] variables;
        public Event<Object>[] events;

        public PrimitiveServiceCache(int variablesCount, int servicesCount)
        {
            variables= new Variable<object>[variablesCount];
            events= new Event<object>[variablesCount];
        }

        public void AddVariable<T>(Variable<T> variable)
        {
            variables.SetValue(variable, variables.Count() - 1);
        }

        public Variable<T> GetVariable<T>(MareaAddress primitiveMad)
        {
            return (VariableImpl<T>)variables.Where(v => v.Name == primitiveMad.GetPrimitiveAddress()).First();
        }
    }
}
