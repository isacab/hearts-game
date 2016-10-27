using HeartsGameEngine.DataObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;

namespace HeartsGameWpf.View.Converters
{
    class CardInListConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (values == null || values.Length < 2)
                return false;

            Card card = values[0] as Card;
            IList<Card> list = values[1] as IList<Card>;

            if (card == null || list == null)
                return false;

            bool found = list.Any(x => x.Equals(card));//.Contains(card, EqualityComparer<Card>.Default);

            return found;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
