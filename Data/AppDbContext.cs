using System;
using System.Collections.Generic;
using BookCatalog.Models;
using Microsoft.EntityFrameworkCore;

namespace BookCatalog.Data;

public partial class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Author> Authors { get; set; }

    public virtual DbSet<Book> Books { get; set; }

    public virtual DbSet<Genre> Genres { get; set; }

    public virtual DbSet<Note> Notes { get; set; }

    public virtual DbSet<ReadingLog> ReadingLogs { get; set; }

    public virtual DbSet<User> Users { get; set; }

    public virtual DbSet<UserAchievement> UserAchievements { get; set; }

    public virtual DbSet<UserBook> UserBooks { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Author>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Authors__3214EC07B9AC4F5A");

            entity.Property(e => e.BirthPlace).HasMaxLength(200);
            entity.Property(e => e.Name).HasMaxLength(200);
        });

        modelBuilder.Entity<Book>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Books__3214EC0771DEE014");

            entity.HasIndex(e => e.Title, "IX_Books_Title");

            entity.Property(e => e.CoverPath).HasMaxLength(500);
            entity.Property(e => e.Isbn).HasMaxLength(20);
            entity.Property(e => e.Title).HasMaxLength(300);

            entity.HasOne(d => d.Author).WithMany(p => p.Books)
                .HasForeignKey(d => d.AuthorId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Books_Authors");

            entity.HasMany(d => d.Genres).WithMany(p => p.Books)
                .UsingEntity<Dictionary<string, object>>(
                    "BookGenre",
                    r => r.HasOne<Genre>().WithMany()
                        .HasForeignKey("GenreId")
                        .HasConstraintName("FK_BookGenres_Genres"),
                    l => l.HasOne<Book>().WithMany()
                        .HasForeignKey("BookId")
                        .HasConstraintName("FK_BookGenres_Books"),
                    j =>
                    {
                        j.HasKey("BookId", "GenreId");
                        j.ToTable("BookGenres");
                    });
        });

        modelBuilder.Entity<Genre>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Genres__3214EC075D2FA5EB");

            entity.HasIndex(e => e.Name, "UQ__Genres__737584F6BAA84918").IsUnique();

            entity.Property(e => e.Name).HasMaxLength(100);
        });

        modelBuilder.Entity<Note>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Notes__3214EC07BF82F7FE");

            entity.Property(e => e.UpdatedAt).HasDefaultValueSql("(sysutcdatetime())");

            entity.HasOne(d => d.UserBook).WithMany(p => p.Notes)
                .HasForeignKey(d => d.UserBookId)
                .HasConstraintName("FK_Notes_UserBooks");
        });

        modelBuilder.Entity<ReadingLog>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__ReadingL__3214EC07AE3B01A3");

            entity.HasIndex(e => e.LogDate, "IX_ReadingLogs_Date");

            entity.Property(e => e.LogDate).HasDefaultValueSql("(CONVERT([date],sysutcdatetime()))");

            entity.HasOne(d => d.UserBook).WithMany(p => p.ReadingLogs)
                .HasForeignKey(d => d.UserBookId)
                .HasConstraintName("FK_ReadingLogs_UserBooks");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Users__3214EC07FE71328F");

            entity.HasIndex(e => e.Email, "UQ__Users__A9D10534ADB7D1DF").IsUnique();

            entity.Property(e => e.AvatarPath).HasMaxLength(500);
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(sysutcdatetime())");
            entity.Property(e => e.DailyGoalPages).HasDefaultValue(30);
            entity.Property(e => e.DisplayName).HasMaxLength(100);
            entity.Property(e => e.Email).HasMaxLength(256);
            entity.Property(e => e.GoogleId).HasMaxLength(100);
            entity.Property(e => e.PasswordHash).HasMaxLength(500);
            entity.Property(e => e.SubscriptionPlan)
                .HasMaxLength(20)
                .HasDefaultValue("Free");
        });

        modelBuilder.Entity<UserAchievement>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__UserAchi__3214EC07228B5485");

            entity.HasIndex(e => new { e.UserId, e.AchievementCode }, "UQ_UserAchievements").IsUnique();

            entity.Property(e => e.AchievementCode).HasMaxLength(50);
            entity.Property(e => e.EarnedAt).HasDefaultValueSql("(sysutcdatetime())");

            entity.HasOne(d => d.User).WithMany(p => p.UserAchievements)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("FK_UA_Users");
        });

        modelBuilder.Entity<UserBook>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__UserBook__3214EC0733ACA7BC");

            entity.HasIndex(e => new { e.UserId, e.BookId }, "UQ_UserBooks").IsUnique();

            entity.Property(e => e.AddedAt).HasDefaultValueSql("(sysutcdatetime())");
            entity.Property(e => e.Status)
                .HasMaxLength(20)
                .HasDefaultValue("WantToRead");

            entity.HasOne(d => d.Book).WithMany(p => p.UserBooks)
                .HasForeignKey(d => d.BookId)
                .HasConstraintName("FK_UserBooks_Books");

            entity.HasOne(d => d.User).WithMany(p => p.UserBooks)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("FK_UserBooks_Users");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
