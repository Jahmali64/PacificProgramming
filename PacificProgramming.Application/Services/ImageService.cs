using Microsoft.EntityFrameworkCore;
using PacificProgramming.Application.ViewModels;
using PacificProgramming.Infrastructure.Interfaces;

namespace PacificProgramming.Application.Services;

public interface IImageService {
    Task<ImageVM?> GetImageById(int id);
}

public sealed class ImageService : IImageService {
    private readonly IPptDbContext _dbContext;
    private readonly CancellationToken _cancellationToken;

    public ImageService(IPptDbContext dbContext, CancellationToken cancellationToken) {
        _dbContext = dbContext;
        _cancellationToken = cancellationToken;
    }

    public async Task<ImageVM?> GetImageById(int id) {
        return await _dbContext.Images
            .Where(x => x.Id == id)
            .Select(x => new ImageVM { Id = x.Id, Url = x.Url, })
            .FirstOrDefaultAsync(_cancellationToken);
    }
}