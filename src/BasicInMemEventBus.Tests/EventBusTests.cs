using BasicInMemEventBus.Tests.Domain.Events;
using BasicInMemEventBus.Tests.Domain.Handlers;
using FakeItEasy;
using KesselRun.EventLockAndLoad.Event;

namespace BasicInMemEventBus.Tests
{
    public class EventBusTests
    {
        
        [Fact]
        public void HandleMethodCalled()
        {
            IEventBus bus = new EventBus();

            var handler = A.Fake<IEventHandler<OrderCreatedEvent>>();

            bus.Subscribe(handler);

            var evt = new OrderCreatedEvent { Message = "Order was created" };

            bus.RaiseEvent(evt);

            A.CallTo(() => handler.Handle(A<OrderCreatedEvent>.Ignored)).MustHaveHappened();
        }
    }
}