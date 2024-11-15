using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using PacificProgramming.Application.Factories;
using PacificProgramming.Application.Services;
using PacificProgramming.Application.Strategies;
using PacificProgramming.Infrastructure.Contexts;
using PacificProgramming.Infrastructure.Interfaces;

namespace PacificProgramming.Tests.IntegrationTests;
public sealed class AvatarImageStrategyFactoryIntegrationTests {
    private readonly IServiceProvider _serviceProvider;
    private readonly IAvatarImageStrategyFactory _avatarImageStrategyFactory;

    public AvatarImageStrategyFactoryIntegrationTests() {
        var serviceCollection = GetRequiredServicesForIntegrationTests();

        _serviceProvider = serviceCollection.BuildServiceProvider();
        _avatarImageStrategyFactory = _serviceProvider.GetRequiredService<IAvatarImageStrategyFactory>();
    }

    [Fact]
    public void GetStrategy_WithEmptyIdentifier_ReturnsDefaultAvatarImageStrategy() {
        // Act
        var result = _avatarImageStrategyFactory.GetStrategy("");

        // Assert
        Assert.IsType<DefaultAvatarImageStrategy>(result);
    }

    [Fact]
    public void GetStrategy_WithLastDigitBetweenOneAndFiveInIdentifier_ReturnsDatabaseAvatarImageStrategy() {
        // Act
        var result = _avatarImageStrategyFactory.GetStrategy("myth1");

        // Assert
        Assert.IsType<DatabaseAvatarImageStrategy>(result);
    }

    [Fact]
    public void GetStrategy_WithLastDigitBetweenSixAndNineInIdentifier_ReturnsJsonAvatarImageStrategy() {
        // Act
        var result = _avatarImageStrategyFactory.GetStrategy("myth7");

        // Assert
        Assert.IsType<JsonAvatarImageStrategy>(result);
    }

    [Fact]
    public void GetStrategy_WithVowelInIdentifier_ReturnsVowelAvatarImageStrategy() {
        // Act
        var result = _avatarImageStrategyFactory.GetStrategy("mythos");

        // Assert
        Assert.IsType<VowelAvatarImageStrategy>(result);
    }

    [Fact]
    public void GetStrategy_WithSpecialCharacterInIdentifier_ReturnsRandomAvatarImageStrategy() {
        // Act
        var result = _avatarImageStrategyFactory.GetStrategy("myth+");

        // Assert
        Assert.IsType<RandomAvatarImageStrategy>(result);
    }

    [Fact]
    public void GetStrategy_WithNoMetConditionsIdentifier_ReturnsDefaultAvatarImageStrategy() {
        // Act
        var result = _avatarImageStrategyFactory.GetStrategy("myth");

        // Assert
        Assert.IsType<DefaultAvatarImageStrategy>(result);
    }

    private ServiceCollection GetRequiredServicesForIntegrationTests() {
        var serviceCollection = new ServiceCollection();

        serviceCollection.AddDbContext<IPptDbContext, PptDbContext>(options =>
            options.UseInMemoryDatabase("PptIntegrationTestDatabase"));

        serviceCollection.AddScoped<IImageService, ImageService>();
        serviceCollection.AddScoped<IAvatarImageService, AvatarImageService>();
        serviceCollection.AddScoped<DefaultAvatarImageStrategy>();
        serviceCollection.AddScoped<JsonAvatarImageStrategy>();
        serviceCollection.AddScoped<DatabaseAvatarImageStrategy>();
        serviceCollection.AddScoped<VowelAvatarImageStrategy>();
        serviceCollection.AddScoped<RandomAvatarImageStrategy>();
        serviceCollection.AddScoped<IAvatarImageStrategyFactory, AvatarImageStrategyFactory>();
        serviceCollection.AddHttpContextAccessor();
        serviceCollection.AddScoped(typeof(CancellationToken), serviceProvider => {
            IHttpContextAccessor httpContext = serviceProvider.GetRequiredService<IHttpContextAccessor>();
            return httpContext.HttpContext?.RequestAborted ?? CancellationToken.None;
        });

        return serviceCollection;
    }
}