using System;
using System.Text.Json;
using System.Threading.Tasks;
using GoldMonitor.Common;
using GoldMonitor.Models;

namespace GoldMonitor.Services;

public class GoldPriceService {

    private const int Timeout = 5;

    private HttpReqService<GoldStatus>? _enFetcher;
    private HttpReqService<GoldStatus>? _zhFetcher;
    public async Task GetVal(Action<GoldStatus> successCallback) {
        _enFetcher = CreateFetcher(nameof(GoldProperty.Currency.USD), GoldProperty.Locale.EN);
        _zhFetcher = CreateFetcher(nameof(GoldProperty.Currency.CNY), GoldProperty.Locale.ZH);
        await Task.WhenAll(
            _enFetcher.AutoFetch(Timeout),
            _zhFetcher.AutoFetch(Timeout)
        );
        return;
        
        string OzToGram(string str) {
            return decimal.TryParse(str, out var num) ? (num / 31.1035m).ToString("F2") : str;
        }

        string Fix2(string str) {
            // 格式化为最多两位小数
            return decimal.TryParse(str, out var number) ? number.ToString("F2") : "NaN";
        }

        GoldStatus Parse(string raw, GoldProperty.Locale locale) {
            Console.WriteLine($"Request result in {nameof(locale)}");
            Console.WriteLine(raw);
            Console.WriteLine();
            try {
                /*
                 {
                      "curr": "CNY",           // 货币单位：人民币
                      "xauPrice": 24283.1758,  // 当前黄金价格（XAU）：每金衡盎司的人民币价格
                      "chgXau": 141.8101,      // 每盎司黄金价格变动（较上一交易日）：+141.81元
                      "pcXau": 0.5874,         // 黄金价格变动百分比：+0.5874%
                      "xauClose": 24141.36574, // 上一交易日黄金收盘价（用于计算变动）
                    }
                 */
                using var doc = JsonDocument.Parse(raw);
                var root = doc.RootElement;
                var items = root.GetProperty("items");
                if (items.GetArrayLength() == 0) {
                    return new GoldStatus(price: "", rate: "", locale: locale);
                }
                var pricePerOz = items[0].GetProperty("xauPrice").ToString();
                var changeRate = items[0].GetProperty("pcXau").ToString();
                var priceRes = locale == GoldProperty.Locale.ZH ? OzToGram(pricePerOz) : pricePerOz;
                return new GoldStatus(price: Fix2(priceRes), rate: Fix2(changeRate), locale: locale);

            } catch (Exception e) {
                Console.WriteLine($"GoldPrice Parse Error: {e.Message}");
                return new GoldStatus(price: "", rate: "", locale: locale);
            }
        }
        HttpReqService<GoldStatus> CreateFetcher(string currency, GoldProperty.Locale locale)
            => new (
                url: $"https://data-asg.goldprice.org/dbXRates/{currency}",
                parser: str => Parse(str, locale),
                successCallback: res => {
                    if (res.Locale == GoldProperty.Locale.EN) {
                        CurrentPrice.EnRecord = res;
                    } else {
                        CurrentPrice.ZhRecord = res; 
                    }

                    successCallback(res);
                }
            );
        
    }
}