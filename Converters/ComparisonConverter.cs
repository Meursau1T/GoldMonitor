using System;
using System.Globalization;
using Avalonia.Data.Converters;

namespace GoldMonitor.Converters;

public class ComparisonConverter : IValueConverter
{
    public static readonly ComparisonConverter Instance = new();

    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        return value is Models.PriceComparison comp
            ? comp switch
            {
                Models.PriceComparison.LessThan => "<",
                Models.PriceComparison.GreaterThan => ">",
                _ => "?"
            }
            : "?";
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        return value is string str && str == ">"
            ? Models.PriceComparison.GreaterThan
            : Models.PriceComparison.LessThan;
    }
}