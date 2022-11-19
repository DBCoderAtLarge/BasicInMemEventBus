using System.Collections.Generic;

namespace KesselRun.EventLockAndLoad.Event
{
    public interface IEventSource
    {
        IEnumerable<IEvent> GetEvents();
        void Clear();
    }
}
