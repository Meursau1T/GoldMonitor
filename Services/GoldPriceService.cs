using System;
using System.Text.Json;
using System.Threading.Tasks;
using GoldMonitor.Common;

namespace GoldMonitor.Models;

public class GoldPriceService {

    private const int Timeout = 5;

    private GoldProperty.Currency _currCurrency;
    private GoldProperty.Unit _currUnit;
    private HttpReqService? _fetcher;
    private readonly Action<string> _onPriceChange;
    private readonly Action<string> _onRateChange;

    public GoldPriceService(
        GoldProperty.Currency? currency = null,
        GoldProperty.Unit? unit = null,
        Action<string>? onRateChange = null,
        Action<string>? onPriceChange = null
    ) {
        _currCurrency = currency ?? GoldProperty.Currency.CNY;
        _currUnit = unit ?? GoldProperty.Unit.G;
        _onRateChange = onRateChange ?? (str => { });
        _onPriceChange = onPriceChange ?? (str => { });
    }

    public void UpdateLocale(GoldProperty.Currency? currency = null, GoldProperty.Unit? unit = null) {
        Console.WriteLine($"{currency.ToString()} - {unit}");
        _currCurrency = currency ?? _currCurrency;
        _currUnit = unit ?? _currUnit;
        _fetcher.StopAutoFetch();
        _fetcher = null;
        GetVal();
    }

    public async Task GetVal() {
        string OzToGram(string str) {
            decimal num;
            return decimal.TryParse(str, out num) ? (num / 31.1035m).ToString("F2") : str;
        }

        string Fix2(string str) {
            if (decimal.TryParse(str, out var number))
                // 格式化为最多两位小数
                return number.ToString("F2");

            return "NaN";
        }

        string Parse(string raw) {
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
                if (items.GetArrayLength() > 0) {
                    var pricePerOz = items[0].GetProperty("xauPrice").ToString();
                    var changeRate = items[0].GetProperty("pcXau").ToString();
                    var priceRes = _currUnit == GoldProperty.Unit.G ? OzToGram(pricePerOz) : pricePerOz;
                    _onPriceChange(Fix2(priceRes));
                    _onRateChange(Fix2(changeRate));
                    return priceRes;
                }

                return "NaN";
            } catch (Exception e) {
                Console.WriteLine($"GoldPrice Parse Error: {e.Message}");
                return "NaN";
            }
        }

        _fetcher = new HttpReqService(
            $"https://data-asg.goldprice.org/dbXRates/{_currCurrency.ToString()}",
            parser: Parse
        );
        await _fetcher.AutoFetch(Timeout);
    }
}