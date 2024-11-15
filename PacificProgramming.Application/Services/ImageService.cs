using Microsoft.EntityFrameworkCore;
using PacificProgramming.Application.ViewModels;
using PacificProgramming.Infrastructure.Interfaces;

namespace PacificProgramming.Application.Services;

public interface IImageService {
    Task<ImageVM?> GetImageById(int id);
}

public sealed class ImageService : IImageService {
    private readonly IPptDbContext _dbContext;

    public ImageService(IPptDbContext dbContext) {
        _dbContext = dbContext;
    }

    public async Task<ImageVM?> GetImageById(int id) {
        var image = await _dbContext.Images.Where(x => x.Id == id).FirstOrDefaultAsync();

        if (image == null) return null;
        return new ImageVM { Id = image.Id, Url = image.Url };
    }
}