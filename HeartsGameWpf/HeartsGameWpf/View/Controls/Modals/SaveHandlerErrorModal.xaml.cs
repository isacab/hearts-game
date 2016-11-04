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

namespace HeartsGameWpf.View.Controls.Modals
{
    /// <summary>
    /// Interaction logic for SaveHandlerErrorModal.xaml
    /// </summary>
    public partial class SaveHandlerErrorModal : UserControl
    {
        public SaveHandlerErrorModal()
        {
            InitializeComponent(); 
            
            Modal.DataContext = this;
        }

        private static readonly DependencyProperty SaveHandlerErrorModalViewModelProperty
            = DependencyProperty.Register("SaveHandlerErrorModalViewModel", typeof(SaveHandlerErrorModalViewModel), typeof(SaveHandlerErrorModal));

        public SaveHandlerErrorModalViewModel SaveHandlerErrorModalViewModel
        {
            get { return (SaveHandlerErrorModalViewModel)GetValue(SaveHandlerErrorModalViewModelProperty); }
            set { SetValue(SaveHandlerErrorModalViewModelProperty, value); }
        }

        private static readonly DependencyProperty CloseCommandProperty
            = DependencyProperty.Register("CloseCommand", typeof(ICommand), typeof(SaveHandlerErrorModal));

        public ICommand CloseCommand
        {
            get { return (ICommand)GetValue(CloseCommandProperty); }
            set { SetValue(CloseCommandProperty, value); }
        }

        private void RetryButton_Click(object sender, RoutedEventArgs e)
        {
            CloseCommand.Execute(0);
        }

        private void NewGameButton_Click(object sender, RoutedEventArgs e)
        {
            CloseCommand.Execute(1);
        }
    }
}
