using System.Net.WebSockets;
using EdServer.Interfaces;

namespace EdServer.Adapters;

public class WebSocketAdapter : IWebSocket, IDisposable
{
    private readonly WebSocket _webSocket;

    public WebSocketAdapter(WebSocket webSocket)
    {
        _webSocket = webSocket;
    }
    
    public async Task<WebSocketReceiveResult> ReceiveAsync(ArraySegment<byte> buffer, CancellationToken cancellationToken)
    {
        return await _webSocket.ReceiveAsync(buffer, cancellationToken);
    }

    public async Task SendAsync(ArraySegment<byte> buffer, WebSocketMessageType messageType, bool endOfMessage,
        CancellationToken cancellationToken)
    {
        await _webSocket.SendAsync(buffer, messageType, endOfMessage, cancellationToken);
    }

    public void Dispose()
    {
        _webSocket.Dispose();
        GC.SuppressFinalize(this);
    }
}