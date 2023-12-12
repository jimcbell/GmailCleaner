﻿// <auto-generated />
using System;
using GmailCleaner.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace GmailCleaner.Common.DataContext.SqlServer.Migrations
{
    [DbContext(typeof(GmailCleanerContext))]
    [Migration("20231211045516_InitialCloudMigration")]
    partial class InitialCloudMigration
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "7.0.0")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("GmailCleaner.Common.Models.GCMessage", b =>
                {
                    b.Property<int>("MessageId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("MessageId"));

                    b.Property<string>("From")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("MessageGmailId")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Snippet")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("UnsubscribeLink")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("UserId")
                        .HasColumnType("int");

                    b.HasKey("MessageId");

                    b.HasIndex("UserId");

                    b.ToTable("GCMessage");
                });

            modelBuilder.Entity("GmailCleaner.Common.Models.GCUser", b =>
                {
                    b.Property<int>("UserId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("UserId"));

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("GmailId")
                        .IsRequired()
                        .HasColumnType("nvarchar(450)");

                    b.Property<bool>("IsAdmin")
                        .HasColumnType("bit");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("Usages")
                        .HasColumnType("int");

                    b.HasKey("UserId");

                    b.HasIndex("GmailId")
                        .IsUnique();

                    b.ToTable("GCUser");
                });

            modelBuilder.Entity("GmailCleaner.Common.Models.GCUserToken", b =>
                {
                    b.Property<int>("UserTokenId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("UserTokenId"));

                    b.Property<string>("AccessToken")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime?>("ExpiresOn")
                        .HasColumnType("datetime");

                    b.Property<string>("IdToken")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("RefreshToken")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("UserId")
                        .HasColumnType("int");

                    b.HasKey("UserTokenId");

                    b.HasIndex("UserId");

                    b.ToTable("GCUserToken");
                });

            modelBuilder.Entity("GmailCleaner.Common.Models.GCMessage", b =>
                {
                    b.HasOne("GmailCleaner.Common.Models.GCUser", "User")
                        .WithMany("GCMessages")
                        .HasForeignKey("UserId")
                        .IsRequired()
                        .HasConstraintName("FK_GCMessage_GCUser");

                    b.Navigation("User");
                });

            modelBuilder.Entity("GmailCleaner.Common.Models.GCUserToken", b =>
                {
                    b.HasOne("GmailCleaner.Common.Models.GCUser", "User")
                        .WithMany("GCUserTokens")
                        .HasForeignKey("UserId")
                        .IsRequired()
                        .HasConstraintName("FK_GCUserToken_GCUser");

                    b.Navigation("User");
                });

            modelBuilder.Entity("GmailCleaner.Common.Models.GCUser", b =>
                {
                    b.Navigation("GCMessages");

                    b.Navigation("GCUserTokens");
                });
#pragma warning restore 612, 618
        }
    }
}
