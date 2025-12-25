using System;
using System.Windows;
using System.Windows.Data;
using System.Windows.Markup;
using Theme.Converters;

namespace Theme.MarkupExtensions
{
    public class DynamicStringResourceWithStringFormatExtension : MarkupExtension
    {
        public string ResourceKey { get; set; }
        public string Format { get; set; } = "{0}:";

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            // 创建一个 MultiBinding
            var multiBinding = new MultiBinding();

            // 设置 StringFormat
            multiBinding.StringFormat = Format;

            // 创建第一个绑定：获取目标元素
            var elementBinding = new Binding
            {
                RelativeSource = new RelativeSource(RelativeSourceMode.Self)
            };
            multiBinding.Bindings.Add(elementBinding);

            // 设置转换器
            multiBinding.Converter = new ResourceConverter { ResourceKey = ResourceKey };

            return multiBinding.ProvideValue(serviceProvider);
        }
    }
}