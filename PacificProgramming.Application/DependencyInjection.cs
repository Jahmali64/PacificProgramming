using Microsoft.Extensions.DependencyInjection;
using PacificProgramming.Application.Factories;
using PacificProgramming.Application.Services;
using PacificProgramming.Application.Strategies;
using System.Reflection;

namespace PacificProgramming.Application;

public static class DependencyInjection {
    public static IServiceCollection AddApplicationServices(this IServiceCollection services) {
        services.AddScoped<IAvatarImageService, AvatarImageService>();
        services.AddScoped<IImageService, ImageService>();
        services.AddScoped<JsonAvatarImageStrategy>();
        services.AddScoped<DatabaseAvatarImageStrategy>();
        services.AddScoped<VowelAvatarImageStrategy>();
        services.AddScoped<RandomAvatarImageStrategy>();
        services.AddScoped<DefaultAvatarImageStrategy>();
        services.AddScoped<IAvatarImageStrategyFactory, AvatarImageStrategyFactory>();
        services.AddMediatR(configuration => configuration.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()));

        return services;
    }
}