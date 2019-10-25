// Decompiled with JetBrains decompiler
// Type: EventFly.Web.Swagger.CustomParameterInfo
// Assembly: EventFly.Web, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 043A546D-C869-4CCE-8BB0-8D9655232ADF
// Assembly location: C:\Users\naych\source\repos\!!!!!\netcoreapp2.2\EventFly.Web.dll

using System;
using System.Reflection;

namespace EventFly.Swagger
{
  public class CustomParameterInfo : ParameterInfo
  {
    private readonly string _name;
    private readonly Type _type;

    public CustomParameterInfo(string name, Type type)
    {
      _name = name;
      _type = type;
    }

    public override string Name => _name;

    public override Type ParameterType => _type;
  }
}
