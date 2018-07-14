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
            return assembly.GetCustomAttributes(typeof(TAttribute), false) as TAttribute[];
        }

        public static object GetFirstAttributeValue<TAttribute>(this Assembly assembly,
            Expression<Func<TAttribute, object>> propertyLambda) where TAttribute : Attribute
        {
            var type = typeof(TAttribute);
            var attributes = assembly.GetAttributes<TAttribute>();
            if (attributes.Length == 0)
                throw new ArgumentException("No values found");
            var body = propertyLambda.Body as MemberExpression;
            if (body == null)
                throw new ArgumentException(string.Format("Expression '{0}' refers to a method, not a property.",
                    propertyLambda));
            var member = body.Member as PropertyInfo;
            if (member == null)
                throw new ArgumentException(string.Format("Expression '{0}' refers to a field, not a property.",
                    propertyLambda));
            if (type != member.ReflectedType && !type.IsSubclassOf(member.ReflectedType))
                throw new ArgumentException(string.Format(
                    "Expresion '{0}' refers to a property that is not from type {1}.", propertyLambda, type));
            var attribute = attributes[0];
            return member.GetValue(attribute, null);
        }

        public static string GetAssemblyDescription(Assembly assembly)
        {
            return assembly
                .GetFirstAttributeValue((Expression<Func<AssemblyDescriptionAttribute, object>>) (x => x.Description))
                .ToString();
        }

        public static string GetAssemblyTitle(Assembly assembly)
        {
            return assembly.GetFirstAttributeValue((Expression<Func<AssemblyTitleAttribute, object>>) (x => x.Title))
                .ToString();
        }

        public static string GetAssemblyCompany(Assembly assembly)
        {
            return assembly
                .GetFirstAttributeValue((Expression<Func<AssemblyCompanyAttribute, object>>) (x => x.Company))
                .ToString();
        }

        public static string GetAssemblyProduct(Assembly assembly)
        {
            return assembly
                .GetFirstAttributeValue((Expression<Func<AssemblyProductAttribute, object>>) (x => x.Product))
                .ToString();
        }

        public static string GetAssemblyVersion(Assembly assembly)
        {
            var version = assembly.GetName().Version;
            return string.Format("{0}.{1}.{2}", version.Major, version.Minor, version.Build);
        }
    }
}