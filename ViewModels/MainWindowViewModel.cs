using System;
using System.Threading.Tasks;

namespace GoldMonitor.ViewModels;

public class MainWindowViewModel : ViewModelBase {
    public MainWindowViewModel() {
        UpdatePoints();
    }

    public string Price { get; set; } = "0";
    public string Points { get; set; } = "0";

    public async Task UpdatePoints() {
        Points = "Fetching";
        Console.WriteLine("Start Fetching");
        var log = (string content) => { Points = content; };
        Price = await FetchData.FetchPoints(log);
    }
}