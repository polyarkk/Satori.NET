using System.Text.Json;
using Satori.Client.Internal;

namespace Satori.Client;

public class SatoriClient : IDisposable {
    internal readonly ISatoriApiService ApiService;

    internal readonly ISatoriEventService EventService;

    internal readonly static JsonSerializerOptions JsonOptions = new() {
        PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower,
    };

    public event EventHandler<SatoriClientLog>? Logging;

    public SatoriClient(Uri baseUri, string? token = null) {
        ApiService = new SatoriHttpApiService(baseUri, token, this);
        EventService = new SatoriWebSocketEventService(baseUri, token, this);
        Log(LogLevel.Information, "Satori client is now running!");
        EventService.StartAsync();
    }

    internal void Log(LogLevel logLevel, string message) {
        Logging?.Invoke(this, new SatoriClientLog(logLevel, message));
    }

    internal void Log(Exception e) {
        Logging?.Invoke(this, new SatoriClientLog(e));
    }

    /// <summary>
    /// 获取服务端 Bot，在 WebSocket 获取到 Platform 与 SelfId 之前将持续阻塞进程
    /// </summary>
    /// <returns>Bot</returns>
    public SatoriBot Bot(string platform, string selfId) {
        return new SatoriBot(this, platform, selfId);
    }

    public void Dispose() {
        Log(LogLevel.Information, "Satori client is now stopping!");
        EventService.StopAsync();
        EventService.Dispose();
        GC.SuppressFinalize(this);
    }
}
