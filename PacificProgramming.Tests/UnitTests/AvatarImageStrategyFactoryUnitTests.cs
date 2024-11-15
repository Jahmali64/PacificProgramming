using Moq;
using PacificProgramming.Application.Factories;
using PacificProgramming.Application.Services;
using PacificProgramming.Application.Strategies;

namespace PacificProgramming.Tests.UnitTests;
public sealed class AvatarImageStrategyFactoryUnitTests {
    private readonly Mock<IServiceProvider> _serviceProviderMock;
    private readonly Mock<IImageService> _imageServiceMock;
    private readonly IAvatarImageStrategyFactory _avatarImageStrategyFactory;

    public AvatarImageStrategyFactoryUnitTests() {
        _serviceProviderMock = new Mock<IServiceProvider>();
        _imageServiceMock = new Mock<IImageService>();
        _avatarImageStrategyFactory = new AvatarImageStrategyFactory(_serviceProviderMock.Object);
    }

    [Fact]
    public void GetStrategy_WithEmptyIdentifier_ReturnsDefaultAvatarImageStrategy() {
        // Arrange
        _serviceProviderMock.Setup(s => s.GetService(typeof(DefaultAvatarImageStrategy))).Returns(new DefaultAvatarImageStrategy());

        // Act
        var result = _avatarImageStrategyFactory.GetStrategy("");

        // Assert
        Assert.IsType<DefaultAvatarImageStrategy>(result);
    }

    [Fact]
    public void GetStrategy_WithLastDigitBetweenOneAndFiveInIdentifier_ReturnsDatabaseAvatarImageStrategy() {
        // Arrange
        _serviceProviderMock.Setup(s => s.GetService(typeof(DatabaseAvatarImageStrategy))).Returns(new DatabaseAvatarImageStrategy(_imageServiceMock.Object));

        // Act
        var result = _avatarImageStrategyFactory.GetStrategy("myth1");

        // Assert
        Assert.IsType<DatabaseAvatarImageStrategy>(result);
    }

    [Fact]
    public void GetStrategy_WithLastDigitBetweenSixAndNineInIdentifier_ReturnsJsonAvatarImageStrategy() {
        // Arrange
        _serviceProviderMock.Setup(s => s.GetService(typeof(JsonAvatarImageStrategy))).Returns(new JsonAvatarImageStrategy(It.IsAny<CancellationToken>()));

        // Act
        var result = _avatarImageStrategyFactory.GetStrategy("myth7");

        // Assert
        Assert.IsType<JsonAvatarImageStrategy>(result);
    }

    [Fact]
    public void GetStrategy_WithVowelInIdentifier_ReturnsVowelAvatarImageStrategy() {
        // Arrange
        _serviceProviderMock.Setup(s => s.GetService(typeof(VowelAvatarImageStrategy))).Returns(new VowelAvatarImageStrategy());

        // Act
        var result = _avatarImageStrategyFactory.GetStrategy("mythos");

        // Assert
        Assert.IsType<VowelAvatarImageStrategy>(result);
    }

    [Fact]
    public void GetStrategy_WithSpecialCharacterInIdentifier_ReturnsRandomAvatarImageStrategy() {
        // Arrange
        _serviceProviderMock.Setup(s => s.GetService(typeof(RandomAvatarImageStrategy))).Returns(new RandomAvatarImageStrategy());

        // Act
        var result = _avatarImageStrategyFactory.GetStrategy("myth+");

        // Assert
        Assert.IsType<RandomAvatarImageStrategy>(result);
    }

    [Fact]
    public void GetStrategy_WithNoMetConditionsIdentifier_ReturnsDefaultAvatarImageStrategy() {
        // Arrange
        _serviceProviderMock.Setup(s => s.GetService(typeof(DefaultAvatarImageStrategy))).Returns(new DefaultAvatarImageStrategy());

        // Act
        var result = _avatarImageStrategyFactory.GetStrategy("myth");

        // Assert
        Assert.IsType<DefaultAvatarImageStrategy>(result);
    }
}