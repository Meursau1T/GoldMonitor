using System;
using GoldMonitor.ViewModels;

public class FetchData
{
    string FetchPoints()
    {
        return "";
    }
    string FetchPrice()
    {
        return "";
    }
    public void updatePoints(MainWindowViewModel viewModel)
    {
        viewModel.Points = FetchPoints();
        viewModel.Price = FetchPrice();
    }
}