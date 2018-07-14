// Decompiled with JetBrains decompiler
// Type: Generic.Models.XAttribute
// Assembly: Vibor.Helpers, Version=1.0.1.0, Culture=neutral, PublicKeyToken=null
// MVID: E29329B7-F05A-4CC7-B834-7BAFB4348D90
// Assembly location: C:\Users\alan\Downloads\Ver 1.1.8\Debug\Vibor.Helpers.dll

using System;
using System.Linq.Expressions;
using System.Reflection;

namespace Vibor.Helpers
{
  public static class XAttribute
  {
    public static TAttribute[] GetAttributes<TAttribute>(this Assembly assembly) where TAttribute : Attribute
    {
      return assembly.GetCustomAttributes(typeof (TAttribute), false) as TAttribute[];
    }

    public static object GetFirstAttributeValue<TAttribute>(this Assembly assembly, Expression<Func<TAttribute, object>> propertyLambda) where TAttribute : Attribute
    {
      Type type = typeof (TAttribute);
      TAttribute[] attributes = assembly.GetAttributes<TAttribute>();
      if (attributes.Length == 0)
        throw new ArgumentException("No values found");
      MemberExpression body = propertyLambda.Body as MemberExpression;
      if (body == null)
        throw new ArgumentException(string.Format("Expression '{0}' refers to a method, not a property.",  propertyLambda));
      PropertyInfo member = body.Member as PropertyInfo;
      if (member == (PropertyInfo) null)
        throw new ArgumentException(string.Format("Expression '{0}' refers to a field, not a property.",  propertyLambda));
      if (type != member.ReflectedType && !type.IsSubclassOf(member.ReflectedType))
        throw new ArgumentException(string.Format("Expresion '{0}' refers to a property that is not from type {1}.",  propertyLambda,  type));
      TAttribute attribute = attributes[0];
      return member.GetValue( attribute, (object[]) null);
    }

    public static string GetAssemblyDescription(Assembly assembly)
    {
      return assembly.GetFirstAttributeValue<AssemblyDescriptionAttribute>((Expression<Func<AssemblyDescriptionAttribute, object>>) (x => x.Description)).ToString();
    }

    public static string GetAssemblyTitle(Assembly assembly)
    {
      return assembly.GetFirstAttributeValue<AssemblyTitleAttribute>((Expression<Func<AssemblyTitleAttribute, object>>) (x => x.Title)).ToString();
    }

    public static string GetAssemblyCompany(Assembly assembly)
    {
      return assembly.GetFirstAttributeValue<AssemblyCompanyAttribute>((Expression<Func<AssemblyCompanyAttribute, object>>) (x => x.Company)).ToString();
    }

    public static string GetAssemblyProduct(Assembly assembly)
    {
      return assembly.GetFirstAttributeValue<AssemblyProductAttribute>((Expression<Func<AssemblyProductAttribute, object>>) (x => x.Product)).ToString();
    }

    public static string GetAssemblyVersion(Assembly assembly)
    {
      Version version = assembly.GetName().Version;
      return string.Format("{0}.{1}.{2}",  version.Major,  version.Minor,  version.Build);
    }
  }
}
