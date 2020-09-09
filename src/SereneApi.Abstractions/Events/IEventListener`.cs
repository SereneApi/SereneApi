namespace SereneApi.Abstractions.Events
{
    public interface IEventListener<out T>: IEventListener
    {
        T Value { get; }
    }
}
