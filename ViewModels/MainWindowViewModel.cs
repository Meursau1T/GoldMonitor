using System;
using System.ComponentModel;
using GoldMonitor.Helpers;

namespace GoldMonitor.ViewModels;

public class MainWindowViewModel : INotifyPropertyChanged {
    private string _price = "0";
    public string Price {
        get => _price;
        set {
            _price = value;
            OnPropertyChanged(nameof(Price));
        }
    }
    private string _rate = "0";
    public string Rate {
        get => _rate;
        set {
            _rate = value;
            OnPropertyChanged(nameof(Rate));
        }
    }

    private string _locale = "CN";

    public string Locale {
        get => _locale;
        set {
            _locale = value;
            OnPropertyChanged(nameof(Locale));
        }
    }
    private GoldPrice _goldPrice;

    public MainWindowViewModel() {
        _goldPrice = new GoldPrice(
            onPriceChange: (str) => Price = str,
            onRateChange: (str) => Rate = str + "%"
            );
        _goldPrice.GetVal();
    }

    public void ChangeCurrency() {
        if (Locale != "EN") { 
            _goldPrice.UpdateLocale(GoldPrice.Currency.USD, GoldPrice.Unit.OZ);
            Locale = "EN";
        } else {
            _goldPrice.UpdateLocale(GoldPrice.Currency.CNY, GoldPrice.Unit.G);
            Locale = "CN";
        }
    }

    public event PropertyChangedEventHandler PropertyChanged;

    protected virtual void OnPropertyChanged(string propertyName)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}