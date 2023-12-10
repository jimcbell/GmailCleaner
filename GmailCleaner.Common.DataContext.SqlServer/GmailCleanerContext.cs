using System;
using System.Collections.Generic;
using GmailCleaner.Common.Models;
using Microsoft.EntityFrameworkCore;

namespace GmailCleaner.Common;

public partial class GmailCleanerContext : DbContext
{
    public GmailCleanerContext()
    {
    }

    public GmailCleanerContext(DbContextOptions<GmailCleanerContext> options)
        : base(options)
    {
    }

    public virtual DbSet<GCUser> GCUsers { get; set; }

    public virtual DbSet<GCUserToken> GCUserTokens { get; set; }
    public virtual DbSet<GCMessage> GCMessages { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Data Source=.;Initial Catalog=GmailCleaner;Integrated Security=true;TrustServerCertificate=true;");

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

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
