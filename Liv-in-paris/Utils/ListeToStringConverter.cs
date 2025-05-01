using System;
using System.Collections.Generic;
using System.Globalization;
using System.Windows.Data;

public class ListeToStringConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is List<string> list)
            return string.Join(", ", list);
        return "";
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}