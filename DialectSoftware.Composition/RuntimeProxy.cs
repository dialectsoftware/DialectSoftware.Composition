using System;
using System.Reflection;
using System.EnterpriseServices;
using System.Runtime.Remoting;
using System.Runtime.Remoting.Proxies;
using System.Runtime.Remoting.Messaging; 
using System.Security.Permissions;
using System.Collections;

namespace DialectSoftware.Composition
{
	/// <summary>
	/// Hosts the RuntimeManagedObject in the RuntimeContext.
	/// </summary>
	public abstract class RuntimeProxy:RealProxy,IRemotingTypeInfo,IDisposable,IRuntimeProxy
	{
        public object Target
        {
            get;
            private set;
        }

        public string TypeName
        {
            get;
            set;
        }

        public string Uri
        {
            get;
            private set;
        }

        public RuntimeProxy(Object target, Type type)
            : base(type)
        {
            // This constructor forwards the call to base RealProxy.
            // RealProxy uses the Type to generate a transparent proxy.
            Target = target;
            TypeName = target.GetType().FullName;
            Uri = String.Format("{0}/{1}", Guid.NewGuid().ToString(), Target.GetHashCode().ToString());
        }

        public T GetTransparentProxy<T>()
        {
            return (T)this.GetTransparentProxy(); 
        }

		public override IMessage Invoke(IMessage message)
		{
            IMethodCallMessage callMessage = (IMethodCallMessage)message;
            return callMessage.ToMethodReturnMessage(Target);
       }
        
        public bool CanCastTo(Type fromType, object o)
        {
            return fromType.IsAssignableFrom(o.GetType());
        }

		public void Dispose()
		{
			GC.SuppressFinalize(this);
            Dispose(false);
		}
		
		protected virtual void Dispose(bool disposing)
		{
       
		}

		~RuntimeProxy()
		{
			Dispose(true);
		    GC.Collect();
		}

    }
}
