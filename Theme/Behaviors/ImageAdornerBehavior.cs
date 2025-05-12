using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows;
using Theme.CustomControl;

namespace Theme.Behaviors
{
    public static class ImageAdornerBehavior
    {
        #region AdornerImage
        public static readonly DependencyProperty AdornerImageProperty =
            DependencyProperty.RegisterAttached(
                "AdornerImage",
                typeof(ImageSource),
                typeof(ImageAdornerBehavior),
                new PropertyMetadata(null, OnAdornerPropertiesChanged));

        public static void SetAdornerImage(UIElement element, ImageSource value)
        {
            element.SetValue(AdornerImageProperty, value);
        }

        public static ImageSource GetAdornerImage(UIElement element)
        {
            return (ImageSource)element.GetValue(AdornerImageProperty);
        }
        #endregion

        #region AdornerOffset
        public static readonly DependencyProperty AdornerOffsetProperty =
            DependencyProperty.RegisterAttached(
                "AdornerOffset",
                typeof(Point),
                typeof(ImageAdornerBehavior),
                new PropertyMetadata(new Point(0, 0), OnAdornerPropertiesChanged));

        public static void SetAdornerOffset(UIElement element, Point value)
        {
            element.SetValue(AdornerOffsetProperty, value);
        }

        public static Point GetAdornerOffset(UIElement element)
        {
            return (Point)element.GetValue(AdornerOffsetProperty);
        }
        #endregion

        private static void OnAdornerPropertiesChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is UIElement adornedElement)
            {
                var layer = AdornerLayer.GetAdornerLayer(adornedElement);
                if (layer == null) return;

                // Get current adorners
                var adorners = layer.GetAdorners(adornedElement);
                var existingAdorner = adorners?.OfType<ImageAdorner>().FirstOrDefault();

                var imageSource = GetAdornerImage(adornedElement);
                var offset = GetAdornerOffset(adornedElement);

                if (existingAdorner != null)
                {
                    if (imageSource == null)
                    {
                        // Remove existing adorner if the image source is null
                        layer.Remove(existingAdorner);
                    }
                    else
                    {
                        // Update offset of existing adorner
                        existingAdorner.UpdateOffset(offset);
                    }
                }
                else if (imageSource != null)
                {
                    // Create a new adorner
                    layer.Add(new ImageAdorner(adornedElement, imageSource, offset));
                }
            }
        }
    }
}
