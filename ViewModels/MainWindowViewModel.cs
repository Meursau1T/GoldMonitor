using System;
using System.Threading.Tasks;
using System.ComponentModel;
using GoldMonitor.Models;

namespace GoldMonitor.ViewModels;

public class MainWindowViewModel : INotifyPropertyChanged {
    private string price = "0";
    public string Price {
        get => price;
        set {
            price = value;
            OnPropertyChanged(nameof(Price));
        }
    }
    private string points = "0";
    public string Points {
        get => points;
        set {
            points = value;
            OnPropertyChanged(nameof(Points));
        }
    }
    public MainWindowViewModel() {
        UpdatePoint();
        UpdatePrice();
    }
    private async Task UpdatePoint() {
        void Callback(string content, bool? isPrice = false) {
            if (isPrice ?? false) {
                Points = content;
                // Console.WriteLine($"update {content}");
            }
            else {
                // Console.WriteLine(content);
            }
        }
        await FetchPoint.UpdatePoints(Callback);
    }
    private async Task UpdatePrice() {
        void Callback(string content, bool? isPrice = false) {
            if (isPrice ?? false) {
                Price = content;
                // Console.WriteLine($"update {content}");
            }
            else {
                // Console.WriteLine(content);
            }
        }
        await FetchPrice.UpdatePrice(Callback);
    }
    
    public event PropertyChangedEventHandler PropertyChanged;

    protected virtual void OnPropertyChanged(string propertyName)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}