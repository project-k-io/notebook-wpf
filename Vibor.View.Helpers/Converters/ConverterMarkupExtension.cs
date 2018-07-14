using System;
using System.Windows.Markup;

namespace Vibor.View.Helpers.Converters
{
    public class ConverterMarkupExtension<T> : MarkupExtension where T : class, new()
    {
        private static T extensionConverter;

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            if (extensionConverter == null)
                extensionConverter = Activator.CreateInstance<T>();
            return extensionConverter;
        }
    }
}