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

            //InitAnimations();
            //SetItemTemplateOnTrickItemsControl();
        }

        private static readonly DependencyProperty BoardViewModelProperty
            = DependencyProperty.Register("BoardViewModel", typeof(BoardViewModel), typeof(BoardControl));

        public BoardViewModel BoardViewModel
        {
            get { return (BoardViewModel)GetValue(BoardViewModelProperty); }
            set { SetValue(BoardViewModelProperty, value); }
        }

        private void InitAnimations()
        {
            //TrickItemsControl.Triggers.Add();
        }

        private void DoubleAnimation_Completed(object sender, EventArgs e)
        {
            //BoardViewModel.StartNewTurn.Execute(null);
        }

        private void Storyboard_Completed(object sender, EventArgs e)
        {
            //BoardViewModel.ClearTrick.Execute(null);
        }

        private void SetItemTemplateOnTrickItemsControl()
        {
            DataTemplate dataTemplate = new DataTemplate();
            dataTemplate.DataType = typeof(TrickItem);

            FrameworkElementFactory canvas = new FrameworkElementFactory(typeof(Canvas));
            canvas.SetValue(WidthProperty, 71d);
            canvas.SetValue(HeightProperty, 96d);

            FrameworkElementFactory border = new FrameworkElementFactory(typeof(Border));
            border.SetValue(BackgroundProperty, new SolidColorBrush(Colors.Black));
            Style borderStyle = new Style(typeof(Border));
            
            border.SetValue(StyleProperty, borderStyle);
            canvas.AppendChild(border);

            FrameworkElementFactory image = new FrameworkElementFactory(typeof(Image));
            image.SetBinding(DataContextProperty, new Binding("Card"));
            image.SetResourceReference(StyleProperty, "CardImage");
            border.AppendChild(image);

            dataTemplate.VisualTree = canvas;

            TrickItemsControl.ItemTemplate = dataTemplate;
        }

        private DataTrigger TrickItemPositionDataTrigger(int player)
        {
            DataTrigger trigger = new DataTrigger();
            trigger.Binding = new Binding("Player");
            trigger.Value = player;

            return trigger;
        }
    }
}
