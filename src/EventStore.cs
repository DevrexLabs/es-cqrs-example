using System;
using System.Collections.Generic;
using System.Linq;

namespace EventSourcingDemo
{
    public static class EventStore
    {
        private static readonly Dictionary<Guid, List<Event>> Store 
            = new Dictionary<Guid, List<Event>>();


        public static void Append(Guid aggregateId, params Event[] newEvents)
        {
            if (!Store.TryGetValue(aggregateId, out var events))
            {
                events = new List<Event>();
            }
            events.AddRange(newEvents);
            Store[aggregateId] = events;
        }

        public static List<Event> GetEvents(Guid aggregateId)
        {
            if (!Store.TryGetValue(aggregateId, out var events))
            {
                events = new List<Event>();
            }

            return events;
        }

        public static IEnumerable<Event> AllEvents()
        {
            foreach (var events in Store.Values)
            {
                foreach (var @event in events)
                {
                    yield return @event;
                }
            }
        }
    }
}