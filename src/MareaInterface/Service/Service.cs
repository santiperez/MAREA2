namespace Marea
{
    public abstract class Service : IService
    {
        protected IServiceContainer container;
        protected ServiceAddress id;

        public bool Start(IServiceContainer container, ServiceAddress id)
        {
            this.container = container;
            this.id = id;
            return Start();
        }

        public virtual bool Start() {
			return true;
		}
       
        public virtual bool Stop()
        {
            return true;
        }

		//TODO Test if it works!!
		public void Exit() {
			container.StopService (id);
		}

    }
}
