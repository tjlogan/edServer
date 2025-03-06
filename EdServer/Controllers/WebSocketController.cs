using EdServer.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace EdServer.Controllers;

public class WebSocketController : ControllerBase
{
    private readonly IWebSocketManager _webSocketManager;
    private readonly IJournalMonitorService _journalMonitorService;
    private readonly IFileLocationService _fileLocationService;

    public WebSocketController(IWebSocketManager webSocketManager, IJournalMonitorService journalMonitorService, IFileLocationService fileLocationService)
    {
        _webSocketManager = webSocketManager;
        _journalMonitorService = journalMonitorService;
        _fileLocationService = fileLocationService;
    }
    
    [Route("/ws")]
    public async Task Get()
    {
        if (_webSocketManager.IsWebSocketRequest(HttpContext))
        {
            using var webSocket = await _webSocketManager.GetWebSocket(HttpContext);
            await MonitorJournals(webSocket);
        }
        else
        {
            HttpContext.Response.StatusCode = StatusCodes.Status400BadRequest;
        }
    }

    private async Task MonitorJournals(IWebSocket webSocket)
    {
        _journalMonitorService.StartMonitoring(webSocket, _fileLocationService.GetJournalLocation());
        await WaitForWebSocketClose(webSocket);
        _journalMonitorService.StopMonitoring();
    }

    private static async Task WaitForWebSocketClose(IWebSocket webSocket)
    {
        // ReceiveAsync loop to keep middleware pipeline running 
        var buffer = new byte[1024 * 4];
        var receiveResult = await webSocket.ReceiveAsync(
            new ArraySegment<byte>(buffer), CancellationToken.None);
        while (!receiveResult.CloseStatus.HasValue)
        {
            receiveResult = await webSocket.ReceiveAsync(
                new ArraySegment<byte>(buffer), CancellationToken.None);
        }
    }
}