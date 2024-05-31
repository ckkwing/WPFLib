using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace Theme.Resources
{
    public class ResourceProvider
    {
        public static string LoadString(string resource)
        {
            try
            {
                // MI: Must try first to find the resource otherwise application will crash.
                if (Application.Current != null)
                {
                    object obj = Application.Current.TryFindResource(resource);
                    if (obj != null)
                    {
                        obj = Application.Current.FindResource(resource);
                        return obj.ToString();
                    }
                }
            }
            catch (ResourceReferenceKeyNotFoundException e)
            {
                Debug.WriteLine($"LoadString ResourceReferenceKeyNotFoundException {e}");
                //LogHelper.UILogger.Debug("LoadString ResourceReferenceKeyNotFoundException", e);
            }
            catch (ArgumentNullException ex)
            {
                Debug.WriteLine($"LoadString ArgumentNullException {ex}");
                //LogHelper.UILogger.Debug("LoadString ArgumentNullException", ex);
            }

            return resource;
        }

        /// <summary>
        ///  AH: function to load strings from Localizaed dictionary
        /// </summary>
        public static object LoadResource(object resourceKey)
        {
            if (resourceKey == null)
            {
                return null;
            }

            object obj = null;
            try
            {
                // MI: Must try first to find the resource otherwise application will crash.
                if (Application.Current != null)
                {
                    obj = Application.Current.TryFindResource(resourceKey);
                    if (obj != null)
                    {
                        obj = Application.Current.FindResource(resourceKey);
                    }
                }
            }
            catch (ResourceReferenceKeyNotFoundException e)
            {
                Debug.WriteLine($"ResourceReferenceKeyNotFoundException: {e}");
                //LogHelper.UILogger.Debug("ResourceReferenceKeyNotFoundException:", exc);
            }
            catch (ArgumentNullException ex)
            {
                Debug.WriteLine($"ArgumentNullException: {ex}");
                //LogHelper.UILogger.Debug("ArgumentNullException:", exc);
            }

            return obj;
        }

        public static ImageSource LoadImageSource(object resourceKey)
        {
            ImageSource source = null;
            try
            {
                if (Application.Current != null)
                {
                    var obj = Application.Current.TryFindResource(resourceKey);
                    if (obj != null)
                        source = (ImageSource)obj;
                }
            }
            catch (ResourceReferenceKeyNotFoundException e)
            {
                Debug.WriteLine($"LoadImageSource ResourceReferenceKeyNotFoundException {e}");
                //LogHelper.UILogger.Debug("LoadImageSource ResourceReferenceKeyNotFoundException", e);
            }
            catch (ArgumentNullException ex)
            {
                Debug.WriteLine($"LoadImageSource ArgumentNullException {ex}");
                //LogHelper.UILogger.Debug("LoadImageSource ArgumentNullException", ex);
            }
            return source;
        }
    }
}
