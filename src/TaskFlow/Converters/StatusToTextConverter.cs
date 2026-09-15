using System;
using System.Globalization;
using System.Windows.Data;
using TaskFlow.Models;

namespace TaskFlow.Converters
{
    public class StatusToTextConverter : IValueConverter
    {
        public static string ToText(TaskState state)
        {
            switch (state)
            {
                case TaskState.AFaire:
                    return "A faire";
                case TaskState.EnCours:
                    return "En cours";
                case TaskState.Bloquee:
                    return "Bloquee";
                case TaskState.Terminee:
                    return "Terminee";
                default:
                    return "";
            }
        }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return ToText((TaskState)value);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
