using System;
using System.Linq;

namespace EventSourcingDemo
{
    class Program
    {
        static void Main(string[] args)
        {
            
            //Rebuild the read model and hook it up to
            //receive future events
            var readModel = ReadModel.Load();
            MessageBus.Subscribe<Event>(e => readModel.Update(e));

            //Simulate a command
            var id = CreateEmployee("bart", "bart@simpsons.net");

            //Recreate an aggregate from events
            var bart = Load(id);
            Console.WriteLine(bart.Name);
            Console.WriteLine(bart.Email);
            
            //Issue another command
            ChangeEmail(id, "bart@dsimpsons.org");

            bart = Load(id);
            Console.WriteLine(bart.Name);
            Console.WriteLine(bart.Email);

            //Generate some data so we have something to query
            CreateManyEmployees();

            //Simulate a query that accesses the readmodel
            var page = 4;
            var itemsPerPage = 10;

            readModel.EmployeesByEmail
                .Values
                .Skip((page - 1) * itemsPerPage)
                .Take(itemsPerPage)
                .ToList()
                .ForEach(e => Console.WriteLine(e.Email));



        }

        private static void CreateManyEmployees()
        {
            for (int i = 0; i < 100; i++)
            {
                CreateEmployee("user" + i, "user" + i +  "@email.com");
            }
        }

        static Guid CreateEmployee(string name, string email)
        {
            var id = Guid.NewGuid();
            var employee = new Employee(id, name, email);
            
            var newEvents = employee.GetNewEvents();
            EventStore.Append(id, newEvents.ToArray());
            MessageBus.Publish(newEvents);
            return id;
        }

        //Typical write-side operation
        static void ChangeEmail(Guid id, String newEmail)
        {
            //Recreate current state from old events
            var employee = Load(id);
            
            //Call a mutating method, validation might throw
            employee.Email = newEmail;

            //Capture any events generated
            var events = employee.GetNewEvents();
            
            //might fail optimistic concurrency validation
            EventStore.Append(id, events);
            
            //No errors, broadcast events!
            MessageBus.Publish(events);
        }

        static Employee Load(Guid guid)
        {
            var employee = new Employee();
            var events = EventStore.GetEvents(guid);
            events.ForEach(e => employee.ApplyEvent(e));
            return employee;
        }
    }
}