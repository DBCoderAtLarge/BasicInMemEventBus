using KesselRun.EventLockAndLoad.Event;

namespace BasicInMemEventBus
{
    public interface IEventBus
    {
        void RaiseEvent(IEvent evt);
    }
}
