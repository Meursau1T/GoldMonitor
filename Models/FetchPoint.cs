using System;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Text.Json;

namespace GoldMonitor.Models;

public static class FetchPoint {
    private struct Message {
        public string Type { get; init; }
        public string Price { get; init; }
    }
    public delegate void UpdaterDelegate(string content, bool? isPoint = false); 
    private static Message ParsePoints(string raw) {
        using var doc = JsonDocument.Parse(raw);
        var root = doc.RootElement;
        var firstItem = root[0]; // 获取第一个元素
        var val = new Message {
            Price = firstItem.GetProperty("bid").ToString() ?? "",
            Type = firstItem.GetProperty("symbol").GetString() ?? "",
        };
        return val;
    }
    private static bool IsGold(Message msg) {
        return msg.Type == "GOLD";
    }
    private static async Task ReceiveMessages(ClientWebSocket webSocket, UpdaterDelegate updater) {
        var buffer = new byte[1024 * 4];
        while (webSocket.State == WebSocketState.Open)
            try {
                var result = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);

                if (result.MessageType == WebSocketMessageType.Text) {
                    var rawMsg = Encoding.UTF8.GetString(buffer, 0, result.Count);
                    var msg = ParsePoints(rawMsg);
                    if (IsGold(msg)) {
                        updater(msg.Price, true);
                    }
                }
                else if (result.MessageType == WebSocketMessageType.Close) {
                    updater("收到关闭帧。");
                    await webSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, "收到关闭指令", CancellationToken.None);
                }
            }
            catch (OperationCanceledException) {
                updater("接收操作被取消。");
                break;
            }
            catch (Exception ex) {
                updater($"接收消息出错: {ex.Message}");
                break;
            }
    }

    public static async Task UpdatePoints(UpdaterDelegate updater) {
        var clientWebSocket = new ClientWebSocket();
        /* 万洲黄金 */
        var uri = new Uri("wss://alb-8yr5quj236kibwg1zd.cn-shenzhen.alb.aliyuncs.com:9701/");

        try {
            updater("正在连接到 WebSocket 服务器...");
            await clientWebSocket.ConnectAsync(uri, CancellationToken.None);
            updater("连接成功！");

            // 启动接收消息的任务
            var receiveTask = ReceiveMessages(clientWebSocket, updater);
            // 等待接收任务（可以一直运行）
            await receiveTask;
        }
        catch (Exception ex) {
            updater($"发生错误: {ex.Message}");
        }
        finally {
            if (clientWebSocket.State == WebSocketState.Open ||
                clientWebSocket.State == WebSocketState.CloseReceived ||
                clientWebSocket.State == WebSocketState.CloseSent)
                await clientWebSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, "关闭连接", CancellationToken.None);
            clientWebSocket.Dispose();
            updater("WebSocket 已关闭。");
        }
    }
}