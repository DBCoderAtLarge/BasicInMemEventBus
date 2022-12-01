using KesselRun.EventLockAndLoad.Event;
using System;
using System.Linq;

namespace BasicInMemEventBus.Tests.Domain.Events
{
    public class OrderCreatedEvent : IEvent
    {
        public string Message { get; set; }
    }
}
