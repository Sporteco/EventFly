// Decompiled with JetBrains decompiler
// Type: Akkatecture.Web.Swagger.ReflectionExtensions
// Assembly: Akkatecture.Web, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 043A546D-C869-4CCE-8BB0-8D9655232ADF
// Assembly location: C:\Users\naych\source\repos\!!!!!\netcoreapp2.2\Akkatecture.Web.dll

using System;
using System.Collections.Generic;
using System.Linq;

namespace Akkatecture.Web.Swagger
{
  public static class ReflectionExtensions
  {
    public static Type GetSubclassOfRawGeneric(Type generic, Type toCheck)
    {
      for (; toCheck != (Type) null && toCheck != typeof (object); toCheck = toCheck.BaseType)
      {
        Type type = toCheck.IsGenericType ? toCheck.GetGenericTypeDefinition() : toCheck;
        if (generic == type)
          return toCheck;
      }
      return (Type) null;
    }

    public static Type GetSubclassOfRawGenericInterface(Type generic, Type toCheck)
    {
      return ((IEnumerable<Type>) toCheck.GetInterfaces()).FirstOrDefault<Type>((Func<Type, bool>) (i =>
      {
        if (i.IsGenericType)
          return i.GetGenericTypeDefinition() == generic;
        return false;
      }));
    }
  }
}
