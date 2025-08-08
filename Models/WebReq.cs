using System;

namespace GoldMonitor.Models;

public class WebReq {
    protected readonly Func<string, bool> Checker;
    protected readonly Func<string, string> Parser;
    protected readonly Action<string> SuccessCallback;

    protected WebReq(
        string url,
        Action<string>? successCallback = null,
        Func<string, string>? parser = null,
        Func<string, bool>? checker = null) {
        Url = url;
        SuccessCallback = successCallback ?? (str => { });
        Checker = checker ?? (str => true);
        Parser = parser ?? (str => str);
    }

    protected string Url { get; init; }
}