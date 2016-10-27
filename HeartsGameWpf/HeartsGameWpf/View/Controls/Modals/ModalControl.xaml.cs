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
    /// Interaction logic for ModalControl.xaml
    /// </summary>
    public partial class ModalControl : UserControl
    {
        public ModalControl()
        {
            InitializeComponent();

            Modal.DataContext = this;
            StaticBackdrop = false;
        }

        private static readonly DependencyProperty TitleProperty
            = DependencyProperty.Register("Title", typeof(object), typeof(ModalControl));

        public object Title
        {
            get { return (object)GetValue(TitleProperty); }
            set { SetValue(TitleProperty, value); }
        }

        private static readonly DependencyProperty BodyProperty
            = DependencyProperty.Register("Body", typeof(object), typeof(ModalControl));

        public object Body
        {
            get { return (object)GetValue(BodyProperty); }
            set { SetValue(BodyProperty, value); }
        }

        private static readonly DependencyProperty FooterProperty
            = DependencyProperty.Register("Footer", typeof(object), typeof(ModalControl));

        public object Footer
        {
            get { return (object)GetValue(FooterProperty); }
            set { SetValue(FooterProperty, value); }
        }

        private static readonly DependencyProperty StaticBackdropProperty
            = DependencyProperty.Register("StaticBackdrop", typeof(object), typeof(ModalControl));

        public bool StaticBackdrop
        {
            get { return (bool)GetValue(StaticBackdropProperty); }
            set { SetValue(StaticBackdropProperty, value); }
        }

        private void Modal_MouseUp(object sender, MouseButtonEventArgs e)
        {
            if (!StaticBackdrop)
                this.Visibility = Visibility.Collapsed;
        }
    }
}
