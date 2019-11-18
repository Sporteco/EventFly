// Decompiled with JetBrains decompiler
// Type: EventFly.Web.Swagger.NamingUtils
// Assembly: EventFly.Web, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 043A546D-C869-4CCE-8BB0-8D9655232ADF
// Assembly location: C:\Users\naych\source\repos\!!!!!\netcoreapp2.2\EventFly.Web.dll

using System.Linq;
using System.Text.RegularExpressions;

namespace EventFly.Swagger
{
    public static class NamingUtils
    {
        public static System.String ToKebabCase(System.String pascalCasedString)
        {
            return Regex.Replace(pascalCasedString, "[a-z][A-Z]", m => $"{m.Value[0]}-{m.Value[1]}").ToLower();
        }

        public static System.String Humanize(System.String pascalCasedString)
        {
            return Regex.Replace(pascalCasedString, "[a-z][A-Z]", m => $"{m.Value[0]} {System.Char.ToLower(m.Value[1])}");
        }

        public static System.String HumanizeTitle(System.String pascalCasedString)
        {
            return Regex.Replace(pascalCasedString, "[a-z][A-Z]", m => $"{m.Value[0]} {m.Value[1]}");
        }

        public static System.String ToPascalCase(System.String kebabCasedString)
        {
            return System.String.Join("", kebabCasedString.Split('-').Select(part => part.ToLower()).Select(part => part.Substring(0, 1).ToUpper() + part.Substring(1)));
        }
    }
}
