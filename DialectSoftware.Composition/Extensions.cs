using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.Remoting.Messaging;
using System.Reflection;

namespace DialectSoftware.Composition
{
    public static class Extensions
    {
        public static IMethodReturnMessage ToMethodReturnMessage(this IMethodCallMessage callMessage, object target)
        {
            try
            {
                object returnValue = callMessage.MethodBase.Invoke(target, callMessage.Args);
                return new ReturnMessage(returnValue, callMessage.Args, callMessage.ArgCount, callMessage.LogicalCallContext, callMessage);
            }
            catch (TargetInvocationException ex)
            {
                return new ReturnMessage(ex, callMessage);
            }
        }
    }
}
