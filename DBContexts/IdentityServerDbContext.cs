using System;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

public class IdentityServerDbContext : IdentityDbContext<User>
{
    public IdentityServerDbContext(DbContextOptions<IdentityServerDbContext> options)
            : base(options){}

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<IdentityUserRole<Guid>>().HasKey(e => new { e.UserId, e.RoleId });
        base.OnModelCreating(modelBuilder);
    }
}