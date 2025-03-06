using System.IO.Abstractions;
using System.IO.Abstractions.TestingHelpers;
using System.Net.WebSockets;
using EdServer.Interfaces;
using EdServer.Services;
using Moq;

namespace EdServer.Tests.Services;

public class JournalMonitorServiceTests
{
    private readonly Mock<IWebSocket> _webSocketMock = new();
    private readonly Mock<IFileSystemWatcher> _fileSystemWatcherMock = new();
    private readonly MockFileSystem _fileSystemMock;

    private readonly JournalMonitorService _journalMonitorService;

    public JournalMonitorServiceTests()
    {
        Mock<IFileSystemWatcherFactory> fileSystemWatcherFactoryMock = new Mock<IFileSystemWatcherFactory>();
        fileSystemWatcherFactoryMock.Setup(f => f.New()).Returns(_fileSystemWatcherMock.Object);
        _fileSystemMock = new MockFileSystem(new Dictionary<string, MockFileData>
        {
            { @"c:\Journal.test.log", new MockFileData("some js") },
        });
        
        _journalMonitorService = new JournalMonitorService(fileSystemWatcherFactoryMock.Object, _fileSystemMock);
    }
    
    [Fact]
    public void ShouldRaiseEventsWhenMonitoring()
    {
        _journalMonitorService.StartMonitoring(_webSocketMock.Object, @"c:\");
        
        _fileSystemWatcherMock.VerifySet(x => x.EnableRaisingEvents = true);
    }

    [Fact]
    public void ShouldNotRaiseEventsWhenNotMonitoring()
    {
        _journalMonitorService.StopMonitoring();
        
        _fileSystemWatcherMock.VerifySet(x => x.EnableRaisingEvents = false);
    }

    [Fact]
    public void ShouldMonitorPathUsingCorrectFilter()
    {
        _journalMonitorService.StartMonitoring(_webSocketMock.Object, @"c:\");
        
        _fileSystemWatcherMock.VerifySet(x => x.Path = @"c:\");
        _fileSystemWatcherMock.VerifySet(x => x.Filter = "Journal.*.log");
    }

    [Fact]
    public void ShouldSendNewDataToWebSocketOnFileChange()
    {
        _journalMonitorService.StartMonitoring(_webSocketMock.Object, @"c:\");
        
        _fileSystemWatcherMock.Raise(w => w.Changed += null, new FileSystemEventArgs(WatcherChangeTypes.Changed, @"c:\", "Journal.test.log"));
        _fileSystemMock.AddFile(@"c:\Journal.test.log", new MockFileData("some js.some more js"));
        _fileSystemWatcherMock.Raise(w => w.Changed += null, new FileSystemEventArgs(WatcherChangeTypes.Changed, @"c:\", "Journal.test.log"));

        var message = GetLastWebSocketMessageSent();
        Assert.Equal(".some more js", message);
    }

    [Fact]
    public void ShouldSendWholeFileToWebSocketOnFileCreate()
    {
        _journalMonitorService.StartMonitoring(_webSocketMock.Object, @"c:\");
        
        _fileSystemWatcherMock.Raise(w => w.Changed += null, new FileSystemEventArgs(WatcherChangeTypes.Changed, @"c:\", "Journal.test.log"));
        _fileSystemMock.AddFile(@"c:\Journal.test.log", new MockFileData("some new js."));
        _fileSystemWatcherMock.Raise(w => w.Created += null, new FileSystemEventArgs(WatcherChangeTypes.Changed, @"c:\", "Journal.test.log"));

        var message = GetLastWebSocketMessageSent();
        Assert.Equal("some new js.", message);
    }
    
    private string GetLastWebSocketMessageSent()
    {
        var arg = new ArgumentCaptor<ArraySegment<byte>>();
        _webSocketMock.Verify(s => s.SendAsync(arg.Capture(), WebSocketMessageType.Text, true, It.IsAny<CancellationToken>()));
        return System.Text.Encoding.Default.GetString(arg.Value.Array);
    }
}