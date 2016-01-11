using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.Remoting.Messaging;
using System.Reflection;

/// ******************************************************************************************************************
/// * Copyright (c) 2011 Dialect Software LLC                                                                        *
/// * This software is distributed under the terms of the Apache License http://www.apache.org/licenses/LICENSE-2.0  *
/// *                                                                                                                *
/// ******************************************************************************************************************

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
