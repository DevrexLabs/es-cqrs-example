using System;
using System.Collections.Generic;
using System.Linq;

namespace EventSourcingDemo
{
    public static class MessageBus
    {
        private static readonly Dictionary<Type, List<Action<Event>>> Handlers
            = new Dictionary<Type, List<Action<Event>>>();

        public static void Publish(params Event[] events)
            => events.ToList().ForEach(Publish);
        public static void Publish(Event @event)
        {
            var eventType = @event.GetType();
            Handlers.ToList().ForEach(kvp =>
            {
                if (kvp.Key == eventType || eventType.IsSubclassOf(kvp.Key))
                    kvp.Value.ForEach(
                        handler => handler.Invoke(@event));
            });
        }

        public static void Subscribe<T>(Action<T> handler) where T : Event
        {
            if (!Handlers.TryGetValue(typeof(T), out var handlers))
            {
                handlers = new List<Action<Event>>();
            }
            handlers.Add(e => handler.Invoke((T)e));
            Handlers[typeof(T)] = handlers;
        }
    }
}