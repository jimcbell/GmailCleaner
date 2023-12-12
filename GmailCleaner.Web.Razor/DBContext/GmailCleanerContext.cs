using EntityFrameworkCore.EncryptColumn.Extension;
using EntityFrameworkCore.EncryptColumn.Interfaces;
using EntityFrameworkCore.EncryptColumn.Util;
using GmailCleaner.Entities;
using Microsoft.EntityFrameworkCore;

namespace GmailCleaner.Data;

public partial class GmailCleanerContext : DbContext
{
    private readonly IEncryptionProvider _provider;

    public GmailCleanerContext(IConfiguration configuration)
    {
        string? key = configuration["gmail-cleaner-key"];
        this._provider = new GenerateEncryptionProvider(key);

    }

    public GmailCleanerContext(DbContextOptions<GmailCleanerContext> options, string key)
        : base(options)
    {
        this._provider = new GenerateEncryptionProvider(key);
    }

    public virtual DbSet<GCUser> GCUsers { get; set; }

    public virtual DbSet<GCUserToken> GCUserTokens { get; set; }
    public virtual DbSet<GCMessage> GCMessages { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        // Should be configured from app that runs the libray, else defaulting to local connection string.
        if (!optionsBuilder.IsConfigured)
        {
            optionsBuilder.UseSqlServer("Data Source=.;Initial Catalog=GmailCleaner;Integrated Security=true;TrustServerCertificate=true;");
        }
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<GCUser>(entity =>
        {
            entity.Property(e => e.UserId).ValueGeneratedOnAdd();
            entity.HasIndex(e => e.GmailId).IsUnique();
        });

        modelBuilder.Entity<GCUserToken>(entity =>
        {
            entity.Property(e => e.UserTokenId).ValueGeneratedOnAdd();

            entity.HasOne(d => d.User).WithMany(p => p.GCUserTokens)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_GCUserToken_GCUser");

        });
        modelBuilder.Entity<GCMessage>(entity =>
        {
            entity.Property(e => e.MessageId).ValueGeneratedOnAdd();
            entity.HasOne(d => d.User).WithMany(p => p.GCMessages)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_GCMessage_GCUser");
        });
        modelBuilder.UseEncryption(_provider);
        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
