namespace GoldMonitor.Common;

public class GoldStatus(string price, string rate, GoldProperty.Locale locale) {
    public readonly GoldProperty.Locale Locale = locale;
    public readonly string Price = price;
    public readonly string Rate = rate;
}