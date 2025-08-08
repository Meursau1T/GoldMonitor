using System.Reactive;
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

    [Reactive] public string Locale { get; set; } = "CN";

    /* 区域点击事件 */
    public ReactiveCommand<Unit, Unit> LocalePressed { get; set; }

    private void ChangeCurrency() {
        if (Locale != "EN") {
            _goldPriceService.UpdateLocale(GoldPriceService.Currency.USD, GoldPriceService.Unit.OZ);
            Locale = "EN";
        } else {
            _goldPriceService.UpdateLocale(GoldPriceService.Currency.CNY, GoldPriceService.Unit.G);
            Locale = "CN";
        }
    }
}