using System;
using System.Net.Http;
using System.Threading.Tasks;
using HtmlAgilityPack;

public class FetchData {
    private static readonly HttpClient _httpClient = new();

    public static async Task<string> FetchPoints(Action<string> log) {
        log("hello");
        try {
            // 抓取网页内容
            var url = "https://www.wzg.com/gold/";
            var htmlContent = await _httpClient.GetStringAsync(url);

            // 使用 HtmlAgilityPack 解析 HTML
            var htmlDocument = new HtmlDocument();
            htmlDocument.LoadHtml(htmlContent);

            // 查找 ID 为 goldprice 的元素
            var goldPriceElement = htmlDocument.GetElementbyId("goldprice");

            if (goldPriceElement != null) return goldPriceElement.InnerText.Trim();

            log("未找到 ID 为 'goldprice' 的元素");
            return "未找到 ID 为 'goldprice' 的元素";
        }
        catch (HttpRequestException ex) {
            log($"请求错误: {ex.Message}");
            return $"请求错误: {ex.Message}";
        }
        catch (Exception ex) {
            log($"错误: {ex.Message}");
            return $"发生错误: {ex.Message}";
        }
    }

    public static string FetchPrice() {
        Console.WriteLine("HELLO WORLD");
        return "HELLO WORLD";
    }
}