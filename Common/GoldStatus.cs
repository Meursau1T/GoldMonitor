using GoldMonitor.Models;

namespace GoldMonitor.Common;

public class GoldStatus(string price, string rate, GoldProperty.Locale locale) {
    public string Price = price;
    public string Rate = rate;
    public GoldProperty.Locale Locale = locale;
};