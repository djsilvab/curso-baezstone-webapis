using BaezStone.MagicVilla.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace BaezStone.MagicVilla.Api.Store;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
        
    }

    public DbSet<Villa> Villas { get; set; }
    public DbSet<NumeroVilla> NumeroVillas { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Villa>().HasData(
            new Villa
            {
                Id = 1,
                Nombre = "Villa Real",
                Detalle = "Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua.",
                ImagenURL = "https://dotnetmasteryimages.blob.core.windows.net/bluevillaimages/villa3.jpg",
                Ocupantes = 4,
                Tarifa = 200.0,
                MetrosCuadrados = 550,
                Amenidad = "",
                FechaCreacion = new DateTime(2026, 4, 10),
                FechaActualizacion = new DateTime(2026, 4, 10)
            },
            new Villa
            {
                Id = 2,
                Nombre = "Premium Pool Villa",
                Detalle = "Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua.",
                ImagenURL = "https://dotnetmasteryimages.blob.core.windows.net/bluevillaimages/villa1.jpg",
                Ocupantes = 4,
                Tarifa = 300.0,
                MetrosCuadrados = 550,
                Amenidad = "",
                FechaCreacion = new DateTime(2026, 4, 10),
                FechaActualizacion = new DateTime(2026, 4, 10)
            }
        );

        modelBuilder.Entity<NumeroVilla>(entity =>
        {
            entity.Property(x => x.FechaCreacion)
                .HasDefaultValueSql("GETUTCDATE()");           
        });

    }
}
