using System.Net.WebSockets;
using EdServer.Controllers;
using EdServer.Interfaces;
using Microsoft.AspNetCore.Http;
using Moq;

namespace EdServer.Tests.Controllers;

public class WebSocketControllerTests
{
    private readonly Mock<IWebSocketManager> _webSocketManagerMock = new();
    private readonly Mock<IJournalMonitorService> _journalMonitorServiceMock = new();
    
    private readonly WebSocketController _webSocketController;
    private readonly Mock<IWebSocket> _webSocketMock = new();
    private readonly Mock<IFileLocationService> _fileLocationServiceMock;

    public WebSocketControllerTests()
    {
        _fileLocationServiceMock = new Mock<IFileLocationService>();
        _fileLocationServiceMock.Setup(x => x.GetJournalLocation()).Returns(@"C:\");

        _webSocketMock.Setup(x => x.ReceiveAsync(It.IsAny<ArraySegment<byte>>(), CancellationToken.None))
            .ReturnsAsync(new WebSocketReceiveResult(0, WebSocketMessageType.Close, true, WebSocketCloseStatus.NormalClosure, "instant close for tests"));
        _webSocketManagerMock.Setup(x => x.GetWebSocket(It.IsAny<HttpContext>())).ReturnsAsync(_webSocketMock.Object);

        _webSocketController =
            new WebSocketController(_webSocketManagerMock.Object, _journalMonitorServiceMock.Object, _fileLocationServiceMock.Object);
        _webSocketController.ControllerContext.HttpContext = new DefaultHttpContext();
    }
    
    [Fact]
    public async Task ShouldReturnBadRequestIfNotWebSocketRequest()
    { 
        _webSocketManagerMock.Setup(m => m.IsWebSocketRequest(It.IsAny<HttpContext>())).Returns(false);
        
        await _webSocketController.Get();
        
        Assert.Equal(StatusCodes.Status400BadRequest, _webSocketController.HttpContext.Response.StatusCode);
    }

    [Fact]
    public async Task ShouldStartMonitoringAtCorrectLocation()
    {
        const string expectedLocation = @"C:\expected\location";
        _fileLocationServiceMock.Setup(x => x.GetJournalLocation()).Returns(expectedLocation);
        _webSocketManagerMock.Setup(m => m.IsWebSocketRequest(It.IsAny<HttpContext>())).Returns(true);
        
        await _webSocketController.Get();
        
        _journalMonitorServiceMock.Verify(x => x.StartMonitoring(It.IsAny<IWebSocket>(), expectedLocation));
    }
}