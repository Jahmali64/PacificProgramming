using Microsoft.EntityFrameworkCore;
using PacificProgramming.Domain.PptEntities;
using PacificProgramming.Infrastructure.Interfaces;

namespace PacificProgramming.Infrastructure.Contexts;

#nullable disable
public class PptDbContext : DbContext, IPptDbContext {
    public virtual DbSet<Image> Images { get; set; }

    public PptDbContext(DbContextOptions<PptDbContext> options) : base(options) { }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) => base.OnConfiguring(optionsBuilder);
}