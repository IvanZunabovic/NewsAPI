using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Data;

public sealed class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        :base(options)
    {
    }

    public DbSet<Author> Authors { get; set; }
    public DbSet<News> News { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Author>(entity => 
        {
            entity.ComplexProperty(a => a.FirstName, p =>
            {
                p.Property(p => p.Value)
                    .HasMaxLength(50)
                    .HasColumnName("FirstName");;
            });

            entity.ComplexProperty(a => a.LastName, p =>
            {
                p.Property(p => p.Value)
                    .HasMaxLength(50)
                    .HasColumnName("LastName"); ;
            });

            entity.ComplexProperty(a => a.Email, p =>
            {
                p.Property(p => p.Value)
                    .HasMaxLength(100)
                    .HasColumnName("Email");
            });

            entity.Property(a => a.UserName)
                .HasMaxLength(100);

            entity.HasMany(a => a.News)
                .WithOne(n => n.Author);
        });

        modelBuilder.Entity<News>(entity => 
        {
            entity.ComplexProperty(n => n.Content, p =>
            {
                p.Property(p => p.Value)
                    .HasMaxLength(10000)
                    .HasColumnName("Content");
            });
        });

        base.OnModelCreating(modelBuilder);
    }
}
