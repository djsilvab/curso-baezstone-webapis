using BaezStone.MagicVilla.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace BaezStone.MagicVilla.Api.Store;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
        
    }

    public DbSet<Villa> Villas{ get; set; }
}
