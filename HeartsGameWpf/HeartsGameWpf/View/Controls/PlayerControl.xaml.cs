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
    /// Interaction logic for PlayerControl.xaml
    /// </summary>
    public partial class PlayerControl : UserControl
    {
        public PlayerControl()
        {
            InitializeComponent();

            RootElement.DataContext = this;
        }

        private static readonly DependencyProperty HumanPlayerViewModelProperty
            = DependencyProperty.Register("HumanPlayerViewModel", typeof(HumanPlayerViewModel), typeof(PlayerControl));

        public HumanPlayerViewModel HumanPlayerViewModel
        {
            get { return (HumanPlayerViewModel)GetValue(HumanPlayerViewModelProperty); }
            set { SetValue(HumanPlayerViewModelProperty, value); }
        }

        private bool isMax;
        private void ListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (isMax)
                return;

            ListBox listView = (ListBox)sender;

            int count = listView.SelectedItems.Count;

            int maxCards = HumanPlayerViewModel.ExpectedNumberOfCards;

            if (count == maxCards && HumanPlayerViewModel.PassButtonVisibility == System.Windows.Visibility.Hidden)
            {
                PerformAction();
            }
            else if (count > maxCards)
            {
                isMax = true;

                listView.SelectedItems.RemoveAt(0); // Will raise the event SelectionChanged again

                isMax = false;
            }
        }

        private void PassButton_Click(object sender, RoutedEventArgs e)
        {
            PerformAction();
        }

        private void PerformAction()
        {
            IList<Card> selectedCards = HandListView.SelectedItems.Cast<Card>().ToList();
            if (selectedCards.Count > 0)
            {
                ICommand command = HumanPlayerViewModel.SubmitCommand;
                if (command.CanExecute(selectedCards))
                {
                    command.Execute(selectedCards);
                }
            }
        }
    }
}
