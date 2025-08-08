using System;

namespace GoldMonitor.Services;

public class WebReqService<T> {
    protected readonly Func<T, bool> Checker;
    protected readonly Func<string, T> Parser;
    protected readonly Action<T> SuccessCallback;

    protected WebReqService(
        string url,
        Action<T>? successCallback = null,
        Func<string, T>? parser = null,
        Func<T, bool>? checker = null) {
        Url = url;
        SuccessCallback = successCallback ?? (str => { });
        Checker = checker ?? (str => throw new ArgumentNullException(nameof(checker)));
        Parser = parser ?? (str => throw new ArgumentNullException(nameof(parser)));
    }

    protected string Url { get; init; }
}