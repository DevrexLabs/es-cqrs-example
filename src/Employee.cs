using System;

namespace EventSourcingDemo
{
    public class Employee : Aggregate
    {
        private string _name;
        private string _email;
        
        public Employee(Guid id, string name, string email)
        {
            Id = id;
            _name = name;
            _email = email;
            NewEvents.Add(new EmployeeCreated(this));
        }

        public Employee()
        {
            //An empty constructor is needed for hydration
        }

        public string Name
        {
            get => _name;
            set
            {
                NewEvents.Add(new NameChanged(Id, _name, value));
                _name = value;
            } 
        }

        public string Email
        {
            get => _email;
            set
            {
                //note no event is produced unless the operation is valid
                if (! value.Contains("@")) throw new Exception("Invalid email");
                NewEvents.Add(new EmailChanged(Id, _email, value));
                _email = value;
            }
        }

        public void Apply(EmailChanged emailChanged)
        {
            //Assign to the backing field to avoid creating a new event
            //during hydration
            _email = emailChanged.NewEmail;
        }

        public void Apply(EmployeeCreated employeeCreated)
        {
            Id = employeeCreated.Id;
            _name = employeeCreated.Name;
            _email = employeeCreated.Email;
        }
    }
}