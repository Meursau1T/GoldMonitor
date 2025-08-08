// Models/ConfigModel.cs

using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;
using GoldMonitor.Common;

namespace GoldMonitor.Models;

public enum PriceComparison {
    GreaterThan,
    LessThan
}

public class GoldAlertConfig {
    public decimal PriceThreshold { get; set; }
    public PriceComparison Comparison { get; set; }
    public GoldProperty.Locale Locale { get; set; }
}

public class Config : IEnumerable<GoldAlertConfig> {
    private static readonly string ConfigDirectory = Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "GoldMonitor");

    private static readonly string ConfigFilePath = Path.Combine(ConfigDirectory, "alerts.json");

    private static readonly JsonSerializerOptions JsonOptions = new() {
        WriteIndented = true,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    };

    public Config() {
        LoadAsync();
    }

    public List<GoldAlertConfig> Alerts { get; } = new();

    public int Count => Alerts.Count;

    public IEnumerator<GoldAlertConfig> GetEnumerator() {
        return Alerts.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator() {
        return GetEnumerator();
    }

    public async Task LoadAsync() {
        try {
            if (!Directory.Exists(ConfigDirectory)) {
                Directory.CreateDirectory(ConfigDirectory);
                return; // 空配置
            }

            if (!File.Exists(ConfigFilePath)) return; // 保持空列表

            await using var stream = File.OpenRead(ConfigFilePath);
            var loaded = await JsonSerializer.DeserializeAsync<SerializableWrapper>(stream, JsonOptions);

            Alerts.Clear();
            if (loaded?.Alerts != null) Alerts.AddRange(loaded.Alerts);
        } catch (Exception ex) {
            Console.WriteLine($"[Config] Failed to load config: {ex.Message}");
            // 可以选择抛出，或保持空列表
        }
    }

    public async Task SaveAsync() {
        try {
            if (!Directory.Exists(ConfigDirectory)) Directory.CreateDirectory(ConfigDirectory);

            var wrapper = new SerializableWrapper { Alerts = Alerts };
            await using var stream = File.Create(ConfigFilePath);
            await JsonSerializer.SerializeAsync(stream, wrapper, JsonOptions);
        } catch (Exception ex) {
            Console.WriteLine($"[Config] Failed to save config: {ex.Message}");
            throw; // 保存失败应通知调用者
        }
    }

    // --- 便捷方法（可选） ---
    public void Add(GoldAlertConfig config) {
        Alerts.Add(config);
    }

    public bool Remove(GoldAlertConfig config) {
        return Alerts.Remove(config);
    }

    public void Clear() {
        Alerts.Clear();
    }

    private class SerializableWrapper {
        public List<GoldAlertConfig> Alerts { get; set; }
    }
}