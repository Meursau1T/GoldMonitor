using System;
using System.Globalization;
using Avalonia.Data.Converters;
using Avalonia.Media;

namespace GoldMonitor.Converters;

public class TextToBrushConverter : IValueConverter {
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture) {
        if (value is string text)
            // 判断文本是否以 "-" 开头
            return text.StartsWith('-')
                ? Brushes.Green
                : Brushes.Red;

        return Brushes.Red; // 默认红色
    }

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture) {
        throw new NotSupportedException();
    }
}