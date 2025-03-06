using System.IO.Abstractions;
using System.Net.WebSockets;
using System.Text;
using EdServer.Interfaces;

namespace EdServer.Services;

public class JournalMonitorService : IJournalMonitorService
{
    private IWebSocket? _webSocket;
    private readonly IFileSystemWatcher _watcher;
    private long _lastRead;
    private readonly IFileSystem _fileSystem;

    public JournalMonitorService(IFileSystemWatcherFactory fileSystemWatcherFactory, IFileSystem fileSystem)
    {
        _fileSystem = fileSystem;
        _watcher = fileSystemWatcherFactory.New();
    }

    public void StartMonitoring(IWebSocket webSocket, string path)
    {
        _webSocket = webSocket;
        _watcher.Path = path;
        _watcher.Filter = "Journal.*.log";
        _watcher.EnableRaisingEvents = true;
        _watcher.Changed += JournalChangedEvent;
        _watcher.Created += JournalCreatedEvent;
    }

    public void StopMonitoring()
    {
        _watcher.EnableRaisingEvents = false;
    }

    private void JournalCreatedEvent(object sender, FileSystemEventArgs e)
    {
        HandleJournalChangedEvent(e.FullPath, 0);
    }

    private void JournalChangedEvent(object sender, FileSystemEventArgs e)
    {
        HandleJournalChangedEvent(e.FullPath, _lastRead);
    }

    private void HandleJournalChangedEvent(string fullPath, long fileOffset)
    {
        using var fileStream = _fileSystem.File.Open(fullPath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
        fileStream.Seek(fileOffset, SeekOrigin.Begin);
        var streamReader = new StreamReader(fileStream);
        var newData = streamReader.ReadToEnd();
        _lastRead = streamReader.BaseStream.Position;   
        _webSocket.SendAsync(new ArraySegment<byte>(Encoding.Default.GetBytes(newData)), WebSocketMessageType.Text, true, CancellationToken.None);
    }
}