using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace EventSourcingDemo
{
    class ReadModel
    {
        public SortedList<String, EmployeeView> EmployeesByEmail
            = new SortedList<String, EmployeeView>();

        public static ReadModel Load()
        {
            var model = new ReadModel();
            foreach (var @event in EventStore.AllEvents())
            {
                model.Update(@event);
            }

            return model;
        }

        public void Update(Event e)
        {
            Apply((dynamic) e);
        }

        private void Apply(EmailChanged emailChanged)
        {
            var idx = EmployeesByEmail
                .IndexOfKey(emailChanged.OldEmail);
            
            var employee = EmployeesByEmail[emailChanged.OldEmail];
            EmployeesByEmail.RemoveAt(idx);
            employee.Email = emailChanged.NewEmail;
            EmployeesByEmail[emailChanged.NewEmail] = employee;
        }

        private void Apply(EmployeeCreated employeeCreated)
        {
            EmployeesByEmail.Add(employeeCreated.Email, 
                new EmployeeView
                {
                    Id = employeeCreated.Id,
                    Name = employeeCreated.Name,
                    Email = employeeCreated.Email
                });
        }
    }
}