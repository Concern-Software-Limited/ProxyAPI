using Microsoft.EntityFrameworkCore;
using ProxyAPI.Models;

namespace ProxyAPI.Data;

/// <summary>
/// Entity Framework Core Database Context - Database First Approach
/// Connects to existing database with Users and Products tables
/// No migrations needed - database must exist beforehand
/// </summary>
public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
        // Database First: Don't ensure database is created
        // Database must already exist with proper schema
    }
    
    public DbSet<User> Users { get; set; }
    public DbSet<Product> Products { get; set; }
    public DbSet<Otp> Otps { get; set; }
    public DbSet<UserDetail> UserDetails { get; set; }
    public DbSet<Variant> Variants { get; set; }
    public DbSet<CDKey> CDKeys { get; set; }
    public DbSet<Order> Orders { get; set; }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        
        // User entity configuration - maps to existing [Users] table
        modelBuilder.Entity<User>(entity =>
        {
            entity.ToTable("Users"); // Map to existing table
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => e.Email).IsUnique();
            entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
            entity.Property(e => e.Email).IsRequired().HasMaxLength(100);
            entity.Property(e => e.PasswordHash).IsRequired();
            entity.Property(e => e.Role).IsRequired().HasMaxLength(50);
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("GETUTCDATE()");
        });
        
        // Product entity configuration - maps to existing [Products] table
        modelBuilder.Entity<Product>(entity =>
        {
            entity.ToTable("Products"); // Map to existing table
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).IsRequired().HasMaxLength(200);
            entity.Property(e => e.Description).HasMaxLength(1000);
            entity.Property(e => e.Image).HasMaxLength(500);
            entity.Property(e => e.IsActive).IsRequired().HasDefaultValue(true);
            entity.Property(e => e.Price).HasColumnType("decimal(18,2)");
            entity.Property(e => e.Stock).IsRequired();
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("GETUTCDATE()");
            
            // Navigation properties
            entity.HasMany(e => e.Variants)
                .WithOne(v => v.Product)
                .HasForeignKey(v => v.ProductId)
                .OnDelete(DeleteBehavior.Cascade);
                
            entity.HasMany(e => e.CDKeys)
                .WithOne(k => k.Product)
                .HasForeignKey(k => k.ProductId)
                .OnDelete(DeleteBehavior.Cascade);
        });
        
        // OTP entity configuration - maps to [Otps] table
        modelBuilder.Entity<Otp>(entity =>
        {
            entity.ToTable("Otps");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Email).IsRequired().HasMaxLength(100);
            entity.Property(e => e.OtpCode).IsRequired().HasMaxLength(10);
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("GETUTCDATE()");
            entity.Property(e => e.ExpiresAt).IsRequired();
            entity.Property(e => e.IsUsed).IsRequired().HasDefaultValue(false);
        });
        
        // UserDetail entity configuration - maps to [user_details] table
        modelBuilder.Entity<UserDetail>(entity =>
        {
            entity.ToTable("user_details");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.FirstName).HasColumnName("first_name").HasMaxLength(100);
            entity.Property(e => e.LastName).HasColumnName("last_name").HasMaxLength(100);
            entity.Property(e => e.Email).HasColumnName("email").IsRequired().HasMaxLength(100);
            entity.Property(e => e.CreationDate).HasColumnName("creation_date");
            entity.Property(e => e.Status).HasColumnName("status").HasMaxLength(50);
            entity.Property(e => e.Balance).HasColumnName("balance").HasColumnType("decimal(18,2)");
            entity.Property(e => e.Bot).HasColumnName("bot").HasMaxLength(100);
            entity.Property(e => e.Referral).HasColumnName("referral").HasMaxLength(100);
            entity.Property(e => e.RechBonus).HasColumnName("rech_bonus").HasMaxLength(50);
        });
        
        // Variant entity configuration - maps to [Variants] table
        modelBuilder.Entity<Variant>(entity =>
        {
            entity.ToTable("Variants");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
            entity.Property(e => e.TotalKeys).IsRequired();
            entity.Property(e => e.AvailableKeys).IsRequired();
            entity.Property(e => e.SoldKeys).IsRequired();
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP");
            
            // Foreign key relationship
            entity.HasOne(e => e.Product)
                .WithMany(p => p.Variants)
                .HasForeignKey(e => e.ProductId)
                .OnDelete(DeleteBehavior.Cascade);
        });
        
        // CDKey entity configuration - maps to [CDKeys] table
        modelBuilder.Entity<CDKey>(entity =>
        {
            entity.ToTable("CDKeys");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.KeyValue).IsRequired().HasMaxLength(500);
            entity.Property(e => e.Status).IsRequired().HasMaxLength(50);
            entity.Property(e => e.AddedDate).HasDefaultValueSql("CURRENT_TIMESTAMP");
            
            // Unique constraint on key value
            entity.HasIndex(e => e.KeyValue).IsUnique();
            
            // Foreign key relationships
            entity.HasOne(e => e.Product)
                .WithMany(p => p.CDKeys)
                .HasForeignKey(e => e.ProductId)
                .OnDelete(DeleteBehavior.Cascade);
                
            entity.HasOne(e => e.Variant)
                .WithMany(v => v.CDKeys)
                .HasForeignKey(e => e.VariantId)
                .OnDelete(DeleteBehavior.Restrict);
                
            entity.HasOne(e => e.User)
                .WithMany(u => u.CDKeys)
                .HasForeignKey(e => e.UserId)
                .OnDelete(DeleteBehavior.SetNull);
        });
        
        // Order entity configuration - maps to [Orders] table
        modelBuilder.Entity<Order>(entity =>
        {
            entity.ToTable("Orders");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.KeyValue).IsRequired().HasMaxLength(500);
            entity.Property(e => e.OrderDate).HasDefaultValueSql("CURRENT_TIMESTAMP");
            entity.Property(e => e.Amount).HasColumnType("decimal(18,2)");
            entity.Property(e => e.Status).IsRequired().HasMaxLength(50);
            
            // Foreign key relationships
            entity.HasOne(e => e.User)
                .WithMany(u => u.Orders)
                .HasForeignKey(e => e.UserId)
                .OnDelete(DeleteBehavior.Cascade);
                
            entity.HasOne(e => e.Product)
                .WithMany(p => p.Orders)
                .HasForeignKey(e => e.ProductId)
                .OnDelete(DeleteBehavior.Restrict);
                
            entity.HasOne(e => e.Variant)
                .WithMany(v => v.Orders)
                .HasForeignKey(e => e.VariantId)
                .OnDelete(DeleteBehavior.Restrict);
                
            entity.HasOne(e => e.CDKey)
                .WithMany(k => k.Orders)
                .HasForeignKey(e => e.CDKeyId)
                .OnDelete(DeleteBehavior.Restrict);
        });
    }
}
