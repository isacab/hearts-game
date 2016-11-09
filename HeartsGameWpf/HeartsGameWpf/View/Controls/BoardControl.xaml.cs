using HeartsGameEngine.DataObjects;
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

namespace HeartsGameWpf.View.Controls
{
    /// <summary>
    /// Interaction logic for BoardControl.xaml
    /// </summary>
    public partial class BoardControl : UserControl
    {
        public BoardControl()
        {
            InitializeComponent();

            RootElement.DataContext = this;
        }

        private static readonly DependencyProperty BoardViewModelProperty
            = DependencyProperty.Register("BoardViewModel", typeof(BoardViewModel), typeof(BoardControl));

        public BoardViewModel BoardViewModel
        {
            get { return (BoardViewModel)GetValue(BoardViewModelProperty); }
            set { SetValue(BoardViewModelProperty, value); }
        }
    }
}
