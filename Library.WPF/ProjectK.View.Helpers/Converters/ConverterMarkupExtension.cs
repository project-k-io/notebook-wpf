using System;
using System.Windows.Markup;

namespace ProjectK.View.Helpers.Converters
{
    public class ConverterMarkupExtension<T> : MarkupExtension where T : class, new()
    {
        private static T _extensionConverter;

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            return _extensionConverter ??= Activator.CreateInstance<T>();
        }
    }
}