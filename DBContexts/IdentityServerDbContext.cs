using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

public class IdentityServerDbContext : IdentityDbContext
{
    public IdentityServerDbContext(DbContextOptions<IdentityServerDbContext> options)
            : base(options){}

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
    }
}