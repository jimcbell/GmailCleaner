using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace GmailCleaner.EFCore.Models;

public partial class GmailCleanerDb : DbContext
{
    public GmailCleanerDb()
    {
    }

    public GmailCleanerDb(DbContextOptions<GmailCleanerDb> options)
        : base(options)
    {
    }

    public virtual DbSet<User> Users { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        => optionsBuilder.UseSqlServer("Data Source=.;Initial Catalog=GmailCleaner;Integrated Security=true;TrustServerCertificate=true;");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Users__3214EC07AAD4CFE9");

            entity.Property(e => e.Id).HasDefaultValueSql("(newid())");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
