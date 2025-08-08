using Avalonia.Controls;
using Avalonia.Input;

namespace GoldMonitor.Views;

public partial class DialogView : Window {
    public DialogView() {
        InitializeComponent();
    }

    private void TitleBar_PointerPressed(object? sender, PointerPressedEventArgs e) {
        if (e.GetCurrentPoint(null).Properties.IsLeftButtonPressed) BeginMoveDrag(e);
    }

    private void Close_PointerPressed(object? sender, PointerPressedEventArgs e) {
        if (e.GetCurrentPoint(null).Properties.IsLeftButtonPressed) Close();
    }
}