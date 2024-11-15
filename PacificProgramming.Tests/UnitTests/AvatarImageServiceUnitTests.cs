using Microsoft.EntityFrameworkCore;
using MockQueryable.Moq;
using Moq;
using PacificProgramming.Application.Factories;
using PacificProgramming.Application.Services;
using PacificProgramming.Application.Strategies;
using PacificProgramming.Application.ViewModels;
using PacificProgramming.Domain.PptEntities;
using PacificProgramming.Infrastructure.Interfaces;

namespace PacificProgramming.Tests.UnitTests;

public sealed class AvatarImageServiceUnitTests {
    private readonly Mock<IImageService> _imageServiceMock;
    private readonly Mock<IAvatarImageStrategyFactory> _avatarImageStrategyFactoryMock;
    private readonly Mock<IPptDbContext> _pptDbContextMock;
    private readonly AvatarImageService _avatarImageService;

    public AvatarImageServiceUnitTests() {
        _imageServiceMock = new Mock<IImageService>();
        _avatarImageStrategyFactoryMock = new Mock<IAvatarImageStrategyFactory>();
        _pptDbContextMock = new Mock<IPptDbContext>();

        _avatarImageService = new AvatarImageService(_avatarImageStrategyFactoryMock.Object);
    }

    [Theory]
    [InlineData("", 0, "https://api.dicebear.com/8.x/pixel-art/png?seed=default&size=150")]
    public async Task GetAvatarImage_WithEmptyIdentifier_ReturnsImageWithDefaultUrl(string userIdentifier, int expectedId, string expectedUrl) {
        // Arrange
        _avatarImageStrategyFactoryMock.Setup(s => s.GetStrategy(userIdentifier)).Returns(new DefaultAvatarImageStrategy());

        // Act
        var result = await _avatarImageService.GetAvatarImage(userIdentifier);

        // Assert
        Assert.Equal(result.Id, expectedId);
        Assert.Equal(result.Url, expectedUrl);
    }

    [Theory]
    [InlineData("myth1", 1, "https://api.dicebear.com/8.x/pixel-art/png?seed=db1&size=150")]
    public async Task GetAvatarImage_WithLastDigitBetweenOneAndFiveInIdentifier_ReturnsImageWithUrlFromDatabase(string userIdentifier, int expectedId, string expectedUrl) {
        // Arrange
        List<Image> images = new List<Image> { new Image { Id = expectedId, Url = "https://api.dicebear.com/8.x/pixel-art/png?seed=db1&size=150" } };
        Mock<DbSet<Image>> dbSetImageMock = images.AsQueryable().BuildMockDbSet();
        _avatarImageStrategyFactoryMock.Setup(s => s.GetStrategy(userIdentifier)).Returns(new DatabaseAvatarImageStrategy(_imageServiceMock.Object));
        _imageServiceMock.Setup(s => s.GetImageById(expectedId)).ReturnsAsync(images.Select(x => new ImageVM { Id = x.Id, Url = x.Url }).FirstOrDefault());
        _pptDbContextMock.Setup(s => s.Images).Returns(dbSetImageMock.Object);

        // Act
        var result = await _avatarImageService.GetAvatarImage(userIdentifier);

        // Assert
        Assert.Equal(result.Id, expectedId);
        Assert.Equal(result.Url, expectedUrl);
    }

    [Theory]
    [InlineData("myth7", 7, "https://api.dicebear.com/8.x/pixel-art/png?seed=7&size=150")]
    public async Task GetAvatarImage_WithLastDigitBetweenSixAndNineInIdentifier_ReturnsImageWithUrlFromJson(string userIdentifier, int expectedId, string expectedUrl) {
        // Arrange
        _avatarImageStrategyFactoryMock.Setup(s => s.GetStrategy(userIdentifier)).Returns(new JsonAvatarImageStrategy(It.IsAny<CancellationToken>()));

        // Act
        var result = await _avatarImageService.GetAvatarImage(userIdentifier);

        // Assert
        Assert.Equal(result.Id, expectedId);
        Assert.Equal(result.Url, expectedUrl);
    }

    [Theory]
    [InlineData("mythos", 0, "https://api.dicebear.com/8.x/pixel-art/png?seed=vowel&size=150")]
    public async Task GetAvatarImage_WithVowelInIdentifier_ReturnsImageWithVowelUrl(string userIdentifier, int expectedId, string expectedUrl) {
        // Arrange
        _avatarImageStrategyFactoryMock.Setup(s => s.GetStrategy(userIdentifier)).Returns(new VowelAvatarImageStrategy());

        // Act
        var result = await _avatarImageService.GetAvatarImage(userIdentifier);

        // Assert
        Assert.Equal(result.Id, expectedId);
        Assert.Equal(result.Url, expectedUrl);
    }

    [Theory]
    [InlineData("myth+", 1, 5, "https://api.dicebear.com/8.x/pixel-art")]
    public async Task GetAvatarImage_WithSpecialCharacterInIdentifier_ReturnsImageWithRandomUrl(string userIdentifier, int expectedMinId, int expectedMaxId, string expectedUrl) {
        // Arrange
        _avatarImageStrategyFactoryMock.Setup(s => s.GetStrategy(userIdentifier)).Returns(new RandomAvatarImageStrategy());

        // Act
        var result = await _avatarImageService.GetAvatarImage(userIdentifier);

        // Assert
        Assert.InRange<int>(result.Id, expectedMinId, expectedMaxId);
        Assert.StartsWith(expectedUrl, result.Url);
    }

    [Theory]
    [InlineData("myth", 0, "https://api.dicebear.com/8.x/pixel-art/png?seed=default&size=150")]
    public async Task GetAvatarImage_WithNoMetConditionsIdentifier_ReturnsImageWithDefaultUrl(string userIdentifier, int expectedId, string expectedUrl) {
        // Arrange
        _avatarImageStrategyFactoryMock.Setup(s => s.GetStrategy(userIdentifier)).Returns(new DefaultAvatarImageStrategy());

        // Act
        var result = await _avatarImageService.GetAvatarImage(userIdentifier);

        // Assert
        Assert.Equal(result.Id, expectedId);
        Assert.Equal(result.Url, expectedUrl);
    }
}