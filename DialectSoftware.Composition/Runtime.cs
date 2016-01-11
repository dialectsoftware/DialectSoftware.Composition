using System;
using System.Reflection;
using System.Collections;
using System.Runtime.Remoting;
using System.EnterpriseServices;
using System.Security.Permissions;
using System.Runtime.Remoting.Proxies;
using System.Runtime.Remoting.Messaging;
using DialectSoftware.Composition.Properties;

/// ******************************************************************************************************************
/// * Copyright (c) 2011 Dialect Software LLC                                                                        *
/// * This software is distributed under the terms of the Apache License http://www.apache.org/licenses/LICENSE-2.0  *
/// *                                                                                                                *
/// ******************************************************************************************************************

namespace DialectSoftware.Composition
{
	/// <summary>
	/// </summary>
    public class Runtime:IDisposable,DialectSoftware.Composition.IFactory 
    {
  
        #region Ctor and Versioning
            /// <remarks>
	        /// This static constructor loads and initializes the SecurityManager if one is implemented.
	        /// This allows auditing of and permission checks on object constructors.
	        /// </remarks>
	        public Runtime()
	        {

            }
        

            /// <remarks>
	        /// For implementation tracking purposes.
	        /// </remarks>
	        /// <returns>Returns the Assembly's full name.</returns>
	        public virtual string Version()
	        {
		        return Assembly.GetExecutingAssembly().FullName;
            }
        #endregion

        #region Runtime
            public bool TypeSupportsInterception(Type t)
            {
   	           if(!typeof(MarshalByRefObject).IsAssignableFrom(t) && !t.IsInterface)
               {
                   throw new ArgumentException(
                     string.Format(Resources.InterceptionNotSupported, t.Name),
                    "typeToReturn");
               }
               return true;
            }
         
            protected virtual object UnwrapTarget(object target)
            {
                if(RemotingServices.IsTransparentProxy(target))
                {
                     RuntimeProxy realProxy =
                        RemotingServices.GetRealProxy(target) as RuntimeProxy;
                    if( realProxy != null )
                    {
                        target = realProxy.Target;
                    }
                }
                return target;
            }

            protected virtual object DoWrap(object instance, Type typeToReturn)
            {
                TypeSupportsInterception(typeToReturn);
                RuntimeContext proxy = new RuntimeContext(UnwrapTarget(instance), typeToReturn);
                return proxy.GetTransparentProxy();
             }

            protected virtual object DoWrap(object instance, Type typeToReturn, IDictionary properties)
            {
                TypeSupportsInterception(typeToReturn);
                RuntimeContext proxy = new RuntimeContext(UnwrapTarget(instance), typeToReturn, properties);
                return proxy.GetTransparentProxy();
             }

            protected virtual void EnsureTypeIsInterceptable(Type typeToReturn)
            {
                if(!TypeSupportsInterception(typeToReturn))
                {
                    throw new ArgumentException(
                        string.Format(Resources.InterceptionNotSupported, typeToReturn.Name),
                        "typeToReturn");
                }
            }

            protected virtual object DoCreate(Type typeToCreate, Type typeToReturn, object[] arguments)
            {
                object target = Activator.CreateInstance(typeToCreate, arguments);
                return DoWrap(target, typeToReturn);
            }

            /// <summary>
            /// Creates the target object and appropriate interception when one of the Injector's Create methods is called.
            /// </summary>
            /// <remarks>Implementors can override this method if they need to control creation of the target object. 
            /// The base class implementation calls 
            /// <see cref="System.Activator.CreateInstance(Type, object[])"/> and then defers the interception
            /// to <see cref="PolicyInjector.DoWrap(object, Type, PolicySet)"/>.</remarks>
            /// <param name="typeToCreate"><see cref="System.Type"/> of target object to create.</param>
            /// <param name="typeToReturn">Type of the reference to return.</param>
            /// <param name="policiesForThisType">Policy set specific to typeToReturn.</param>
            /// <param name="arguments">Constructor parameters.</param>
            /// <returns>The new target object.</returns>
            protected virtual object DoCreate(Type typeToCreate, Type typeToReturn, IDictionary properties, object[] arguments)
            {
                object target = Activator.CreateInstance(typeToCreate, arguments);
                return DoWrap(target, typeToReturn, properties);
            }

            /// <summary>
            /// 
            /// </summary>
            /// <param name="instance"></param>
            /// <param name="typeToReturn"></param>
            /// <returns></returns>
            public object Wrap(object instance, Type typeToReturn)
            {
                EnsureTypeIsInterceptable(typeToReturn);
                return DoWrap(instance, typeToReturn);
            }

            /// <summary>
            /// Takes an existing object and returns a new reference that includes
            /// the policies specified in the current <see cref="PolicySet"/>.
            /// </summary>
            /// <param name="instance">The object to wrap.</param>
            /// <param name="typeToReturn">Type to return. This can be either an
            /// interface implemented by the object, or its concrete class.</param>
            /// <returns>A new reference to the object that includes the policies.</returns>
            public object Wrap(object instance, Type typeToReturn, IDictionary properties)
            {
                EnsureTypeIsInterceptable(typeToReturn);
                return DoWrap(instance, typeToReturn, properties);
            }

            /// <summary>
            /// 
            /// </summary>
            /// <typeparam name="TInterface"></typeparam>
            /// <param name="instance"></param>
            /// <returns></returns>
            public TInterface Wrap<TInterface>(object instance)
            {
                return (TInterface)Wrap(instance, typeof(TInterface));
            }

            /// <summary>
            /// Takes an existing object and returns a new reference that includes
            /// the policies specified in the current <see cref="PolicySet"/>.
            /// </summary>
            /// <typeparam name="TInterface">The type of wrapper to return. Can be either
            /// an interface implemented by the target instance or its entire concrete type.</typeparam>
            /// <param name="instance">The object to wrap.</param>
            /// <returns>A new reference to the object that includes the policies.</returns>
            public TInterface Wrap<TInterface>(object instance, IDictionary properties)
            {
                return (TInterface)Wrap(instance, typeof(TInterface), properties);
            }

            /// <summary>
            /// 
            /// </summary>
            /// <param name="typeToCreate"></param>
            /// <param name="args"></param>
            /// <returns></returns>
            public object Create(Type typeToCreate, params object[] args)
            {
                EnsureTypeIsInterceptable(typeToCreate);
                return DoCreate(typeToCreate, typeToCreate, args);
            }

            /// <summary>
            /// 
            /// </summary>
            /// <param name="typeToCreate"></param>
            /// <param name="typeToReturn"></param>
            /// <param name="args"></param>
            /// <returns></returns>
            public object Create(Type typeToCreate, Type typeToReturn, params object[] args)
            {
                EnsureTypeIsInterceptable(typeToCreate);
                return DoCreate(typeToCreate, typeToReturn, args);
            }

            /// <summary>
            /// Creates a new instance of typeToCreate and then applies policy.
            /// </summary>
            /// <param name="typeToCreate">The concrete type of the object to be created.</param>
            /// <param name="typeToReturn">The type of the reference to return. This can either
            /// be a concrete type, or the type of an interface that typeToCreate implements. If
            /// an interface type is specified, policy interception will only occur on calls
            /// to that interface.</param>
            /// <param name="args">Arguments to pass to the constructor.</param>
            /// <returns>The wrapped object instance of type typeToReturn.</returns>
            public object Create(Type typeToCreate, Type typeToReturn, IDictionary properties, params object[] args)
            {
                EnsureTypeIsInterceptable(typeToReturn);
                return DoCreate(typeToCreate, typeToReturn, properties, args);
            }

            /// <summary>
            /// Creates a new instance of typeToCreate and then applies policy.
            /// </summary>
            /// <param name="typeToCreate">The type of object to be created.</param>
            /// <param name="args">Arguments to be passed to the constructor.</param>
            /// <returns>The wrapped object instance of type typeToCreate.</returns>
            public object Create(Type typeToCreate, IDictionary properties, params object[] args)
            {
                return Create(typeToCreate, typeToCreate, properties, args);
            }

            /// <summary>
            /// 
            /// </summary>
            /// <typeparam name="TObject"></typeparam>
            /// <typeparam name="TInterface"></typeparam>
            /// <param name="args"></param>
            /// <returns></returns>
            public TInterface Create<TObject, TInterface>(params object[] args)
            {
                return (TInterface)Create(typeof(TObject), typeof(TInterface), args);
            }

            /// <summary>
            /// Creates new instance of type TObject and applies policy to it.
            /// </summary>
            /// <typeparam name="TObject">Type of object to create.</typeparam>
            /// <typeparam name="TInterface">Type of reference to return. If an interface type is
            /// specified here, policy is only applied to the methods of that interface.</typeparam>
            /// <param name="args">Constructor arguments.</param>
            /// <returns>A reference to the created object.</returns>
            public TInterface Create<TObject, TInterface>(IDictionary properties, params object[] args)
            {
                return (TInterface)Create(typeof(TObject), typeof(TInterface), properties, args);
            }

            /// <summary>
            /// 
            /// </summary>
            /// <typeparam name="TObject"></typeparam>
            /// <param name="args"></param>
            /// <returns></returns>
            public TObject Create<TObject>(params object[] args)
            {
                return (TObject)Create(typeof(TObject), typeof(TObject), args);
            }

            /// <summary>
            /// Creates new instance of type TObject and applies policy to it.
            /// </summary>
            /// <typeparam name="TObject">Type of object to create.</typeparam>
            /// <param name="args">Constructor arguments.</param>
            /// <returns>A reference to the created object.</returns>
            public TObject Create<TObject>(IDictionary properties, params object[] args)
            {
                return (TObject)Create(typeof(TObject), typeof(TObject), properties, args);
            }

        #endregion

        #region IDispose
            public void Dispose()
            {
                GC.SuppressFinalize(this);
                Dispose(false);
            }

            protected virtual void Dispose(bool disposing)
            {
                
            }

		    ~Runtime()
		    {
                Dispose(true);
			    GC.Collect();
            }
        #endregion

    }
	
}




	