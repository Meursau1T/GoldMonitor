using System;
using System.Reactive;
using System.Threading.Tasks;
using ReactiveUI;

namespace GoldMonitor.ViewModels;

public class DialogViewModel {
    public DialogViewModel() {
        CloseCommand = ReactiveCommand.CreateFromTask(Close);
    }
    public ReactiveCommand<Unit, Unit> CloseCommand { get; }
    private async Task Close() {
        // 通知关闭
        await Task.Delay(1); // 保证命令完成
        OnCloseRequested?.Invoke();
    }
    public event Action OnCloseRequested;
}