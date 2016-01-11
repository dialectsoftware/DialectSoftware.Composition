using System;
using System.Reflection;
using System.Runtime.Remoting;
using System.EnterpriseServices;
using System.Runtime.Remoting.Proxies;
using System.Runtime.Remoting.Messaging;

/// ******************************************************************************************************************
/// * Copyright (c) 2011 Dialect Software LLC                                                                        *
/// * This software is distributed under the terms of the Apache License http://www.apache.org/licenses/LICENSE-2.0  *
/// *                                                                                                                *
/// ******************************************************************************************************************

namespace DialectSoftware.Composition
{
    public delegate void BeginInvokeEventHandler<T>(T Source, IMethodCallMessage Message);
    public delegate IMethodReturnMessage InvokeEventHandler<T>(T Source, IMethodCallMessage Message);
    public delegate void EndInvokeEventHandler<T>(T Source, IMethodCallMessage Message, IMethodReturnMessage returnMessage);
    public delegate void InvokeErrorEventHandler<T>(T Source, IMethodCallMessage Message, IMethodReturnMessage returnMessage, System.Exception e);
	public delegate void EventHandler<T>(T Source,System.Reflection.MethodBase Method);
	public delegate void ErrorHandler<T>(T Source,System.Reflection.MethodBase Method,System.Exception e);

}
