using System;
using System.ComponentModel;
using EventFly.Commands;

namespace Demo.User.Commands
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