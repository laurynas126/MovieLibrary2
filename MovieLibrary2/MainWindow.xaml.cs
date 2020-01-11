using MovieLibrary2.DataManagement;
using MovieLibrary2.ViewModel;
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

namespace MovieLibrary2
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private Brush WhiteBrush = new SolidColorBrush(Colors.White);
        private Brush BlackBrush = new SolidColorBrush(Colors.Black);

        public MainWindow()
        {
            InitializeComponent();
            foreach(var a in itemControl.ItemsSource)
            {
                a.ToString();
            }
        }

        private void Grid_MouseEnter(object sender, MouseEventArgs e)
        {
            if (sender is Panel)
            {
                var panel = sender as Panel;
                var children = panel.Children;
                foreach (var element in children)
                {
                    SetTextBoxForeground(element, BlackBrush);
                }
                panel.Background = WhiteBrush;
            }
        }

        private void Grid_MouseLeave(object sender, MouseEventArgs e)
        {
            if (sender is Panel)
            {
                var panel = sender as Panel;
                var children = panel.Children;
                foreach (var element in children)
                {
                    SetTextBoxForeground(element, WhiteBrush);
                }
                panel.Background = BlackBrush;
            }
        }

        private void SetTextBoxForeground(object element, Brush brush)
        {
            if (element is TextBlock)
            {
                var tb = element as TextBlock;
                tb.Foreground = brush;
            }
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            MovieRepository.SaveMoviesToDataFile(Properties.Settings.Default.DataFilePath);
        }

        private void itemControl_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            var listBox = sender as ListBox;
            if (listBox.SelectedIndex == -1)
            {
                return;
            }
            var selectedItem = listBox.SelectedItem;
            ((MoviesListView)DataContext).MovieClick(selectedItem);
            listBox.SelectedIndex = -1;
        }

        private void ItemControl_MouseRightButtonUp(object sender, MouseButtonEventArgs e)
        {
            itemControl.SelectedIndex = -1;
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            ((MoviesListView)DataContext).CloseDetail();
            itemControl.Items.Refresh();
        }

        private void Image_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ClickCount == 2)
                Launch();
        }

        private void WatchButton_Click(object sender, RoutedEventArgs e)
        {
            Launch();
        }

        private void Launch()
        {
            ((MoviesListView)DataContext).LaunchMovie();
        }

        private void EditButton_Click(object sender, RoutedEventArgs e)
        {
            ((MoviesListView)DataContext).ChangeMode();
        }

        private async void DownloadInfoButton_Click(object sender, RoutedEventArgs e)
        {
            await ((MoviesListView)DataContext).DownloadInfo();
        }

        private async void DownloadAllMoviesInfoButton_Click(object sender, RoutedEventArgs e)
        {
            await ((MoviesListView)DataContext).DownloadAllMoviesInfo();
        }

        private void OpenExternalLinkButton_Click(object sender, EventArgs e)
        {
            ((MoviesListView)DataContext).OpenExternalLink();
        }

        private void window_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
            {
                CloseButton_Click(sender, e);
            }
            ((MoviesListView)DataContext).FilterEvent(e.Key);
        }

        private void window_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
            {
                CloseButton_Click(sender, e);
            }
            ((MoviesListView)DataContext).FilterEvent(e.Key);
        }
    }
}
