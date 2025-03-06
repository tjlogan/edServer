namespace EdServer.Interfaces;

public interface IWebSocketManager
{
    bool IsWebSocketRequest(HttpContext context);
    Task<IWebSocket> GetWebSocket(HttpContext context);
}