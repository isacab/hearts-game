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
    /// Interaction logic for ScoreControl.xaml
    /// </summary>
    public partial class ScoreBoardControl : UserControl
    {
        public ScoreBoardControl()
        {
            InitializeComponent();

            ScoreBoard.DataContext = this;
        }

        private static readonly DependencyProperty ScoreBoardViewModelProperty
            = DependencyProperty.Register("ScoreBoardViewModel", typeof(ScoreBoardViewModel), typeof(ScoreBoardControl));

        public ScoreBoardViewModel ScoreBoardViewModel
        {
            get { return (ScoreBoardViewModel)GetValue(ScoreBoardViewModelProperty); }
            set { SetValue(ScoreBoardViewModelProperty, value); }
        }
    }
}
