using System;

namespace EventSourcingDemo
{
    public class NameChanged : Event
    {
        public readonly Guid EmployeeId;
        public readonly string OldName;
        public readonly string NewName;
        public NameChanged(Guid id, string oldName, string newMame)
        {
            EmployeeId = id;
            OldName = oldName;
            NewName = newMame;
        }
    }
}