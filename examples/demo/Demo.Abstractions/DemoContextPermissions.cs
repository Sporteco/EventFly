using System;
using System.ComponentModel;

namespace Demo
{
    public static class DemoContext
    {
        [Description("Создание пользователя")]
        public const String CreateUser = "Demo:CreateUser";

        [Description("Изменение пользователя")]
        public const String ChangeUser = "Demo:ChangeUser";

        public const String TestPermission = "Demo:TestPermission";
        public const String TestUserPermission = "Demo:TestUserPermission";

    }
}