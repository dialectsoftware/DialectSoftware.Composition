using System;
using System.Reflection;
using System.Collections;
using System.Runtime.Remoting;
using System.EnterpriseServices;
using System.Security.Permissions;
using System.Runtime.Remoting.Proxies;
using System.Runtime.Remoting.Messaging;

/// ******************************************************************************************************************
/// * Copyright (c) 2011 Dialect Software LLC                                                                        *
/// * This software is distributed under the terms of the Apache License http://www.apache.org/licenses/LICENSE-2.0  *
/// *                                                                                                                *
/// ******************************************************************************************************************

namespace DialectSoftware.Composition
{
	public class RuntimeContext:RuntimeProxy, DialectSoftware.Composition.IRuntimeContext<Object> 
	{
		public event BeginInvokeEventHandler<Object> OnBeginInvokeEvent = null;
        public event InvokeEventHandler<Object> OnInvokeEvent = null;
		public event EndInvokeEventHandler<Object>  OnEndInvokeEvent = null;
		public event InvokeErrorEventHandler<Object> OnInvokeErrorEvent = null;

        public static String uriKey = "__Uri"; 
        protected IDictionary _properties = new System.Collections.Generic.Dictionary<object,object>();

        public RuntimeContext(object source):this(source, source.GetType())
		{   
            
        }

        public RuntimeContext(object source, Type type):base(source,type)
		{
            Properties.Add(uriKey, this.Uri); 
           
        }

       
        public RuntimeContext(object source, IDictionary properties):this(source)
		{   
            _properties = properties;
           
        }

        public RuntimeContext(object source, Type type, IDictionary properties):this(source,type)
		{   
            _properties = properties;
            
        }

      
		public IDictionary Properties
        {
            get{return _properties;}
        }
       
		/// <remarks>
		/// Forwards the Method calls to the ISecurityManager implementation
		/// by invoking the IAudit.RuntimeBeginInvokeEventHandler and the
		/// IAudit.RuntimeEndInvokeEventHandler. If an exception occurs
		/// the IAudit.RuntimeErrorInvokeEventHandler is Invoked. This
		/// gives the implementing ISecurityManager and opportunity to
		/// intercept, anaylze, and modify the incoming parameters and
		/// return values. 
		/// </remarks>
		/// <param name="Message"></param>
		/// <returns></returns>
		public override IMessage Invoke(IMessage Message)
		{
            IMethodReturnMessage returnMessage = null;
        	try
            {
                foreach(System.Collections.DictionaryEntry item in this.Properties)
                {
                   if(Message.Properties.Contains(item.Key)== false) 
                   {
                        Message.Properties.Add(item.Key,item.Value);               
                   }
                   else
                   {
                     Message.Properties[item.Key] = item.Value;
                   }
                }

                String ticks = Convert.ToBase64String(System.Text.ASCIIEncoding.ASCII.GetBytes(DateTime.Now.Ticks.ToString()));
                Message.Properties[uriKey] = String.Format("{0}/{1}", Message.Properties[uriKey].ToString(), ticks);

            
				if (OnBeginInvokeEvent != null)
                    OnBeginInvokeEvent(this.Target, (IMethodCallMessage)Message);

                if (OnInvokeEvent == null)
                {
                    returnMessage = (IMethodReturnMessage)base.Invoke(Message);
                }
                else
                {
                    returnMessage = OnInvokeEvent(this.Target, (IMethodCallMessage)Message);
                }


                if (OnEndInvokeEvent != null)
                    OnEndInvokeEvent(this.Target, (IMethodCallMessage)Message, returnMessage);


                if (returnMessage.Exception != null)
                {
                    throw returnMessage.Exception;
                }
				
			}
			catch(System.Exception e)
			{
				
				if(OnInvokeErrorEvent != null)
                {
                    OnInvokeErrorEvent(this.Target, (IMethodCallMessage)Message, (IMethodReturnMessage) returnMessage, e.InnerException == null ? e : e.InnerException);
                }
                else
                {
                    var message = e.InnerException == null ? e.Message : e.InnerException.Message;
                    System.Diagnostics.StackFrame stack = new System.Diagnostics.StackFrame();
				    e = new CompositionException(stack.GetMethod(),String.Format("{0}\r\n\t  {1}",message, stack.ToString()),e);
				    e = new CompositionException((IMethodCallMessage)Message,e.Message,e);
                    throw e;
                }
		
			}
            finally
            {
                
            }
            return returnMessage;
		}

		~RuntimeContext()
		{
				
		}

	}
	
}
