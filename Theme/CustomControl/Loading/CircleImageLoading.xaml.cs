using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Theme.CustomControl.Loading
{
    /// <summary>
    /// Interaction logic for CircleImageLoading.xaml
    /// </summary>
    public partial class CircleImageLoading : UserControl
    {
        /// <summary>
        /// Source property
        /// </summary>
        public static readonly DependencyProperty SourceProperty = DependencyProperty.Register("Source", typeof(ImageSource), typeof(CircleImageLoading), new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.AffectsRender, new PropertyChangedCallback(OnSourceChanged)));
        /// <summary>
        /// Source property
        /// </summary>
        /// <param name="d">Dependency object</param>
        /// <param name="e">Dependency property changed event args</param>
        private static void OnSourceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            CircleImageLoading CircleImageLoading = (CircleImageLoading)d;
            ImageSource source = e.NewValue as ImageSource;
            if (null == source)
                return;
            //Storyboard storyboard = (Storyboard)CircleImageLoading.FindResource("AnimationStoryboard");
            //CircleImageLoading.BeginStoryboard(storyboard);
        }

        /// <summary>
        /// Source
        /// </summary>
        public ImageSource Source
        {
            get { return (ImageSource)GetValue(SourceProperty); }
            set { SetValue(SourceProperty, value); }
        }

        /// <summary>
        /// Duration  property
        /// </summary>
        public static readonly DependencyProperty DurationProperty = DependencyProperty.Register("Duration", typeof(Duration), typeof(CircleImageLoading), new FrameworkPropertyMetadata(new Duration(TimeSpan.FromSeconds(1))));

        /// <summary>
        /// Duration
        /// </summary>
        public Duration Duration
        {
            get { return (Duration)GetValue(DurationProperty); }
            set { SetValue(DurationProperty, value); }
        }


        /// <summary>
        /// Stretch  property
        /// </summary>
        public static readonly DependencyProperty StretchProperty = DependencyProperty.Register("Stretch", typeof(Stretch), typeof(CircleImageLoading), new FrameworkPropertyMetadata(Stretch.None));

        /// <summary>
        /// Stretch 
        /// </summary>
        public Stretch Stretch
        {
            get { return (Stretch)GetValue(StretchProperty); }
            set { SetValue(StretchProperty, value); }
        }

        public CircleImageLoading()
        {
            InitializeComponent();
        }
    }
}
