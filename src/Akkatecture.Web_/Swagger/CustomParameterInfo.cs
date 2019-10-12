// Decompiled with JetBrains decompiler
// Type: Akkatecture.Web.Swagger.CustomParameterInfo
// Assembly: Akkatecture.Web, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 043A546D-C869-4CCE-8BB0-8D9655232ADF
// Assembly location: C:\Users\naych\source\repos\!!!!!\netcoreapp2.2\Akkatecture.Web.dll

using System;
using System.Reflection;

namespace Akkatecture.Web.Swagger
{
  public class CustomParameterInfo : ParameterInfo
  {
    private readonly string _name;
    private readonly Type _type;

    public CustomParameterInfo(string name, Type type)
    {
      this._name = name;
      this._type = type;
    }

    public override string Name
    {
      get
      {
        return this._name;
      }
    }

    public override Type ParameterType
    {
      get
      {
        return this._type;
      }
    }
  }
}
