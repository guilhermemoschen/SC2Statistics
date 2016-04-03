using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

using SC2LiquipediaStatistics.DesktopClient.Common;
using SC2LiquipediaStatistics.DesktopClient.ViewModel;

using Image = System.Windows.Controls.Image;

namespace SC2LiquipediaStatistics.DesktopClient.View
{
    /// <summary>
    /// Interaction logic for PlayerSelection.xaml
    /// </summary>
    public partial class PlayerStatisticsView
    {
        public PlayerStatisticsView()
        {
            InitializeComponent();

            var prop = DependencyPropertyDescriptor.FromProperty(Image.SourceProperty, typeof(Image));
            prop.AddValueChanged(playerImage, Image_SourceUpdated);
        }

        private void Image_SourceUpdated(object sender, EventArgs eventArgs)
        {
            var image = sender as Image;

            if (image == null)
                return;

            if (!(image.Source is BitmapSource))
                return;

            if (image.Source.Width > 300)
            {
                var percentage = 300.0 / image.Source.Width;
                var newImageSouce = new TransformedBitmap((BitmapSource)image.Source, new ScaleTransform(percentage, percentage));
                image.SetCurrentValue(Image.SourceProperty, newImageSouce);
            }

            if (image.Height > 450)
            {
                var percentage = 450.0 / image.Height;
                var newImageSouce = new TransformedBitmap((BitmapSource)image.Source, new ScaleTransform(percentage, percentage));
                image.SetCurrentValue(Image.SourceProperty, newImageSouce);
            }
        }
    }
}