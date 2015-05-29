using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Marea
{

    //TODO InvokeAction "asincrono" que no espere... podria ser el Message<int>

    /// <summary>
    /// Invoker that calls a function from an array of objects and returns the resulting object.
    /// </summary>
    public interface IInvoke
    {
        object Invoke(object[] objects);

    }

    /// <summary>
    /// Invoker that calls a procedure from an array of objects and does not return anything.
    /// </summary>
    class InvokeAction : IInvoke
    {
        public Action f=null;

        public object Invoke(object[] o)
        {
            f();
            return null;
        }
    }

    /// <summary>
    /// InvokeAction with 1 parameter.
    /// </summary>
    class InvokeAction<T1> : IInvoke
    {
        public Action<T1> f=null;

        public object Invoke(object[] o)
        {
            T1 t1 = (T1)o[0];
            f(t1);
            return null;
        }
    }

    /// <summary>
    /// InvokeAction with 2 parameters.
    /// </summary>
    class InvokeAction<T1, T2> : IInvoke
    {
        public Action<T1, T2> f=null;

        public object Invoke(object[] o)
        {
            T1 t1 = (T1)o[0];
            T2 t2 = (T2)o[1];
            f(t1, t2);
            return null;
        }
    }

    /// <summary>
    /// InvokeAction with 3 parameters.
    /// </summary>
	class InvokeAction<T1, T2, T3> : IInvoke
	{
		public Action<T1, T2, T3> f=null;

		public object Invoke(object[] o)
		{
			T1 t1 = (T1)o[0];
			T2 t2 = (T2)o[1];
			T3 t3 = (T3)o[2];
			f(t1, t2, t3);
			return null;
		}
	}

    /// <summary>
    /// InvokeAction with 4 parameters.
    /// </summary>
	class InvokeAction<T1, T2, T3, T4> : IInvoke
	{
		public Action<T1, T2, T3, T4> f=null;

		public object Invoke(object[] o)
		{
			T1 t1 = (T1)o[0];
			T2 t2 = (T2)o[1];
			T3 t3 = (T3)o[2];
			T4 t4 = (T4)o[3];
			f(t1, t2, t3, t4);
			return null;
		}
	}

    /// <summary>
    /// InvokeAction with 5 parameters.
    /// </summary>
	class InvokeAction<T1, T2, T3, T4, T5> : IInvoke
	{
		public Action<T1, T2, T3, T4, T5> f=null;

		public object Invoke(object[] o)
		{
			T1 t1 = (T1)o[0];
			T2 t2 = (T2)o[1];
			T3 t3 = (T3)o[2];
			T4 t4 = (T4)o[3];
			T5 t5 = (T5)o[4];
			f(t1, t2, t3, t4, t5);
			return null;
		}
	}

    /// <summary>
    /// InvokeFunc with return.
    /// </summary>
    class InvokeFunc<TR> : IInvoke
    {
        public Func<TR> f=null;

        public object Invoke(object[] o)
        {
            return f();
        }
    }

    /// <summary>
    /// InvokeFunc with return and 1 parameter.
    /// </summary>
    class InvokeFunc<T1, TR> : IInvoke
    {
        public Func<T1, TR> f=null;

        public object Invoke(object[] o)
        {
            T1 t1 = (T1)o[0];
            return f(t1);
        }
    }

    /// <summary>
    /// InvokeFunc with return and 2 parameters.
    /// </summary>
    class InvokeFunc<T1, T2, TR> : IInvoke
    {
        public Func<T1, T2, TR> f=null;

        public object Invoke(object[] o)
        {
            T1 t1 = (T1)o[0];
            T2 t2 = (T2)o[1];
            return f(t1, t2);
        }
    }

    /// <summary>
    /// InvokeFunc with return and 3 parameters.
    /// </summary>
	class InvokeFunc<T1, T2, T3, TR> : IInvoke
	{
		public Func<T1, T2, T3, TR> f=null;

		public object Invoke(object[] o)
		{
			T1 t1 = (T1)o[0];
			T2 t2 = (T2)o[1];
			T3 t3 = (T3)o[2];
			return f(t1, t2, t3);
		}
	}

    /// <summary>
    /// InvokeFunc with return and 4 parameters.
    /// </summary>
	class InvokeFunc<T1, T2, T3, T4, TR> : IInvoke
	{
		public Func<T1, T2, T3, T4, TR> f=null;

		public object Invoke(object[] o)
		{
			T1 t1 = (T1)o[0];
			T2 t2 = (T2)o[1];
			T3 t3 = (T3)o[2];
			T4 t4 = (T4)o[3];
			return f(t1, t2, t3, t4);
		}
	}

    /// <summary>
    /// InvokeFunc with return and 5 parameters.
    /// </summary>
	class InvokeFunc<T1, T2, T3, T4, T5, TR> : IInvoke
	{
		public Func<T1, T2, T3, T4, T5, TR> f=null;

		public object Invoke(object[] o)
		{
			T1 t1 = (T1)o[0];
			T2 t2 = (T2)o[1];
			T3 t3 = (T3)o[2];
			T4 t4 = (T4)o[3];
			T5 t5 = (T5)o[4];
			return f(t1, t2, t3, t4, t5);
		}
	}
}
