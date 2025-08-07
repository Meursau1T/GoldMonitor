using System;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
namespace GoldMonitor.Models;

public class WsReq : WebReq {
    public WsReq(string url,
        Action<string>? successCallback = null,
        Func<string, string>? parser = null,
        Func<string, bool>? checker = null) : base(url, successCallback, parser, checker) { }
    
    private async Task ReceiveMessages(ClientWebSocket webSocket) {
        var buffer = new byte[1024 * 4];
        while (webSocket.State == WebSocketState.Open)
            try {
                var result = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);

                if (result.MessageType == WebSocketMessageType.Text) {
                    var rawMsg = Encoding.UTF8.GetString(buffer, 0, result.Count);
                    var msg = Parser(rawMsg);
                    if (Checker(msg)) {
                        SuccessCallback(msg);
                    }
                }
                else if (result.MessageType == WebSocketMessageType.Close) {
                    Console.WriteLine("收到关闭帧。");
                    await webSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, "收到关闭指令", CancellationToken.None);
                }
            }
            catch (OperationCanceledException) {
                Console.WriteLine("接收操作被取消。");
                break;
            }
            catch (Exception ex) {
                Console.WriteLine($"接收消息出错: {ex.Message}");
                break;
            }
    }

    public async Task WebSocketConn() {
        var clientWebSocket = new ClientWebSocket();
        /* 万洲黄金 */
        // var uri = new Uri("wss://alb-8yr5quj236kibwg1zd.cn-shenzhen.alb.aliyuncs.com:9701/");
        var uri = new Uri(Url);

        try {
            Console.WriteLine("正在连接到 WebSocket 服务器...");
            await clientWebSocket.ConnectAsync(uri, CancellationToken.None);
            Console.WriteLine("连接成功！");

            // 启动接收消息的任务
            var receiveTask = ReceiveMessages(clientWebSocket);
            // 等待接收任务（可以一直运行）
            await receiveTask;
        }
        catch (Exception ex) {
            Console.WriteLine($"发生错误: {ex.Message}");
        }
        finally {
            if (clientWebSocket.State == WebSocketState.Open ||
                clientWebSocket.State == WebSocketState.CloseReceived ||
                clientWebSocket.State == WebSocketState.CloseSent)
                await clientWebSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, "关闭连接", CancellationToken.None);
            clientWebSocket.Dispose();
            Console.WriteLine("WebSocket 已关闭。");
        }
    }
}