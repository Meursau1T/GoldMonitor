using GoldMonitor.Common;

namespace GoldMonitor.Models;

public static class CurrentPrice {
    public static GoldStatus? ZhRecord { get; set; } = null;
    public static GoldStatus? EnRecord { get; set; } = null;
}