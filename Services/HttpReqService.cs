using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace GoldMonitor.Services;

public class HttpReqService<T> : WebReqService<T> {
    private bool _isAutoFetchCanceled;
    private readonly Action<HttpClient>? ConfigHttpClient;

    public HttpReqService(
        string url,
        Action<T>? successCallback = null,
        Func<string, T>? parser = null,
        Func<T, bool>? checker = null,
        Action<HttpClient>? configHttpClient = null) : base(url, successCallback, parser, checker) {
        ConfigHttpClient = configHttpClient ?? (client => { });
    }

    public async Task Fetch() {
        using var httpClient = new HttpClient();
        var requestUri = new Uri(Url);
        ConfigHttpClient?.Invoke(httpClient);
        // 设置自定义请求头

        try {
            // 发送 GET 请求
            var response = await httpClient.GetAsync(requestUri);

            // 确保请求成功
            response.EnsureSuccessStatusCode();

            // 读取响应内容
            var responseBody = await response.Content.ReadAsStringAsync();
            var parsedPrice = Parser(responseBody);
            SuccessCallback(parsedPrice);
        } catch (HttpRequestException e) {
            Console.WriteLine($"请求出错: {e.Message}");
        } catch (Exception e) {
            Console.WriteLine($"其他错误: {e.Message}");
        }
    }

    public async Task AutoFetch(int timeout = 10) {
        _isAutoFetchCanceled = false;
        while (_isAutoFetchCanceled == false) {
            await Fetch();
            await Task.Delay(TimeSpan.FromSeconds(timeout)); // 等待 5 秒
        }
    }

    public void StopAutoFetch() {
        _isAutoFetchCanceled = true;
    }
}