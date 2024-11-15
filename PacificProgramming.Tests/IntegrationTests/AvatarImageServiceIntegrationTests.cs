using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using PacificProgramming.Application.Factories;
using PacificProgramming.Application.Services;
using PacificProgramming.Application.Strategies;
using PacificProgramming.Domain.PptEntities;
using PacificProgramming.Infrastructure.Contexts;
using PacificProgramming.Infrastructure.Interfaces;

namespace PacificProgramming.Tests.IntegrationTests;

public sealed class AvatarImageServiceIntegrationTests {
    private readonly IServiceProvider _serviceProvider;
    private readonly IAvatarImageService _avatarImageService;

    public AvatarImageServiceIntegrationTests() {
        var serviceCollection = GetRequiredServicesForIntegrationTests();

        _serviceProvider = serviceCollection.BuildServiceProvider();
        _avatarImageService = _serviceProvider.GetRequiredService<IAvatarImageService>();
        SeedDatabase();
    }

    [Theory]
    [InlineData("", "https://api.dicebear.com/8.x/pixel-art/png?seed=default&size=150")]
    public async Task GetAvatarImage_WithEmptyIdentifier_ReturnsImageWithDefaultUrl(string userIdentifier, string expectedUrl) {
        var result = await _avatarImageService.GetAvatarImage(userIdentifier);

        Assert.Equal(result.Id, 0);
        Assert.Equal(result.Url, expectedUrl);
    }

    [Theory]
    [InlineData("myth1", "https://api.dicebear.com/8.x/pixel-art/png?seed=db1&size=150")]
    public async Task GetAvatarImage_WithLastDigitBetweenOneAndFiveInIdentifier_ReturnsImageWithUrlFromDatabase(string userIdentifier, string expectedUrl) {
        var result = await _avatarImageService.GetAvatarImage(userIdentifier);

        Assert.Equal(result.Id, 1);
        Assert.Equal(result.Url, expectedUrl);
    }

    [Theory]
    [InlineData("myth7", "https://api.dicebear.com/8.x/pixel-art/png?seed=7&size=150")]
    public async Task GetAvatarImage_WithLastDigitBetweenSixAndNineInIdentifier_ReturnsImageWithUrlFromJson(string userIdentifier, string expectedUrl) {
        var result = await _avatarImageService.GetAvatarImage(userIdentifier);

        Assert.Equal(result.Id, 7);
        Assert.Equal(result.Url, expectedUrl);
    }

    [Theory]
    [InlineData("mythos", "https://api.dicebear.com/8.x/pixel-art/png?seed=vowel&size=150")]
    public async Task GetAvatarImage_WithVowelInIdentifier_ReturnsImageWithVowelUrl(string userIdentifier, string expectedUrl) {
        var result = await _avatarImageService.GetAvatarImage(userIdentifier);

        Assert.Equal(result.Id, 0);
        Assert.Equal(result.Url, expectedUrl);
    }

    [Theory]
    [InlineData("myth+", "https://api.dicebear.com/8.x/pixel-art")]
    public async Task GetAvatarImage_WithSpecialCharacterInIdentifier_ReturnsImageWithRandomUrl(string userIdentifier, string expectedUrl) {
        var result = await _avatarImageService.GetAvatarImage(userIdentifier);

        Assert.InRange<int>(result.Id, 1, 5);
        Assert.StartsWith(expectedUrl, result.Url);
    }

    [Theory]
    [InlineData("myth", "https://api.dicebear.com/8.x/pixel-art/png?seed=default&size=150")]
    public async Task GetAvatarImage_WithNoMetConditionsIdentifier_ReturnsImageWithDefaultUrl(string userIdentifier, string expectedUrl) {
        var result = await _avatarImageService.GetAvatarImage(userIdentifier);

        Assert.Equal(result.Id, 0);
        Assert.Equal(result.Url, expectedUrl);
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

    private void SeedDatabase() {
        var context = _serviceProvider.GetRequiredService<PptDbContext>();

        if (!context.Database.IsInMemory()) throw new AccessViolationException("Only use in memory database during tests.");
        context.Database.EnsureDeleted();
        context.Database.EnsureCreated();

        context.Images.Add(new Image { Url = "https://api.dicebear.com/8.x/pixel-art/png?seed=db1&size=150" });
        context.SaveChanges();
    }
}