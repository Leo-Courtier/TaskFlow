using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;
using TaskFlow.Models;

namespace TaskFlow.Converters
{
    public class PriorityToBrushConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            Priority priority = (Priority)value;

            if (priority == Priority.Critique)
            {
                return new SolidColorBrush(Color.FromRgb(196, 30, 30));
            }

            if (priority == Priority.Haute)
            {
                return new SolidColorBrush(Color.FromRgb(214, 116, 12));
            }

            if (priority == Priority.Normale)
            {
                return new SolidColorBrush(Color.FromRgb(45, 45, 45));
            }

            return new SolidColorBrush(Color.FromRgb(140, 140, 140));
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
