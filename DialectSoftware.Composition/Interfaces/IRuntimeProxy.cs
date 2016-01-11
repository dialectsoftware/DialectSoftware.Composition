using System;
namespace DialectSoftware.Composition
{
    public interface IRuntimeProxy
    {
        T GetTransparentProxy<T>();
        String Uri { get; }
    }
}
