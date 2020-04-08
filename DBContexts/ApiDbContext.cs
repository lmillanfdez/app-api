using Microsoft.EntityFrameworkCore;

public class ApiDbContext : DbContext
{
    public ApiDbContext(DbContextOptions<ApiDbContext> options) : base(options){}
    
    public DbSet<User> Users { get; set; }
    public DbSet<RefreshToken> RefreshTokens { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        builder.Entity<User>().HasKey(item => item.Guid);

        builder.Entity<RefreshToken>().HasKey(item => item.Id);
        builder.Entity<RefreshToken>().HasIndex(item => item.UserId).IsUnique(true);
        builder.Entity<RefreshToken>().HasOne<User>()
                                    .WithOne()
                                    .HasForeignKey<RefreshToken>(rt => rt.UserId)
                                    .OnDelete(DeleteBehavior.Cascade);
    }
}