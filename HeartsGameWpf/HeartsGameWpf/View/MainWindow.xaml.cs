using HeartsGameWpf.ViewModel;
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

namespace HeartsGameWpf.View
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            GridMain.DataContext = this;
        }

        private bool firstTime = true;
        private void Window_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (firstTime)
            {
                FrameworkElement element = sender as FrameworkElement;
                element.MinHeight = e.NewSize.Height;
                element.MinWidth = e.NewSize.Width;
                firstTime = false;
            }
        }

        private static readonly DependencyProperty MainViewModelProperty
            = DependencyProperty.Register("MainViewModel", typeof(MainViewModel), typeof(MainWindow));

        public MainViewModel MainViewModel
        {
            get { return (MainViewModel)GetValue(MainViewModelProperty); }
            set { SetValue(MainViewModelProperty, value); }
        }
    }
}
