using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Theme.Resources;

namespace Theme.Attributes
{
    [AttributeUsage(AttributeTargets.All, Inherited = false, AllowMultiple = false)]
    public sealed class LocalizableDescriptionAttribute : DescriptionAttribute
    {
        private readonly string id;

        /// <summary>
        /// A <see cref="DescriptionAttribute"/> loading the description from the applications ressources.
        /// </summary>
        /// <param name="id">The string id of the localized string.</param>
        public LocalizableDescriptionAttribute(string id)
            : base(id)
        {
            this.id = id;
        }

        /// <summary>
        /// Get the string value from the resources.
        /// Always lookup the resource so we don't miss language changes. (TODO: cache this and register for language changes?)
        /// </summary>
        /// <returns>The localized description.</returns>
        public override string Description
        {
            get
            {
                return ResourceProvider.LoadString(Id);
            }
        }

        public string Id
        {
            get
            {
                return id;
            }
        }
    }
}
