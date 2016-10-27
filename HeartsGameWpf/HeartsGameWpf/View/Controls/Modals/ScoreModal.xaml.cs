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
    /// Interaction logic for ScoreModal.xaml
    /// </summary>
    public partial class ScoreModal : UserControl
    {
        public ScoreModal()
        {
            InitializeComponent();

            Modal.DataContext = this;
        }

        private static readonly DependencyProperty ScoreModalViewModelProperty
            = DependencyProperty.Register("ScoreModalViewModel", typeof(ScoreModalViewModel), typeof(ScoreModal));

        public ScoreModalViewModel ScoreModalViewModel
        {
            get { return (ScoreModalViewModel)GetValue(ScoreModalViewModelProperty); }
            set { SetValue(ScoreModalViewModelProperty, value); }
        }

        private static readonly DependencyProperty CloseCommandProperty
            = DependencyProperty.Register("CloseCommand", typeof(ICommand), typeof(ScoreModal));

        public ICommand CloseCommand
        {
            get { return (ICommand)GetValue(CloseCommandProperty); }
            set { SetValue(CloseCommandProperty, value); }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            CloseCommand.Execute(null);
        }
    }
}
