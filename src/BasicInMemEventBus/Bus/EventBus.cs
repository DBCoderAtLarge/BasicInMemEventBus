using KesselRun.EventLockAndLoad.Event;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BasicInMemEventBus
{
    public class EventBus : IEventBus
    {
        readonly Dictionary<Type, IEventHandler[]> _handlerCache = new Dictionary<Type, IEventHandler[]>();
        readonly GenericMethodActionBuilder<IEventHandler, IEvent> _actions = new GenericMethodActionBuilder<IEventHandler, IEvent>(typeof(IEventHandler<>), "Handle");
        readonly List<IEventHandler> _handlers;

        public EventBus()
        {
            _handlers = new List<IEventHandler>();
        }

        public void RaiseEvent(IEvent evt)
        {
            var action = GetAction(evt);
            var matchingHandlers = GetHandlers(evt);

            foreach (var handler in matchingHandlers)
            {
                action(handler, evt);
            }
        }

        public void Subscribe(IEventHandler handler)
        {
            _handlers.Add(handler);
        }

        public void Subscribe(IReadOnlyList<IEventHandler> handlers)
        {
            _handlers.AddRange(handlers);
        }

        public void UnSubscribe(IEventHandler handler)
        {
            _handlers.Remove(handler);
        }

        private Action<IEventHandler, IEvent> GetAction(IEvent evt)
        {
            return _actions.GetAction(evt);
        }

        private IEventHandler[] GetHandlers(IEvent evt)
        {
            var eventType = evt.GetType();

            if (_handlerCache.ContainsKey(eventType)) return _handlerCache[eventType];

            var eventHandlerType = typeof(IEventHandler<>).MakeGenericType(eventType);

            //  Method group conversion used in the where clause. Same as (l => eventHandlerType.IsInstanceOfType(l))
            var query = _handlers.Where(eventHandlerType.IsInstanceOfType);

            var handlers = query.ToArray();
                
            _handlerCache.Add(eventType, handlers);

            return _handlerCache[eventType];
        }
    }

}
