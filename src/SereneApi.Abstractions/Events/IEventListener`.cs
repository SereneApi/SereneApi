namespace SereneApi.Abstractions.Events
{
    public interface IEventListener<out TRef, out TVal> : IEventListener
    {
        TRef Reference { get; }

        TVal Value { get; }
    }
}
