using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Marea
{
    public class EventImpl<T> : Event<T>
    {
        protected class Subscription
        {
            public ServiceAddress id;
            public NotifyFunc<T> func;

            public Subscription(ServiceAddress id, NotifyFunc<T> func)
            {
                this.id = id;
                this.func = func;
            }

            //SOLUCION: A el problema de como crear RemoteService sin tener que generar codigo.
            //Mientras no haya covarianza. Se implementa una funcion Notify(object o ) y dentro 
            //se hace el cast. Hay otra clase SubscriptionGeneric que guarda NotifyFuncGeneric
            //en vez de NotifyFunc<T> pero la funcion Notify(object o) es igual.
            public void Notify(String name, object o)
            {
                func(name, (T)o);
            }

        }

        protected T value;
        public String Name { get; private set; }
        private String name;
        protected ServiceAddress provider;
        protected NotifyFunc<T> subscriptions;

        public EventImpl(ServiceAddress provider, String name)
        {
            this.provider = provider;
            this.Name = name;
            this.name = new MareaAddress(name).GetPrimitive();
        }

        public T Value
        {
            get
            {
                lock (this)
                {
                    return value;
                }
            }
        }

        public void Notify(ServiceAddress id, T value)
        {
            //CUIDADO "!=" NO SIRVE PORQUE NO SON EL MISMO OBJETO... SE TENDRIA Q MEJORAR
            if (!id.Equals(provider) && !provider.isQueryAddress())
            {
                throw new AccessViolationException("You have not rights to Notify() this variable!");
            }

            lock (this)
            {
                this.value = value;
            }

            if (subscriptions != null)
                subscriptions(id.GetServiceAddress()+"/"+name, value);
        }

        //TODO check visibility
        //public delegate void SubscriberChangeEventDelegate(Event<T> variable, bool isAdding, ServiceAddress id, NotifyFunc<T> notifyFunc, int totalSubscribers);
        //public SubscriberChangeEventDelegate addSubscriber;
        //public SubscriberChangeEventDelegate removeSubscriber;

        public ManageSubscriberFunc manageSubscriber;

        public void Subscribe(ServiceAddress id, NotifyFunc<T> func)
        {
            if (subscriptions == null)
                subscriptions = func;
            else
            {
                if (!subscriptions.GetInvocationList().Contains(func))
                subscriptions += func;
            }

            if (manageSubscriber != null)
                manageSubscriber(this, true, id, func, GetTotalSubscriptions());
        }

        public void Unsubscribe(ServiceAddress id, NotifyFunc<T> func)
        {
            if (subscriptions != null)
            {
                if (subscriptions.GetInvocationList().Contains(func))
                subscriptions -= func;
            }

            if (manageSubscriber != null)
                manageSubscriber(this, false, id, func, GetTotalSubscriptions());

        }

        public void AddSubscriber(ManageSubscriberFunc func)
        {
            if (manageSubscriber == null)
                manageSubscriber = func;
            else
                manageSubscriber += func;

        }
        public void RemoveSubscriber(ManageSubscriberFunc func)
        {
            if (manageSubscriber != null)
                manageSubscriber -= func;
        }

        public int GetTotalSubscriptions()
        {
            if (subscriptions != null)
                return subscriptions.GetInvocationList().Count();
            else
                return 0;
        }
    }
}

