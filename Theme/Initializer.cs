using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Theme.Resources;
using Theme.ViewModels;

namespace Theme
{
    public class Initializer : NotifyPropertyObject
    {
        #region Singleton
        public static Initializer Instance
        {
            get { return Nested.instance; }
        }

        class Nested
        {
            static Nested() { }
            internal static readonly Initializer instance = new Initializer();
        }

        Initializer()
        {
        }
        #endregion

        public string AppName { get; set; }

        public void Initialize(string appName)
        {
            AppName = appName;
            ThemeResources.SetLanguage(System.Globalization.CultureInfo.CurrentUICulture);
        }

    }
}
