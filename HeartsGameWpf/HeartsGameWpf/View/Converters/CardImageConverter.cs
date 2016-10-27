using HeartsGameEngine.DataObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media.Imaging;

namespace HeartsGameWpf.View.Converters
{
    public class CardImageConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            Card card = value as Card;

            if (card == null)
                return DependencyProperty.UnsetValue;

            string key = card.ToString().ToLower();

            BitmapImage image = Application.Current.FindResource(key) as BitmapImage;

            return image;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
