using System;
using System.ComponentModel;
using System.Reactive;
using GoldMonitor.Helpers;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace GoldMonitor.ViewModels;

public class MainWindowViewModel : ReactiveObject {
    [Reactive]
    public string Price { get; set; } = "";
    [Reactive]
    public string Rate { get; set; } = "";
    [Reactive]
    public string Locale { get; set; } = "CN";

    private GoldPrice _goldPrice;

    public MainWindowViewModel() {
        LocalePressed = ReactiveCommand.Create(ChangeCurrency);
        _goldPrice = new GoldPrice(
            onPriceChange: (str) => Price = str,
            onRateChange: (str) => Rate = str + "%"
            );
        _goldPrice.GetVal();
    }

    /* 区域点击事件 */
    public ReactiveCommand<Unit, Unit> LocalePressed { get; set; }
    private void ChangeCurrency() {
        if (Locale != "EN") { 
            _goldPrice.UpdateLocale(GoldPrice.Currency.USD, GoldPrice.Unit.OZ);
            Locale = "EN";
        } else {
            _goldPrice.UpdateLocale(GoldPrice.Currency.CNY, GoldPrice.Unit.G);
            Locale = "CN";
        }
    }
}