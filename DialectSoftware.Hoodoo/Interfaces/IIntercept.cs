using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.Runtime.Remoting.Messaging;

namespace DialectSoftware.Composition
{
    /// <summary>
    /// Conjure's Core Interface
    /// </summary>
    /// <typeparam name="T">Type of Interface to conjure from an object</typeparam>
    public interface IIntercept<T>
    {
        /// <summary>
        /// Intercept and modify the value after a method invocation. But BEWARE! The internal state of the object has changed.
        /// You are violating everything OO holds sacred, you heathen, but that's what happens when you are a practitioner of Hoodoo.
        /// </summary>
        /// <param name="method">MethodInfo of the method on which you are attempting to interject</param>
        /// <param name="analog">A function delegate, the signature of which, must match the method on which you are attempting to interject</param>
        /// <returns>IIntercept&lt;T&gt;</returns>
        IIntercept<T> Interject(MethodInfo method, Delegate analog);
        /// <summary>
        /// Intercept and modify the value before a method invocation. But BEWARE! The internal state of the object has NOT changed.
        /// You are violating everything OO holds sacred, you heathen, but that's what happens when you are a practitioner of Hoodoo. 
        /// </summary>
        /// <param name="method">MethodInfo of the method you are attempting to preempt</param>
        /// <param name="analog">A function delegate, the signature of which, must match the method are attempting to preempt</param>
        /// <returns>IIntercept&lt;T&gt;</returns>
        IIntercept<T> Preempt(MethodInfo method, Delegate analog);
        /// <summary>
        /// Prevent method invocation unless you desire it to be so.
        /// </summary>
        /// <param name="method">MethodInfo of the method which you are attempting to predicate</param>
        /// <param name="predicate">Predicate that tests for a condition to determine whether or not the method is to be executed</param>
        /// <param name="analog">A function delegate, the signature of which, must match the method, the execution of which, you are attempting to predicate as an alternative should your predicate return true</param>
        /// <returns>IIntercept&lt;T&gt;</returns>
        IIntercept<T> Predicate(MethodInfo method, Func<T, MethodInfo, object[], bool> predicate, Delegate analog);
        /// <summary>
        /// Watch keenly, yet dispassionately, as each method is invoked. Know what, where and when things happen in your system, though you may scarcely know why.
        /// </summary>
        /// <param name="method">MethodInfo of the method which you are attempting to observe</param>
        /// <param name="pre">Function to execute pre method invocation</param>
        /// <param name="post">Function to execute post method invocation</param>
        /// <returns>IIntercept&lt;T&gt;</returns>
        IIntercept<T> Observe(MethodInfo method, Action<T, MethodInfo, object[]> pre, Action<T, MethodInfo, object> post);
        /// <summary>
        /// Thwart those who would dare raise unhandled exceptions from behind the thin vail of obligation that is the interface.
        /// Trap and record their transgressions.
        /// </summary>
        /// <param name="method">MethodInfo of the method which you are attempting to trap</param>
        /// <param name="action">Action to take in the event of an unhandled exception</param>
        /// <returns>IIntercept&lt;T&gt;</returns>
        IIntercept<T> Trap(MethodInfo method, Action<T, IMethodCallMessage, Exception> action);
        /// <summary>
        /// Yield the value you seek.
        /// </summary>
        /// <returns>T</returns>
        T Cast();
    }
}