# Event sourcing / CQRS demo

The purpose of this repository is to demonstrate the essence of what a typical Event Sourcing (ES) / CQRS architecture might look like. ES/CQRS can have many interpretations but this example is based on the typical patterns adopted by the DDD/CQRS and EventStore communities, normally using .NET

The domain of the demo itself is certainly not a good fit for an ES/CQRS architecture but that is not at all a goal of the demo.

## Event Sourcing
The essence of ES is that the state of a thing (object, aggregate, entity, system) is represented as a sequence of events such that the state of the thing can be recreated from said sequence.

The biggest strength of ES is that you have a history of every change to a system and will not lose information. In traditional systems that store the state itself, typically in an RDBMS, information is lost when updating or deleting rows.

### Implementation
Typical ES borrows terminology from the classic book Domain Driven Design (DDD) by Eric Evans. The unit of event sourcing is the _aggregate_. Each aggregate will have it's own sequence of events. When applications need to access data, it's usually pulled from a database. An RDBMS might use an ORM or a set of complex SQL statements to fetch the data needed to serve a request. In ES, in order to obtain an aggregate to operate on, the system needs to load the sequence of events and apply to some initial state of the aggregate.

After the aggregate (or aggregates) have been loaded, processing of the request continues and any changes to the aggregates are captured as new events. The new events are then written to the backing event storage.

### Event Storage
Events can be stored in an RDBMS, this is common because they are so prevalent and accepted. There are some useful libraries to support RDBMS event storage, most notably SqlStreamStore by Damian Hickey. The flagship is Event Store (geteventstore.org), designed, built and optimized for this sole purpose.

## A huge problem with ES
Some are concerned that loading an aggregate with many events can be slow. This is seldom the case, as they are normally organized sequentially on disk and load very fast. And compared to the complexity of an ORM loading multiple related entities from an RDBMS, ES, if implemented properly, will be significantly faster. And given ES only appends events there will be less contention due to competing reads and writes. 

So what's the huge problem?

    With event sourcing, it's practically impossible to query over the aggregates!

Let's say you want to find all orders with a total order sum exceeding a given amount. With ES as described above it would be necessary to load every single order aggregate

The normal solution here is to introduce read models. A read model is a structure (internal or external)designed to serve reads that cannot easily be served from the event sourced aggregates. 

What we have now is an example of CQRS, Command Query Responsibility Segregation as named by Greg Young.

So commands are used to modify data and handled by ES and queries are served primarily from one or more read models. A strength often argued here is that each model can be optimized for it's single responsibility.

## About the example
The example uses CQRS but without actual command and query objects. The key principle here is that writes are handled with ES, while queries are served primarily, but not necessarily, by read models. GetProductById is an example query that might as well be served with ES.

Key classes in the example:

* Employee -> The sole aggregate in this example.

* Event -> Abstract base class for events, nothing to see here.

* Events: EmployeeCreated, NameChanged, EmailChanged

* Aggregate -> Base class for aggregates, contains id and logic to load and reapply all events. The actual specific event handlers need to be on the concrete aggregate types, in this example Employee.

* ReadModel -> A readmodel with a list of employees sorted by email.

* MessageBus -> events are published to the bus. The readmodel subscribes and updates when new events arrive.

* EventStore -> an in-memory storage module. Read or append new events for a given aggregate. 