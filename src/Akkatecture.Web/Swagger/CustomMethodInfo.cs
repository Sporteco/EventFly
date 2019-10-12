// Decompiled with JetBrains decompiler
// Type: Akkatecture.Web.Swagger.CustomMethodInfo
// Assembly: Akkatecture.Web, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 043A546D-C869-4CCE-8BB0-8D9655232ADF
// Assembly location: C:\Users\naych\source\repos\!!!!!\netcoreapp2.2\Akkatecture.Web.dll

using System;
using System.Globalization;
using System.Reflection;

namespace Akkatecture.Web.Swagger
{
  public class CustomMethodInfo : MethodInfo
  {
    private readonly string _name;
    private readonly Type _parameterType;

    public CustomMethodInfo(string name, Type parameterType)
    {
      _name = name;
      _parameterType = parameterType;
    }

    public override object[] GetCustomAttributes(bool inherit)
    {
      return new object[0];
    }

    public override object[] GetCustomAttributes(Type attributeType, bool inherit)
    {
      return new object[0];
    }

    public override bool IsDefined(Type attributeType, bool inherit)
    {
      return false;
    }

    public override Type DeclaringType
    {
      get
      {
        return typeof (object);
      }
    }

    public override string Name
    {
      get
      {
        return _name;
      }
    }

    public override Type ReflectedType
    {
      get
      {
        return typeof (object);
      }
    }

    public override MethodImplAttributes GetMethodImplementationFlags()
    {
      return MethodImplAttributes.IL;
    }

    public override ParameterInfo[] GetParameters()
    {
      return new ParameterInfo[]{ new CustomParameterInfo("item", _parameterType) };
    }

    public override object Invoke(
      object obj,
      BindingFlags invokeAttr,
      Binder binder,
      object[] parameters,
      CultureInfo culture)
    {
      throw new NotImplementedException();
    }

    public override MethodAttributes Attributes
    {
      get
      {
        return MethodAttributes.Public;
      }
    }

    public override RuntimeMethodHandle MethodHandle
    {
      get
      {
        return new RuntimeMethodHandle();
      }
    }

    public override MethodInfo GetBaseDefinition()
    {
      return null;
    }

    public override ICustomAttributeProvider ReturnTypeCustomAttributes
    {
      get
      {
        throw new InvalidOperationException();
      }
    }
  }
}
