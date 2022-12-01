using KesselRun.EventLockAndLoad.Event;

namespace BasicInMemEventBus
{
    public interface IEventBus
    {
        void Subscribe(IEventHandler handler);        
        void Subscribe(IReadOnlyList<IEventHandler> handlers);        
        void RaiseEvent(IEvent @event);
        void UnSubscribe(IEventHandler handler);
    }
}
