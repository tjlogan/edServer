namespace EdServer.Interfaces;

public interface IJournalMonitorService
{
    void StartMonitoring(IWebSocket socket, string journalFullPath);
    void StopMonitoring();
}