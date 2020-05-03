using System;

namespace EventSourcingDemo
{
    public class EmployeeView
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
    }
}