using System.ComponentModel;
using EventFly.Commands;

namespace Demo.User.Commands
{
    [Description("Изменить заметки о пользователе")]
    public class ChangeUserNotesCommand : Command<UserId>
    {
        public string NewValue { get; }

        public ChangeUserNotesCommand(UserId aggregateId, string newValue) : base(aggregateId)
        {
            NewValue = newValue;
        }
    }
}