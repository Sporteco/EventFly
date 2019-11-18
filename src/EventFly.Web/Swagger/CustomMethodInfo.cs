// Decompiled with JetBrains decompiler
// Type: EventFly.Web.Swagger.CustomMethodInfo
// Assembly: EventFly.Web, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 043A546D-C869-4CCE-8BB0-8D9655232ADF
// Assembly location: C:\Users\naych\source\repos\!!!!!\netcoreapp2.2\EventFly.Web.dll

using System;
using System.Globalization;
using System.Reflection;

namespace EventFly.Swagger
{
    public class CustomMethodInfo : MethodInfo
    {
        private readonly String _name;
        private readonly Type _parameterType;

        public CustomMethodInfo(String name, Type parameterType)
        {
            _name = name;
            _parameterType = parameterType;
        }

        public override Object[] GetCustomAttributes(Boolean inherit)
        {
            return new Object[0];
        }

        public override Object[] GetCustomAttributes(Type attributeType, Boolean inherit)
        {
            return new Object[0];
        }

        public override Boolean IsDefined(Type attributeType, Boolean inherit)
        {
            return false;
        }

        public override Type DeclaringType => typeof(Object);

        public override String Name => _name;

        public override Type ReflectedType => typeof(Object);

        public override MethodImplAttributes GetMethodImplementationFlags()
        {
            return MethodImplAttributes.IL;
        }

        public override ParameterInfo[] GetParameters()
        {
            return new ParameterInfo[] { new CustomParameterInfo("item", _parameterType) };
        }

        public override Object Invoke(
          Object obj,
          BindingFlags invokeAttr,
          Binder binder,
          Object[] parameters,
          CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        public override MethodAttributes Attributes => MethodAttributes.Public;

        public override RuntimeMethodHandle MethodHandle => new RuntimeMethodHandle();

        public override MethodInfo GetBaseDefinition()
        {
            return null;
        }

        public override ICustomAttributeProvider ReturnTypeCustomAttributes => throw new InvalidOperationException();
    }
}
