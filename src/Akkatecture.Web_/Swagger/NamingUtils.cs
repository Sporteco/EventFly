// Decompiled with JetBrains decompiler
// Type: Akkatecture.Web.Swagger.NamingUtils
// Assembly: Akkatecture.Web, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 043A546D-C869-4CCE-8BB0-8D9655232ADF
// Assembly location: C:\Users\naych\source\repos\!!!!!\netcoreapp2.2\Akkatecture.Web.dll

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Akkatecture.Web.Swagger
{
  public static class NamingUtils
  {
    public static string ToKebabCase(string pascalCasedString)
    {
      return Regex.Replace(pascalCasedString, "[a-z][A-Z]", (MatchEvaluator) (m => string.Format("{0}-{1}", (object) m.Value[0], (object) m.Value[1]))).ToLower();
    }

    public static string Humanize(string pascalCasedString)
    {
      return Regex.Replace(pascalCasedString, "[a-z][A-Z]", (MatchEvaluator) (m => string.Format("{0} {1}", (object) m.Value[0], (object) char.ToLower(m.Value[1]))));
    }

    public static string HumanizeTitle(string pascalCasedString)
    {
      return Regex.Replace(pascalCasedString, "[a-z][A-Z]", (MatchEvaluator) (m => string.Format("{0} {1}", (object) m.Value[0], (object) m.Value[1])));
    }

    public static string ToPascalCase(string kebabCasedString)
    {
      return string.Join("", ((IEnumerable<string>) kebabCasedString.Split('-')).Select<string, string>((Func<string, string>) (part => part.ToLower())).Select<string, string>((Func<string, string>) (part => part.Substring(0, 1).ToUpper() + part.Substring(1))));
    }
  }
}
