using System;
using Avalonia.Controls;
using Avalonia.Input;

namespace GoldMonitor.Views;

public partial class MainWindow : Window {
    public MainWindow() {
        InitializeComponent();
    }
    private void TitleBar_PointerPressed(object? sender, PointerPressedEventArgs e)
    {
        if (e.GetCurrentPoint(null).Properties.IsLeftButtonPressed)
        {
            BeginMoveDrag(e);
        }
    }
}