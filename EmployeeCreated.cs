using System;

namespace EventSourcingDemo
{
    public class EmployeeCreated : Event
    {
        public EmployeeCreated(Employee employee)
        {
            Id = employee.Id;
            Name = employee.Name;
            Email = employee.Email;
        }

        public readonly Guid Id;
        public readonly string Name;
        public readonly string Email;
    }
}