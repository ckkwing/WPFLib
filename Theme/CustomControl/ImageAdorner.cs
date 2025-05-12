using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;

namespace Theme.CustomControl
{
    public class ImageAdorner : Adorner
    {
        private readonly Image _image;
        private Point _offset;

        protected override int VisualChildrenCount => 1;

        public ImageAdorner(UIElement adornedElement, ImageSource imageSource, Point offset)
            : base(adornedElement)
        {
            _offset = offset;
            _image = new Image
            {
                Source = imageSource,
                Stretch = Stretch.None,
                HorizontalAlignment = HorizontalAlignment.Left,
                VerticalAlignment = VerticalAlignment.Top,
                Margin = new Thickness(offset.X, offset.Y, 0, 0)
            };
            //Panel.SetZIndex(_image, 999);
            AddVisualChild(_image);
        }

        public void UpdateOffset(Point newOffset)
        {
            _offset = newOffset;
            _image.Margin = new Thickness(newOffset.X, newOffset.Y, 0, 0);
            InvalidateArrange();
        }


        protected override Visual GetVisualChild(int index)
        {
            if (index != 0) throw new ArgumentOutOfRangeException();
            return _image;
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            _image.Arrange(new Rect(finalSize));
            return finalSize;
        }
    }
}
