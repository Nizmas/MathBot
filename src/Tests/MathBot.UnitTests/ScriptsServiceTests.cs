using System.ComponentModel.DataAnnotations;
using MathBot.Bll.Implementations;
using MathBot.Bll.Interfaces;
using MathBot.Dal.Interfaces;
using Moq;
using NUnit.Framework;
using NUnit.Framework.Legacy;

namespace MathBot.UnitTests;

[TestFixture]
public class ScriptsServiceTests
{
    private const string Result = "RESULT";
    private const string CorrectValue = $"return \"{Result}\";";
    private const string Key = "CommandKey";
    private readonly IScriptsService _scriptsService;
    private readonly Mock<IScriptsProvider> _scriptsProviderMock = new ();
    
    public ScriptsServiceTests()
    {
        _scriptsService = new ScriptsService(_scriptsProviderMock.Object);
    }
    
    [Test]
    public async Task ExecuteAsync_CorrectScript_ResultResponse()
    {
        // Arrange
        _scriptsProviderMock.Setup(sP => sP.GetValueAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                            .ReturnsAsync((string _ ,CancellationToken _) => CorrectValue);
        
        // Act
        var result = await _scriptsService.ExecuteAsync(Key, CancellationToken.None);
        
        // Assert
        ClassicAssert.AreEqual(Result, result);
    }
    
    [Test]
    public void GetAllAsync_NoScript_Exception()
    {
        // Arrange
        _scriptsProviderMock.Setup(sP => sP.GetValueAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                            .ReturnsAsync((string _, CancellationToken _) => CorrectValue);
        
        // Act, Assert
        Assert.ThrowsAsync<KeyNotFoundException>(async () => await _scriptsService.ExecuteAsync("/", CancellationToken.None));
    }
    
    [Test]
    public void AddAsync_CommandWoScript_Exception()
    {
        // Arrange, Act, Assert
        Assert.ThrowsAsync<ValidationException>(async () => await _scriptsService.AddAsync($"{Key}", CancellationToken.None));
    }
    
    [Test]
    public void AddAsync_CommandEmptyScript_Exception()
    {
        // Arrange, Act, Assert
        Assert.ThrowsAsync<ValidationException>(async () => await _scriptsService.AddAsync($"{Key}\n", CancellationToken.None));
    }
    
    [Test]
    public void AddAsync_CommandIncorrectScript_Exception()
    {
        // Arrange, Act, Assert
        Assert.ThrowsAsync<ValidationException>(async () => await _scriptsService.AddAsync($"{Key}\n{Result}", CancellationToken.None));
    }
    
    [Test]
    public async Task AddAsync_CommandCorrectScript_Exception()
    {
        // Arrange
        _scriptsProviderMock.Setup(sP => sP.SetValueAsync(It.IsAny<KeyValuePair<string, string>>(), It.IsAny<CancellationToken>()))
                            .Returns(Task.CompletedTask);
        
        // Act
        var addResult = await _scriptsService.AddAsync($"{Key}\n{CorrectValue}", CancellationToken.None);
        var commandKey = Key.ToLower();
        
        // Assert
        ClassicAssert.True(addResult.Contains(commandKey));
    }
}