using EventFly.Commands;
using System.ComponentModel;

namespace Demo.User.Commands
{
    [Description("Изменить заметки о пользователе")]
    public class ChangeUserNotesCommand : Command<UserId>
    {
        public System.String NewValue { get; }

        public ChangeUserNotesCommand(UserId aggregateId, System.String newValue) : base(aggregateId)
        {
            NewValue = newValue;
        }
    }
}