using System;

namespace DialectSoftware.Composition
{
    public interface IFactory
    {
        object Create(Type typeToCreate, System.Collections.IDictionary properties, params object[] args);
        object Create(Type typeToCreate, Type typeToReturn, System.Collections.IDictionary properties, params object[] args);
        TObject Create<TObject>(System.Collections.IDictionary properties, params object[] args);
        TInterface Create<TObject, TInterface>(System.Collections.IDictionary properties, params object[] args);
        object Wrap(object instance, Type typeToReturn, System.Collections.IDictionary properties);
        TInterface Wrap<TInterface>(object instance, System.Collections.IDictionary properties);
        bool TypeSupportsInterception(Type t);
        object Create(Type typeToCreate, params object[] args);
        object Create(Type typeToCreate, Type typeToReturn, params object[] args);
        TObject Create<TObject>(params object[] args);
        TInterface Create<TObject, TInterface>(params object[] args);
        object Wrap(object instance, Type typeToReturn);
        TInterface Wrap<TInterface>(object instance);
     }
}
