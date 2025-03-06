using System.IO.Abstractions.TestingHelpers;
using EdServer.Services;
using Microsoft.Extensions.Configuration;
using XFS = System.IO.Abstractions.TestingHelpers.MockUnixSupport;

namespace EdServer.Tests.Services;

public class FileLocationServiceTests
{
    private readonly IConfigurationRoot _configuration;
    
    private readonly FileLocationService _fileLocationService;

    public FileLocationServiceTests()
    {
        var filepath = XFS.Path(@"C:\journal\location\Journal.test.log");
        var fileSystemMock = new MockFileSystem(new Dictionary<string, MockFileData>
        {
            { filepath, new MockFileData("I'm here") }
        });
        var myConfiguration = new Dictionary<string, string?>
        {
            {"JournalLocation", @"C:\journal\location\"},
        };
        _configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(myConfiguration)
            .Build();
        
        _fileLocationService = new FileLocationService(_configuration, fileSystemMock);
    }

    [Fact]
    public void ShouldReturnConfiguredJournalFileLocationIfPresent()
    {
        Assert.Equal(@"C:\journal\location", _fileLocationService.GetJournalLocation());
    }

    [Fact]
    public void ShouldReturnDefaultJournalFileLocationIfNotConfigured()
    {
        _configuration["JournalLocation"] = null;
        Assert.Equal(_fileLocationService.GetDefaultJournalLocation(), _fileLocationService.GetJournalLocation());
    }

    [Fact]
    public void DefaultLocation_ShouldEndWithCorrectFolderStructure()
    {
        Assert.EndsWith(@"\Saved Games\Frontier Developments\Elite Dangerous", _fileLocationService.GetDefaultJournalLocation());
    }
}