using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace EventSourcingDemo
{
    /// <summary>
    /// Base class for aggregates with some framework code
    /// </summary>
    public abstract class Aggregate
    {
        public Guid Id { get; protected set; }

        public void ApplyEvent(Event e)
        {
            var eventType = e.GetType();

            //ugly reflection hack to find a method
            //that will handle the event e
            //taking an event of the concrete type of e
            var handler = GetType()
                .GetMethod("Apply", new[] {eventType});

            if (handler == null)
                throw new Exception("No Apply method for type " + eventType.Name);
            handler.Invoke(this, new object[] {e});
        }

        
        protected List<Event> NewEvents { get; private set; }
            = new List<Event>();

        public Event[] GetNewEvents() => NewEvents.ToArray();
    }
}