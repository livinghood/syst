using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Markup;
using Logic_Layer;

namespace BUPSystem
{
    [MarkupExtensionReturnType(typeof(object))]
    public class DelayBindingExtension : MarkupExtension
    {
        /// <summary>
        /// The decorated binding class.
        /// </summary>
        private Binding binding = new Binding();

        public DelayBindingExtension()
        {
            Delay = TimeSpan.FromSeconds(0.5);
        }

        public DelayBindingExtension(PropertyPath path)
            : this()
        {
            Path = path;
        }

        [DefaultValue(null)]
        public Collection<ValidationRule> ValidationRules
        {
            get { return binding.ValidationRules; }
        }
        public IValueConverter Converter { get; set; }
        public object ConverterParamter { get; set; }
        public string ElementName { get; set; }
        public RelativeSource RelativeSource { get; set; }
        public object Source { get; set; }
        public bool ValidatesOnDataErrors { get; set; }
        public bool ValidatesOnExceptions { get; set; }
        public TimeSpan Delay { get; set; }
        [ConstructorArgument("path")]
        public PropertyPath Path { get; set; }
        [TypeConverter(typeof(CultureInfoIetfLanguageTagConverter))]
        public CultureInfo ConverterCulture { get; set; }

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            var valueProvider = serviceProvider.GetService(typeof(IProvideValueTarget)) as IProvideValueTarget;
            if (valueProvider != null)
            {
                var bindingTarget = valueProvider.TargetObject as DependencyObject;
                var bindingProperty = valueProvider.TargetProperty as DependencyProperty;
                if (bindingProperty == null || bindingTarget == null)
                {
                    throw new NotSupportedException(string.Format(
                        "The property '{0}' on target '{1}' is not valid for a DelayBinding. The DelayBinding target must be a DependencyObject, "
                        + "and the target property must be a DependencyProperty.",
                        valueProvider.TargetProperty,
                        valueProvider.TargetObject));
                }

                
                binding.Path = Path;
                binding.Converter = Converter;
                binding.ConverterCulture = ConverterCulture;
                binding.ConverterParameter = ConverterParamter;
                if (ElementName != null) binding.ElementName = ElementName;
                if (RelativeSource != null) binding.RelativeSource = RelativeSource;
                if (Source != null) binding.Source = Source;
                binding.ValidatesOnDataErrors = ValidatesOnDataErrors;
                binding.ValidatesOnExceptions = ValidatesOnExceptions;

                return DelayBinding.SetBinding(bindingTarget, bindingProperty, Delay, binding);
            }
            return null;
        }
    }
}
