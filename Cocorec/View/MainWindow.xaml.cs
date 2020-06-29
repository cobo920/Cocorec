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
using System.Windows.Controls.Primitives;

namespace Cocorec
{
    /// <summary>
    /// MainWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Thumb_DragDelta(object sender, DragDeltaEventArgs e)
        {
            var change = Math.Min(e.HorizontalChange, e.VerticalChange);
            Width = Math.Min(MaxWidth, Math.Max(MinWidth, Width + change));
            Height = Math.Min(MaxHeight, Math.Max(MinHeight, Height + change));
        }

        private void WindowSizeMenuItem_Click(object sender, RoutedEventArgs e)
        {
            SizeChangeThumb.Width = RecordButton.ActualWidth;
            SizeChangeThumb.Height = RecordButton.ActualHeight;
            SizeChangeThumb.Visibility = Visibility.Visible;
        }

        private void Thumb_DragCompleted(object sender, DragCompletedEventArgs e)
        {
            SizeChangeThumb.Visibility = Visibility.Hidden;
        }
    }
}
