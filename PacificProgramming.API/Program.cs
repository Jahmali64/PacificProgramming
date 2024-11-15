using PacificProgramming.Infrastructure;
using PacificProgramming.Application;
using PacificProgramming.API.Models;
using Microsoft.Extensions.Options;

namespace PacificProgramming.API;

public class Program {
    public static void Main(string[] args) {
        var builder = WebApplication.CreateBuilder(args);

        // Add services to the container.
        builder.Services.AddApplicationServices();
        builder.Services.AddInfrastructure(builder.Configuration);
        builder.Services.Configure<AppSettings>(builder.Configuration.GetSection("AllowedOrigins"));
        builder.Services.AddHttpContextAccessor();
        builder.Services.AddScoped(typeof(CancellationToken), serviceProvider => {
            IHttpContextAccessor httpContext = serviceProvider.GetRequiredService<IHttpContextAccessor>();
            return httpContext.HttpContext?.RequestAborted ?? CancellationToken.None;
        });

        builder.Services.AddControllers();
        // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();

        var app = builder.Build();

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment()) {
            app.UseSwagger();
            app.UseSwaggerUI();
            var frontendTestUrl = app.Services.GetRequiredService<IOptions<AppSettings>>().Value.FrontendTestUrl;
            app.UseCors(builder => builder.WithOrigins(frontendTestUrl));
        }

        app.UseHttpsRedirection();

        app.UseAuthorization();


        app.MapControllers();

        app.Run();
    }
}