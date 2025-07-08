using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Theme.Attributes;

namespace Theme.Extensions
{
    public static class CustomAttributeProviderExtension
    {
        /// <summary>
        /// Get the value of the DescriptionAttribute associated with the given item.
        /// </summary>
        /// <param name="item">The item to lookup. This can be a Type or a MemberInfo like FieldInfo, PropertyInfo...</param>
        /// <returns>The associated description.</returns>
        public static string GetDescription(this ICustomAttributeProvider item)
        {
            if (item == null)
                throw new ArgumentNullException("item");

            DescriptionAttribute attribute = item.GetCustomAttributes(typeof(DescriptionAttribute), false).OfType<DescriptionAttribute>().FirstOrDefault();

            if (attribute != null)
            {
                return attribute.Description;
            }

            throw new ArgumentException("Item does not have a DescrtiptionAttribute: " + item);
        }

        /// <summary>
        /// Get the value of the LocalizableDescriptionAttribute associated with the given item.
        /// </summary>
        /// <param name="item">The item to lookup. This can be a Type or a MemberInfo like FieldInfo, PropertyInfo...</param>
        /// <returns>The associated description.</returns>
        public static string GetLocalizedDescription(this ICustomAttributeProvider item)
        {
            if (item == null)
                throw new ArgumentNullException("item");

            LocalizableDescriptionAttribute attribute = item.GetCustomAttributes(typeof(LocalizableDescriptionAttribute), false).OfType<LocalizableDescriptionAttribute>().FirstOrDefault();

            if (attribute != null)
            {
                return attribute.Description;
            }

            throw new ArgumentException("Item does not have a LocalizableDescriptionAttribute: " + item);
        }
    }
}
