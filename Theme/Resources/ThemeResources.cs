using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Markup;

namespace Theme.Resources
{
    public static class ThemeResources
    {
        public static class PlaceHolder
        {
            public static string ApplicationName = "[APPLICATION_NAME]";
        }

        public static ResourceDictionary LanguageResourceDictionary { get; private set; }

        public static ResourceDictionary ImageResourceDictionary { get; private set; }

        public static CultureInfo CurrentUICulture { get; private set; }

        static ThemeResources()
        {
            ImageResourceDictionary = new ResourceDictionary() { Source = new Uri("/Theme;Component/Styles/Resource_Images.xaml", UriKind.Relative) };
        }

        public static void SetLanguage(CultureInfo cultureInfo)
        {
            CurrentUICulture = cultureInfo;
            LanguageResourceDictionary = new ResourceDictionary() { Source = new Uri("/Theme;Component/Resources/LocalizedResources/Language.xaml", UriKind.Relative) };
            string LanResPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "LocalizedResources");
            string en_USResFile = Path.Combine(LanResPath, "Language_en-US.xaml");
            string resFile = String.Format("Language_{0}.xaml", cultureInfo.IetfLanguageTag);
            string curLanRes = Path.Combine(LanResPath, resFile);
            ResourceDictionary localizedlanRes = null;
            if (File.Exists(curLanRes))
            {
                try
                {
                    using (var fs = File.Open(curLanRes, FileMode.Open, FileAccess.Read, FileShare.Read))
                    {
                        localizedlanRes = XamlReader.Load(fs) as ResourceDictionary;
                    }
                }
                catch (Exception ex)
                {
                    Trace.WriteLine("load language file Failed: " + ex.Message);
                }
            }

            if (localizedlanRes == null)
            {
                if (File.Exists(en_USResFile))
                {
                    try
                    {
                        using (var fs = File.Open(en_USResFile, FileMode.Open, FileAccess.Read, FileShare.Read))
                        {
                            localizedlanRes = XamlReader.Load(fs) as ResourceDictionary;
                        }
                    }
                    catch (Exception ex)
                    {
                        Trace.WriteLine("load language file Failed: " + ex.Message);
                    }
                }
            }

            if (localizedlanRes != null)
            {
                foreach (object dicKey in localizedlanRes.Keys)
                {
                    if (LanguageResourceDictionary.Contains(dicKey))
                    {
                        LanguageResourceDictionary[dicKey] = localizedlanRes[dicKey];
                    }
                }
            }

            ReplacePlaceHolder(LanguageResourceDictionary);
        }

        private static void ReplacePlaceHolder(ResourceDictionary lanRes)
        {
            Trace.WriteLine(">>ReplacePlaceHolder Start");
            Dictionary<String, String> ControlCodesReplacement = new Dictionary<String, String>();
            ControlCodesReplacement.Add("\\n", "\n");
            ControlCodesReplacement.Add("\\t", "\t");
            ControlCodesReplacement.Add("\\\"", "\"");
            ControlCodesReplacement.Add("\\v", "\v");
            ControlCodesReplacement.Add("\\r", "\r");
            ControlCodesReplacement.Add("\\\\", "\\");
            ControlCodesReplacement.Add(PlaceHolder.ApplicationName, Initializer.Instance.AppName);

            foreach (Object newDictKey in lanRes.Keys)
            {
                Object newDictValue = null;
                if (!lanRes.Contains(newDictKey))
                {
                    continue;
                }

                newDictValue = lanRes[newDictKey];
                if (newDictValue is String)
                {
                    String tempString = (String)newDictValue;
                    foreach (String ControlCode in ControlCodesReplacement.Keys)
                    {
                        tempString = tempString.Replace(ControlCode, ControlCodesReplacement[ControlCode]);
                    }
                    lanRes[newDictKey] = tempString;
                }
            }

            Trace.WriteLine(">>ReplacePlaceHolder End");
        }

    }
}
