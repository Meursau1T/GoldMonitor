using System;
using System.Globalization;
using Avalonia.Data.Converters;
using GoldMonitor.Common;

namespace GoldMonitor.Converters;

public class LocaleConverter : IValueConverter
{
    public static readonly LocaleConverter Instance = new();

    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        return value is GoldProperty.Locale locale
            ? locale switch
            {
                GoldProperty.Locale.ZH => "ZH",
                GoldProperty.Locale.EN => "EN",
                _ => "?"
            }
            : "?";
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        return value is string str && str == "EN"
            ? GoldProperty.Locale.EN
            : GoldProperty.Locale.ZH;
    }
}