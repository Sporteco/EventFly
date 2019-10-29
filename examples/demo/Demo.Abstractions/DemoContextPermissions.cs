using System.ComponentModel;

namespace Demo
{
    public static class DemoContext
    {
        [Description("Создание пользователя")]
        public const string CreateUser = "Demo:CreateUser";

        [Description("Изменение пользователя")]
        public const string ChangeUser = "Demo:ChangeUser";
    }
}