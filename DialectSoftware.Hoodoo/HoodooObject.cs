using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Remoting;
using System.Runtime.Remoting.Messaging;
using System.Reflection;
using DialectSoftware.Composition;

namespace DialectSoftware.Composition
{
    internal class HoodooObject<T> : IIntercept<T>
    {
        object realProxy;
        IRuntimeContext<Object> context;
        Dictionary<MethodInfo, Delegate> PTable;
        Dictionary<MethodInfo, Delegate> ITable;
        Dictionary<MethodInfo, Delegate[]> YTable;
        Dictionary<MethodInfo, Delegate[]> OTable;
        Action<T, IMethodCallMessage, Exception> Trace;

        internal HoodooObject(T proxy)
        {
            //Preempt
            PTable = new Dictionary<MethodInfo, Delegate>();
            //Interject
            ITable = new Dictionary<MethodInfo, Delegate>();
            //Predicate
            YTable = new Dictionary<MethodInfo, Delegate[]>();
            //Observe
            OTable = new Dictionary<MethodInfo, Delegate[]>();
            //Proxy
            Wrap(proxy);

        }

        /// <summary>
        /// Intercept and modify the value after a method invocation. But BEWARE! The internal state of the object has changed.
        /// You are violating everything OO holds sacred, you heathen, but that's what happens when you are a practitioner of Hoodoo.
        /// </summary>
        /// <param name="method">MethodInfo of the method on which you are attempting to interject</param>
        /// <param name="analog">A function delegate, the signature of which, must match the method on which you are attempting to interject</param>
        /// <returns>IIntercept&lt;T&gt;</returns>
        public IIntercept<T> Interject(System.Reflection.MethodInfo method, Delegate analog)
        {
            ITable.Add(method, analog);
            return this;
        }
        /// <summary>
        /// Intercept and modify the value before a method invocation. But BEWARE! The internal state of the object has NOT changed.
        /// You are violating everything OO holds sacred, you heathen, but that's what happens when you are a practitioner of Hoodoo. 
        /// </summary>
        /// <param name="method">MethodInfo of the method you are attempting to preempt</param>
        /// <param name="analog">A function delegate, the signature of which, must match the method are attempting to preempt</param>
        /// <returns>IIntercept&lt;T&gt;</returns>
        public IIntercept<T> Preempt(MethodInfo method, Delegate analog)
        {
            PTable.Add(method, analog);
            return this;
        }
        /// <summary>
        /// Prevent method invocation unless you desire it to be so.
        /// </summary>
        /// <param name="method">MethodInfo of the method which you are attempting to predicate</param>
        /// <param name="predicate">Predicate that tests for a condition to determine whether or not the method is to be executed</param>
        /// <param name="analog">A function delegate, the signature of which, must match the method, the execution of which, you are attempting to predicate as an alternative should your predicate return true</param>
        /// <returns>IIntercept&lt;T&gt;</returns>
        public IIntercept<T> Predicate(MethodInfo method, Func<T, MethodInfo, object[], bool> predicate, Delegate analog)
        {
            YTable.Add(method, new Delegate[] { predicate, analog });
            return this;
        }
        /// <summary>
        /// Watch keenly, yet dispassionately, as each method is invoked. Know what, where and when things happen in your system, though you may scarcely know why.
        /// </summary>
        /// <param name="method">MethodInfo of the method which you are attempting to observe</param>
        /// <param name="pre">Function to execute pre method invocation</param>
        /// <param name="post">Function to execute post method invocation</param>
        /// <returns>IIntercept&lt;T&gt;</returns>
        public IIntercept<T> Observe(MethodInfo method, Action<T, MethodInfo, object[]> pre, Action<T, MethodInfo, object> post)
        {
            OTable.Add(method, new Delegate[] { pre, post });
            return this;
        }
        /// <summary>
        /// Thwart those who would dare raise unhandled exceptions from behind the thin vail of obligation that is the interface.
        /// Trap and record their transgressions.
        /// </summary>
        /// <param name="method">MethodInfo of the method which you are attempting to trap</param>
        /// <param name="action">Action to take in the event of an unhandled exception</param>
        /// <returns>IIntercept&lt;T&gt;</returns>
        public IIntercept<T> Trap(MethodInfo method, Action<T, IMethodCallMessage, Exception> action)
        {
            Trace = action;
            return this;
        }
        /// <summary>
        /// Yield the value you seek.
        /// </summary>
        /// <returns>T</returns>
        public T Cast()
        {
            return (T)realProxy;
        }

        protected virtual void Wrap(T proxy)
        {
            realProxy = proxy;
            context = RemotingServices.GetRealProxy(realProxy) as IRuntimeContext<Object>;

            if (typeof(IRuntimeContext<Object>).IsAssignableFrom(context.GetType()))
            {
                ((IRuntimeContext<Object>)context).OnBeginInvokeEvent += new BeginInvokeEventHandler<Object>(BeginInvokeEvent);
                ((IRuntimeContext<Object>)context).OnInvokeEvent += new InvokeEventHandler<Object>(InvokeEvent);
                ((IRuntimeContext<Object>)context).OnEndInvokeEvent += new EndInvokeEventHandler<Object>(EndInvokeEvent);
                ((IRuntimeContext<Object>)context).OnInvokeErrorEvent += new InvokeErrorEventHandler<Object>(InvokeErrorEvent);
            }

        }

        #region Event Methods
        protected virtual void BeginInvokeEvent(object source, IMethodCallMessage message)
        {
            if (OTable.ContainsKey((System.Reflection.MethodInfo)message.MethodBase))
            {
                ((Action<T, MethodInfo, object[]>)OTable[(System.Reflection.MethodInfo)message.MethodBase][0])((T)this.context.Target, (System.Reflection.MethodInfo)message.MethodBase, message.Args);
                //OTable[(System.Reflection.MethodInfo)message.MethodBase][0].DynamicInvoke(this.context.Target, (System.Reflection.MethodInfo)message.MethodBase, message.Args);
            }
        }

        protected virtual IMethodReturnMessage InvokeEvent(object source, IMethodCallMessage message)
        {
            if (YTable.ContainsKey((System.Reflection.MethodInfo)message.MethodBase))
            {
                if (((Func<T, MethodInfo, object[], bool>)YTable[(System.Reflection.MethodInfo)message.MethodBase][0])((T)this.context.Target, (System.Reflection.MethodInfo)message.MethodBase, message.Args))
                {
                    return YTable[(System.Reflection.MethodInfo)message.MethodBase][1].ToMethodReturnMessage(message);
                }

            }

            if (PTable.ContainsKey((System.Reflection.MethodInfo)message.MethodBase))
            {
                return PTable[(System.Reflection.MethodInfo)message.MethodBase].ToMethodReturnMessage(message);
            }
            else
            {
                IMethodCallMessage callMessage = (IMethodCallMessage)message;
                return callMessage.ToMethodReturnMessage(this.context.Target);
            }
        }

        protected virtual void EndInvokeEvent(object source, IMethodCallMessage message, IMethodReturnMessage returnMessage)
        {
            if (ITable.ContainsKey((System.Reflection.MethodInfo)returnMessage.MethodBase))
            {
                if (returnMessage.Exception != null)
                {
                    returnMessage.GetType().GetField("_e", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic).SetValue(returnMessage, null);
                }
                returnMessage.GetType().GetField("_ret", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic).SetValue(returnMessage, ITable[(System.Reflection.MethodInfo)returnMessage.MethodBase].DynamicInvoke(message.Args));
            }

            if (OTable.ContainsKey((System.Reflection.MethodInfo)message.MethodBase))
            {
                ((Action<T, MethodInfo, object>)OTable[(System.Reflection.MethodInfo)message.MethodBase][1])((T)this.context.Target, (System.Reflection.MethodInfo)message.MethodBase, returnMessage.ReturnValue);
                //OTable[(System.Reflection.MethodInfo)message.MethodBase][1].DynamicInvoke(this.context.Target, (System.Reflection.MethodInfo)message.MethodBase, message);
            }

        }

        protected virtual void InvokeErrorEvent(object source, IMethodCallMessage message, IMethodReturnMessage returnMessage, System.Exception e)
        {

            if (Trace != null)
                Trace((T)this.context.Target, message, e);
            if (returnMessage != null)
                returnMessage.GetType().GetField("_e", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic).SetValue(returnMessage, e);
        }
        #endregion

    }
}


//if (VTable.ContainsKey((System.Reflection.MethodInfo)((System.Reflection.RuntimeMethodInfo)Message.MethodBase)))