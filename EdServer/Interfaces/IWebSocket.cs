using System.Net.WebSockets;

namespace EdServer.Interfaces;

public interface IWebSocket : IDisposable
{
    Task<WebSocketReceiveResult> ReceiveAsync(ArraySegment<byte> buffer, 
        CancellationToken cancellationToken);

    Task SendAsync(ArraySegment<byte> buffer,
        WebSocketMessageType messageType,
        bool endOfMessage,
        CancellationToken cancellationToken);
}