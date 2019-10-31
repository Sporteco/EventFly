using EventFly.Commands;
using System;
using System.ComponentModel;

namespace Demo.User
{
    [Description("Изменить заметки о пользователе")]
    public class ChangeUserNotesCommand : Command<UserId>
    {
        public String NewValue { get; }

        public ChangeUserNotesCommand(UserId aggregateId, String newValue) : base(aggregateId)
        {
            NewValue = newValue;
        }
    }
}