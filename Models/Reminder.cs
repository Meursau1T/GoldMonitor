using GoldMonitor.Common;

namespace GoldMonitor.Models;

public class Reminder
{
    private decimal PriceThreshold { get; set; }

    private PriceComparison Comparison { get; set; }
    private GoldProperty.Locale Locale { get; set; }
    protected Reminder(decimal priceThreshold, PriceComparison comparison)
    {
        PriceThreshold = priceThreshold;
        Comparison = comparison;
    }
    public bool ShouldTrigger(decimal currentGoldPrice)
    {
        return Comparison switch
        {
            PriceComparison.Greater => currentGoldPrice > PriceThreshold,
            PriceComparison.Less => currentGoldPrice < PriceThreshold,
            _ => false
        };
    }

    public override string ToString()
    {
        var condition = Comparison switch
        {
            PriceComparison.Greater => $"金价大于 {PriceThreshold} 元",
            PriceComparison.Less => $"金价小于 {PriceThreshold} 元",
            _ => $"未知条件 {PriceThreshold}"
        };
        return $"提醒：{condition}";
    }
}

public enum PriceComparison
{
    Greater,
    Less
}