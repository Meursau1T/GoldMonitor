using System;
using System.Threading.Tasks;
using GoldMonitor.Models;

namespace GoldMonitor.ViewModels;

public class MainWindowViewModel : ViewModelBase {
    public MainWindowViewModel() {
        UpdateValue();
    }
    public string Price { get; set; } = "0";
    public string Points { get; set; } = "0";
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
}