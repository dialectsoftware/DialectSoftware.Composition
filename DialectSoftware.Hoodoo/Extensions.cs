using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.Runtime.Remoting.Messaging;

namespace DialectSoftware.Composition
{
    public static class Extensions
    {
        public static MethodInfo Method(this Type t, Predicate<MethodInfo> p)
        {
            return t.GetMethods().Where(method => p.Invoke(method)).Single();
        }

        public static PropertyInfo Property(this Type t, Predicate<PropertyInfo> p)
        {
            return t.GetProperties().Where(property => p.Invoke(property)).Single();
        }

        public static IMethodReturnMessage ToMethodReturnMessage(this Delegate @delegate, IMethodCallMessage callMessage)
        {
            try
            {
                object returnValue = @delegate.DynamicInvoke(callMessage.Args);
                return new ReturnMessage(returnValue, callMessage.Args, callMessage.ArgCount, callMessage.LogicalCallContext, callMessage);
            }
            catch (TargetInvocationException ex)
            {
                return new ReturnMessage(ex, callMessage);
            }
        }
    }
}
