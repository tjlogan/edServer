using EdServer.Interfaces;

namespace EdServer.Adapters;

public class WebSocketManagerAdapter : IWebSocketManager
{
    public bool IsWebSocketRequest(HttpContext context)
    {
        return context.WebSockets.IsWebSocketRequest;
    }

    public async Task<IWebSocket> GetWebSocket(HttpContext context)
    {
        return new WebSocketAdapter(await context.WebSockets.AcceptWebSocketAsync());
    }
}