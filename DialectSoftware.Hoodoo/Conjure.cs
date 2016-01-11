using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using System.Runtime.Remoting;
using DialectSoftware.Composition;

namespace DialectSoftware.Composition
{
    /// <summary>
    /// Hoodoo Runtime Class
    /// </summary>
    public partial class Hoodoo : Runtime
    {
        static Hoodoo Instance
        {
            set;
            get;
        }

        static Hoodoo()
        {
            Instance = new Hoodoo();
        }

        /// <summary>
        /// Conjure the interface so that you may bend it to your will
        /// </summary>
        /// <typeparam name="T">Interface to conjure</typeparam>
        /// <typeparam name="U">Type of object to summon</typeparam>
        /// <returns>IIntercept&lt;T&gt;</returns>
        public static IIntercept<T> Conjure<T, U>() where U : new()
        {
            return new HoodooObject<T>((T)Instance.Create(typeof(U), new object[] { }));
        }

        /// <summary>
        /// Conjure the interface so that you may bend it to your will
        /// </summary>
        /// <typeparam name="T">Interface to conjure</typeparam>
        /// <typeparam name="U">Type of object to summon</typeparam>
        /// <param name="args">Arguments to provide to the constructor of type U</param>
        /// <returns>IIntercept&lt;T&gt;</returns>
        public static IIntercept<T> Conjure<T, U>(object[] args)
        {
            return new HoodooObject<T>((T)Instance.Create(typeof(U), args));
        }

        /// <summary>
        /// Conjure the interface so that you may bend it to your will
        /// </summary>
        /// <typeparam name="T">Interface to conjure</typeparam>
        /// <param name="Object">Object from which to conjure the interface</param>
        /// <returns>IIntercept&lt;T&gt;</returns>
        public static IIntercept<T> Conjure<T>(T Object)
        {
            return new HoodooObject<T>((T)Instance.DoWrap(Object, typeof(T)));
        }
    }
}
