using log4net.Repository.Hierarchy;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Theme.CustomControl
{
    /// <summary>
    /// Pixel Snapper: used to decarate blurry elements which will remove blurry .
    /// </summary>
    public class PixelSnapper : Decorator
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PixelSnapper"/> class.
        /// </summary>
        public PixelSnapper()
            : base()
        {
            this.LayoutUpdated += PixelSnapper_LayoutUpdated;

        }

        /// <summary>
        /// Measures the child element of a <see cref="T:System.Windows.Controls.Decorator"/> to prepare for arranging it during the <see cref="M:System.Windows.Controls.Decorator.ArrangeOverride(System.Windows.Size)"/> pass.
        /// </summary>
        /// <param name="constraint">An upper limit <see cref="T:System.Windows.Size"/> that should not be exceeded.</param>
        /// <returns>
        /// The target <see cref="T:System.Windows.Size"/> of the element.
        /// </returns>
        protected override Size MeasureOverride(Size constraint)
        {
            if (this.Child != null)
            {
                this.Child.Measure(constraint);

                return new Size(Math.Floor(this.Child.DesiredSize.Width), Math.Floor(this.Child.DesiredSize.Height));
            }

            return base.MeasureOverride(constraint);
        }

        /// <summary>
        /// Handles the LayoutUpdated event of the PixelSnapper control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        void PixelSnapper_LayoutUpdated(object sender, EventArgs e)
        {
            try
            {
                this.LayoutUpdated -= PixelSnapper_LayoutUpdated;

                if (this.Child == null)
                    return;
                // Remove existing transform so that it is not a part of the calculations
                if (this._transform != null)
                {
                    this._transform.X = 0;
                    this._transform.Y = 0;
                }

                // Calculate actual location
                PresentationSource ps = PresentationSource.FromVisual(this);
                if (ps != null)
                {
                    MatrixTransform globalTransform = this.Child.TransformToVisual(ps.RootVisual) as MatrixTransform;
                    Point p = globalTransform.Matrix.Transform(_zero);

                    double deltaX = Math.Round(p.X) - p.X;
                    double deltaY = Math.Round(p.Y) - p.Y;
                    // Set new transform
                    if (deltaX != 0 || deltaY != 0)
                    {
                        if (this._transform == null)
                        {
                            this._transform = new TranslateTransform();
                            this.Child.RenderTransform = _transform;
                        }

                        if (deltaX != 0)
                        {
                            this._transform.X = deltaX;
                        }

                        if (deltaY != 0)
                        {
                            this._transform.Y = deltaY;
                        }
                    }
                }
                else
                {
                    this.LayoutUpdated += PixelSnapper_LayoutUpdated;
                }
            }
            catch (Exception exp)
            {
                Debug.WriteLine($"Application Exception:{exp}");
            }
        }

        TranslateTransform _transform;
        private static readonly Point _zero = new Point(0, 0);
    }

}
