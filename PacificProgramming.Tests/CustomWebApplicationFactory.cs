using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using PacificProgramming.Domain.PptEntities;
using PacificProgramming.Infrastructure.Contexts;

namespace PacificProgramming.Tests;

public class CustomWebApplicationFactory : WebApplicationFactory<PacificProgramming.API.Program> {
    protected override void ConfigureWebHost(IWebHostBuilder builder) {
        builder.ConfigureServices(services => {
            services.Replace(ServiceDescriptor.Scoped(provider =>
                        new DbContextOptionsBuilder<PptDbContext>()
                            .UseInMemoryDatabase("TestDatabase")
                            .Options));

            var serviceProvider = services.BuildServiceProvider();
            using var scope = serviceProvider.CreateScope();
            var scopedServices = scope.ServiceProvider;
            var dbContext = scopedServices.GetRequiredService<PptDbContext>();

            SeedDatabase(dbContext);
        });
    }

    private void SeedDatabase(PptDbContext context) {
        if (!context.Database.IsInMemory()) throw new AccessViolationException("Only use in memory database during tests.");
        context.Database.EnsureDeleted();
        context.Database.EnsureCreated();

        context.Images.Add(new Image { Url = "https://api.dicebear.com/8.x/pixel-art/png?seed=db1&size=150" });
        context.SaveChanges();
    }
}