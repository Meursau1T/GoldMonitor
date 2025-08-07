using System;
using System.Threading.Tasks;
using System.Text.Json;
using System.Net.Http;

namespace GoldMonitor.Models;

public static class FetchPrice {
    public delegate void UpdaterDelegate(string content, bool? isPrice = false); 
    private static string ParsePrice(string raw) {
        using var doc = JsonDocument.Parse(raw);
        var root = doc.RootElement;
        var res = root.GetProperty("se")[0].ToString();
        return res;
    }

    private static async Task FetchRes(UpdaterDelegate updater) {
        using var httpClient = new HttpClient();
        // 来自 https://quote.fx678.com/symbol/AU9999
        const string url = "https://api-q.fx678img.com/getQuote.php?exchName=SGE&symbol=AU9999&st=0.39143745805336105";
        var requestUri = new Uri(url);

        // 设置自定义请求头
        httpClient.DefaultRequestHeaders.Clear();
        httpClient.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64; rv:141.0) Gecko/20100101 Firefox/141.0");
        httpClient.DefaultRequestHeaders.Add("Accept", "application/json, text/javascript, */*; q=0.01");
        httpClient.DefaultRequestHeaders.Add("Accept-Language", "zh-CN,en-US;q=0.7,en;q=0.3");
        // httpClient.DefaultRequestHeaders.Add("Accept-Encoding", "gzip, deflate, br, zstd"); // 注意：HttpClient 会自动处理压缩
        httpClient.DefaultRequestHeaders.Add("Origin", "https://quote.fx678.com");
        httpClient.DefaultRequestHeaders.Add("Sec-GPC", "1");
        httpClient.DefaultRequestHeaders.Add("Sec-Fetch-Dest", "empty");
        httpClient.DefaultRequestHeaders.Add("Sec-Fetch-Mode", "cors");
        httpClient.DefaultRequestHeaders.Add("Sec-Fetch-Site", "cross-site");
        httpClient.DefaultRequestHeaders.Add("Referer", "https://quote.fx678.com/");
        httpClient.DefaultRequestHeaders.Add("Priority", "u=4");
        httpClient.DefaultRequestHeaders.Add("Pragma", "no-cache");
        httpClient.DefaultRequestHeaders.Add("Cache-Control", "no-cache");

        try
        {
            // 发送 GET 请求
            var response = await httpClient.GetAsync(requestUri);

            // 确保请求成功
            response.EnsureSuccessStatusCode();

            // 读取响应内容
            var responseBody = await response.Content.ReadAsStringAsync();
            var parsedPrice = ParsePrice(responseBody);
            updater(parsedPrice, true);
        }
        catch (HttpRequestException e)
        {
            updater($"请求出错: {e.Message}");
        }
        catch (Exception e)
        {
            updater($"其他错误: {e.Message}");
        }
    }
    public static async Task UpdatePrice(UpdaterDelegate updater) {
        while (true)
        {
            await FetchRes(updater);
            await Task.Delay(TimeSpan.FromSeconds(10)); // 等待 5 秒
        }
    }
}