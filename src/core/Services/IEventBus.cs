namespace Shop.Services
{
    public interface IEventBus
    {
        void Publish(object @event);
    }
}