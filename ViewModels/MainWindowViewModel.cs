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
        UpdateValue();
    }
    private async Task UpdateValue() {
        void Log(string content, bool? isPrice = false) {
            if (isPrice ?? false) {
                Price = content;
                Console.WriteLine($"update {content}");
            }
            else {
                Console.WriteLine(content);
            }
        }
        Console.WriteLine("Start Fetching");
        var fetcher = new FetchData();
        await fetcher.FetchPoints(Log);
    }
    
    public event PropertyChangedEventHandler PropertyChanged;

    protected virtual void OnPropertyChanged(string propertyName)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}