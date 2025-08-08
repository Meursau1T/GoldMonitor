using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Reactive;
using System.Threading.Tasks;
using GoldMonitor.Common;
using GoldMonitor.Models;
using ReactiveUI;

namespace GoldMonitor.ViewModels;

public class DialogViewModel : ReactiveObject {
    private readonly Config _configModel;
    private PriceComparison _newComparison = PriceComparison.LessThan;
    private GoldProperty.Locale _newLocale = GoldProperty.Locale.ZH;
    private decimal _newPriceThreshold;
    public IReadOnlyList<PriceComparison> ComparisonOptions { get; set; } = new[]
    {
        PriceComparison.LessThan,
        PriceComparison.GreaterThan
    };
 
    public IReadOnlyList<GoldProperty.Locale> LocaleOptions { get; set; } = new[]
    {
        GoldProperty.Locale.EN,
        GoldProperty.Locale.ZH,
    };

    public DialogViewModel() {
        _configModel = new Config();

        // 初始化命令
        AddCommand = ReactiveCommand.CreateFromTask(ExecuteAdd);
        RemoveCommand = ReactiveCommand.CreateFromTask<AlertItemViewModel>(ExecuteRemove);

        // 加载数据
        LoadData();
    }

    public ObservableCollection<AlertItemViewModel> Alerts { get; set; } = new();

    // Commands
    public ReactiveCommand<Unit, Unit> AddCommand { get; }
    public ReactiveCommand<AlertItemViewModel, Unit> RemoveCommand { get; }

    // 新增行绑定属性
    public decimal NewPriceThreshold {
        get => _newPriceThreshold;
        set => this.RaiseAndSetIfChanged(ref _newPriceThreshold, value);
    }

    public PriceComparison NewComparison {
        get => _newComparison;
        set => this.RaiseAndSetIfChanged(ref _newComparison, value);
    }

    public GoldProperty.Locale NewLocale {
        get => _newLocale;
        set => this.RaiseAndSetIfChanged(ref _newLocale, value);
    }

    private async Task LoadData() {
        await _configModel.LoadAsync();

        foreach (var alert in _configModel)
            Alerts.Add(new AlertItemViewModel(alert, async item => {
                Alerts.Remove(item);
                await _configModel.SaveAsync(); // 删除后立即保存
            }));
    }

    private async Task ExecuteAdd() {
        if (NewPriceThreshold <= 0) return;

        var newAlert = new GoldAlertConfig {
            PriceThreshold = NewPriceThreshold,
            Comparison = NewComparison,
            Locale = NewLocale
        };

        _configModel.Alerts.Add(newAlert);

        // 添加到 UI
        Alerts.Add(new AlertItemViewModel(newAlert, async item => {
            Alerts.Remove(item);
            await _configModel.SaveAsync();
        }));

        // 保存
        await _configModel.SaveAsync();

        // 重置输入
        NewPriceThreshold = 0;
        NewComparison = PriceComparison.LessThan;
        NewLocale = GoldProperty.Locale.ZH;
    }

    private async Task ExecuteRemove(AlertItemViewModel item) {
        if (item?.Model != null) {
            _configModel.Alerts.Remove(item.Model);
            Alerts.Remove(item);
            await _configModel.SaveAsync();
        }
    }
}

// 子项 ViewModel，每行一个
public class AlertItemViewModel : ReactiveObject {
    private readonly Action<AlertItemViewModel> _onRemove;

    public AlertItemViewModel(GoldAlertConfig model, Action<AlertItemViewModel> onRemove) {
        Model = model;
        _onRemove = onRemove;

        RemoveCommand = ReactiveCommand.Create(() => _onRemove(this));
    }

    public GoldAlertConfig Model { get; }

    public decimal PriceThreshold => Model.PriceThreshold;

    public string ComparisonSymbol => Model.Comparison switch {
        PriceComparison.LessThan => "<",
        PriceComparison.GreaterThan => ">",
        _ => "?"
    };

    public string LocaleText => Model.Locale switch {
        GoldProperty.Locale.ZH => "ZH",
        GoldProperty.Locale.EN => "EN",
        _ => "??"
    };

    public ReactiveCommand<Unit, Unit> RemoveCommand { get; }
}