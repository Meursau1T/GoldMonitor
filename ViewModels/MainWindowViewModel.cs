using System.Reactive;
using GoldMonitor.Common;
using GoldMonitor.Models;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace GoldMonitor.ViewModels;

public class MainWindowViewModel : ReactiveObject {
    private readonly GoldPriceService _goldPriceService;

    public MainWindowViewModel() {
        LocalePressed = ReactiveCommand.Create(ChangeCurrency);
        _goldPriceService = new GoldPriceService(
            onPriceChange: str => Price = str,
            onRateChange: str => Rate = str + "%"
        );
        _goldPriceService.GetVal();
    }

    [Reactive] public string Price { get; set; } = "Loading";

    [Reactive] public string Rate { get; set; } = "Loading";

    [Reactive] public GoldProperty.Locale Locale { get; set; } = GoldProperty.Locale.ZH;

    /* 区域点击事件 */
    public ReactiveCommand<Unit, Unit> LocalePressed { get; set; }

    private void ChangeCurrency() {
        if (Locale != GoldProperty.Locale.EN) {
            _goldPriceService.UpdateLocale(GoldProperty.Currency.USD, GoldProperty.Unit.OZ);
            Locale = GoldProperty.Locale.EN;
        } else {
            _goldPriceService.UpdateLocale(GoldProperty.Currency.CNY, GoldProperty.Unit.G);
            Locale = GoldProperty.Locale.ZH;
        }
    }
}