using System;

namespace EventSourcingDemo
{
    public class EmailChanged : Event
    {
        public EmailChanged(Guid id, string oldEmail, string newEmail)
        {
            EmployeeId = id;
            OldEmail = oldEmail;
            NewEmail = newEmail;
        }

        public Guid EmployeeId { get; set; }
        public string OldEmail { get; set; }
        public string NewEmail { get; set; }
    }
}