using System;
using System.Net.Http;
using System.Threading.Tasks;
using HtmlAgilityPack;
using GoldMonitor.ViewModels;

public class FetchData
{
    private static readonly HttpClient _httpClient = new HttpClient();
    static async Task<string> FetchPoints() {
        try {
            // 抓取网页内容
            string url = "https://www.wzg.com/gold/";
            string htmlContent = await _httpClient.GetStringAsync(url);

            // 使用 HtmlAgilityPack 解析 HTML
            var htmlDocument = new HtmlDocument();
            htmlDocument.LoadHtml(htmlContent);

            // 查找 ID 为 goldprice 的元素
            var goldPriceElement = htmlDocument.GetElementbyId("goldprice");

            if (goldPriceElement != null) {
                return goldPriceElement.InnerText.Trim();
            }
            else {
                return "未找到 ID 为 'goldprice' 的元素";
            }
        }
        catch (HttpRequestException ex) {
            return $"请求错误: {ex.Message}";
        }
        catch (Exception ex) {
            return $"发生错误: {ex.Message}";
        }
    }
    string FetchPrice() {
        return "";
    }
    public async Task updatePoints(MainWindowViewModel viewModel) {
        viewModel.Points = await FetchPoints();
        // viewModel.Price = FetchPrice();
    }
}