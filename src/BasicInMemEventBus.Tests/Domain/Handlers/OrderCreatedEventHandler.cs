using BasicInMemEventBus.Tests.Domain.Events;
using KesselRun.EventLockAndLoad.Event;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BasicInMemEventBus.Tests.Domain.Handlers
{
    public class OrderCreatedEventHandler : IEventHandler<OrderCreatedEvent>
    {
        public void Handle(OrderCreatedEvent orderSubmittedEvent)
        {
            
        }
    }
}
