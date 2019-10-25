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
    public static string ToKebabCase(string pascalCasedString)
    {
      return Regex.Replace(pascalCasedString, "[a-z][A-Z]", m => $"{m.Value[0]}-{m.Value[1]}").ToLower();
    }

    public static string Humanize(string pascalCasedString)
    {
      return Regex.Replace(pascalCasedString, "[a-z][A-Z]", m => $"{m.Value[0]} {char.ToLower(m.Value[1])}");
    }

    public static string HumanizeTitle(string pascalCasedString)
    {
      return Regex.Replace(pascalCasedString, "[a-z][A-Z]", m => $"{m.Value[0]} {m.Value[1]}");
    }

    public static string ToPascalCase(string kebabCasedString)
    {
      return string.Join("", kebabCasedString.Split('-').Select(part => part.ToLower()).Select(part => part.Substring(0, 1).ToUpper() + part.Substring(1)));
    }
  }
}
