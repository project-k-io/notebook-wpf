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

            if (!(propertyLambda.Body is MemberExpression body))
                throw new ArgumentException($"Expression '{propertyLambda}' refers to a method, not a property.");

            var member = body.Member as PropertyInfo;
            if (member == null)
                throw new ArgumentException($"Expression '{propertyLambda}' refers to a field, not a property.");

            if (type != member.ReflectedType && !type.IsSubclassOf(member.ReflectedType))
                throw new ArgumentException(
                    $"Expression '{propertyLambda}' refers to a property that is not from type {type}.");

            var attribute = attributes[0];
            return member.GetValue(attribute, null);
        }

        public static string GetAssemblyDescription(Assembly assembly)
        {
            return assembly
                .GetFirstAttributeValue((Expression<Func<AssemblyDescriptionAttribute, object>>)(x => x.Description))
                .ToString();
        }

        public static string GetAssemblyTitle(Assembly assembly)
        {
            return assembly.GetFirstAttributeValue((Expression<Func<AssemblyTitleAttribute, object>>)(x => x.Title))
                .ToString();
        }

        public static string GetAssemblyCompany(Assembly assembly)
        {
            return assembly
                .GetFirstAttributeValue((Expression<Func<AssemblyCompanyAttribute, object>>)(x => x.Company))
                .ToString();
        }

        public static string GetAssemblyProduct(Assembly assembly)
        {
            return assembly
                .GetFirstAttributeValue((Expression<Func<AssemblyProductAttribute, object>>)(x => x.Product))
                .ToString();
        }

        public static string GetAssemblyVersion(Assembly assembly)
        {
            var version = assembly.GetName().Version;
            return $"{version.Major}.{version.Minor}.{version.Build}";
        }
    }
}