using System.IO.Abstractions;
using EdServer.Interfaces;

namespace EdServer.Services;

public class FileLocationService : IFileLocationService
{
    private readonly IConfiguration _configuration;
    private readonly IFileSystem _fileSystem;

    public FileLocationService(IConfiguration configuration, IFileSystem fileSystem)
    {
        _configuration = configuration;
        _fileSystem = fileSystem;
    }
    
    public string GetJournalLocation()
    {
        var journalLocation = _configuration["JournalLocation"] ?? GetDefaultJournalLocation();
        var directory = _fileSystem.DirectoryInfo.New(journalLocation);
        return directory.FullName;
    }

    public string GetDefaultJournalLocation()
    {
        return Environment.GetFolderPath(Environment.SpecialFolder.UserProfile) + 
               @"\Saved Games\Frontier Developments\Elite Dangerous";
    }
}