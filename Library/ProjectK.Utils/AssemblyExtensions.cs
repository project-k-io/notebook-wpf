using System;
using System.Linq.Expressions;
using System.Reflection;

namespace ProjectK.ToolKit.Utils;

public static class AssemblyExtensions
{
    private static TAttribute[] GetAttributes<TAttribute>(this Assembly assembly) where TAttribute : Attribute
    {
        return assembly.GetCustomAttributes(typeof(TAttribute), false) as TAttribute[];
    }

    private static object GetFirstAttributeValue<TAttribute>(this Assembly assembly,
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


    public static string GetAssemblyTitle(this Assembly assembly)
    {
        return assembly.GetFirstAttributeValue((Expression<Func<AssemblyTitleAttribute, object>>)(x => x.Title))
            .ToString();
    }


    public static string GetAssemblyVersion(this Assembly assembly)
    {
        var version = assembly.GetName().Version;
        return
            $"{(version != null ? version.Major : null)}.{(version != null ? version.Minor : null)}.{(version != null ? version.Build : null)}";
    }
}