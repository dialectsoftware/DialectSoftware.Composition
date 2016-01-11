using System;
namespace FoundationLibrary
{
    public interface IComponent<T>
    {
        event EventHandler<T> OnActivate;
        event EventHandler<T> OnConstruct;
        event EventHandler<T> OnDeactivate;
        event ErrorHandler<T> OnError;
    }
}
