using Microsoft.EntityFrameworkCore;
using PacificProgramming.Domain.PptEntities;

namespace PacificProgramming.Infrastructure.Interfaces;

public interface IPptDbContext : IAsyncDisposable {
    Task<int> SaveChangesAsync(CancellationToken cancellationToken);

    DbSet<Image> Images { get; set; }
}