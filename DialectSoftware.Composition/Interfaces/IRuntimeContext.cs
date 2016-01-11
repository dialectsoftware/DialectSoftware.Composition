using System;
namespace DialectSoftware.Composition
{
    public interface IRuntimeContext<T>
    {
        object Target { get; }
        event BeginInvokeEventHandler<T> OnBeginInvokeEvent;
        event InvokeEventHandler<T> OnInvokeEvent;
        event EndInvokeEventHandler<T> OnEndInvokeEvent;
        event InvokeErrorEventHandler<T> OnInvokeErrorEvent;
    }
}
