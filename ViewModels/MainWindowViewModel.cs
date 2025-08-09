using System.Reactive;
using System.Reactive.Disposables;
using System.Runtime.CompilerServices;
using GoldMonitor.Common;
using GoldMonitor.Models;
using GoldMonitor.Services;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace GoldMonitor.ViewModels;

public class MainWindowViewModel : ReactiveObject, IActivatableViewModel {
    public ViewModelActivator Activator { get; }
    public MainWindowViewModel() {
        LocalePressed = ReactiveCommand.Create(ChangeCurrency);
        Activator = new ViewModelActivator();
        this.WhenActivated((CompositeDisposable disposables) =>
        {
            /* handle activation */
            Disposable
                .Create(() => { /* handle deactivation */ })
                .DisposeWith(disposables);
        });
        new GoldPriceService().GetVal(OnSuccess);
    }
    [Reactive] public string Price { get; set; } = "Loading";
    [Reactive] public string Rate { get; set; } = "Loading";
    [Reactive] private GoldProperty.Locale Locale { get; set; } = GoldProperty.Locale.ZH;
     
    /* 区域点击事件 */
    public ReactiveCommand<Unit, Unit> LocalePressed { get; set; }
    private void OnSuccess(GoldStatus res) {
        if (res.Locale != Locale || res.Price.Length == 0 || res.Rate.Length == 0) {
            return;
        }
        Price = res.Price;
        Rate = res.Rate + "%"; 
    }

    private void ChangeCurrency() {
        Locale = Locale == GoldProperty.Locale.EN
            ? GoldProperty.Locale.ZH
            : GoldProperty.Locale.EN;
        var target = Locale == GoldProperty.Locale.ZH ? CurrentPrice.ZhRecord :  CurrentPrice.EnRecord;
        Price = target?.Price ?? "Loading";
        Rate = target?.Rate == null ? "Loading" : target.Rate + "%";
    }
}