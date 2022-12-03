using BasicInMemEventBus.Tests.Domain.Events;
using BasicInMemEventBus.Tests.Domain.Handlers;
using FakeItEasy;
using FluentAssertions;
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
        
        [Fact]
        public void ExpectedHandlerInvoked()
        {
            IEventBus bus = new EventBus();

            var handler = new OrderCreatedEventHandler();

            bus.Subscribe(handler);

            var evt = new OrderCreatedEvent();

            bus.RaiseEvent(evt);

            evt.Message.Should().Be("This got submitted");
        }
    }
}