using System;
using System.Collections.Generic;
using CryptoWalletApp.Models;
using Microsoft.EntityFrameworkCore;

namespace CryptoWalletApp.Repositories;

public partial class CryptoWalletContext : DbContext
{
    public CryptoWalletContext()
    {
    }

    public CryptoWalletContext(DbContextOptions<CryptoWalletContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Crypto> Cryptos { get; set; }

    public virtual DbSet<RefreshToken> RefreshTokens { get; set; }

    public virtual DbSet<Role> Roles { get; set; }

    public virtual DbSet<TradeTransaction> TradeTransactions { get; set; }

    public virtual DbSet<Transaction> Transactions { get; set; }

    public virtual DbSet<User> Users { get; set; }

    public virtual DbSet<UserCrypto> UserCryptos { get; set; }

    public virtual DbSet<UserRole> UserRoles { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Data Source=DESKTOP-Q0MPFHP\\SQLEXPRESS;Initial Catalog=cryptowallet;Integrated Security=True;Encrypt=True;TrustServerCertificate=True");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Crypto>(entity =>
        {
            entity.HasKey(e => e.CryptoId).HasName("PK__Crypto__44023B1D1D7DA205");

            entity.ToTable("Crypto");

            entity.Property(e => e.CryptoId)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasColumnName("CryptoID");
            entity.Property(e => e.CryptoName)
                .HasMaxLength(30)
                .IsUnicode(false);
            entity.Property(e => e.IconPath)
                .HasMaxLength(200)
                .IsUnicode(false);
        });

        modelBuilder.Entity<RefreshToken>(entity =>
        {
            entity.HasKey(e => e.Token).HasName("PK__RefreshT__1EB4F81680B6D269");

            entity.ToTable("RefreshToken");

            entity.Property(e => e.Token)
                .HasMaxLength(16)
                .IsUnicode(false);
            entity.Property(e => e.IsRevoked).HasColumnName("isRevoked");
            entity.Property(e => e.UserId).HasColumnName("UserID");

            entity.HasOne(d => d.User).WithMany(p => p.RefreshTokens)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_RefreshToken_Users");
        });

        modelBuilder.Entity<Role>(entity =>
        {
            entity.HasKey(e => e.RoleId).HasName("PK__Roles__8AFACE3A8D40746F");

            entity.Property(e => e.RoleId)
                .HasMaxLength(30)
                .IsUnicode(false)
                .HasColumnName("RoleID");
            entity.Property(e => e.RoleDesc).HasColumnType("text");
        });

        modelBuilder.Entity<TradeTransaction>(entity =>
        {
            entity.HasKey(e => new { e.UserId, e.TradingCrypto }).HasName("PK__TradeTra__B9F7BF9BB1EAE1E9");

            entity.ToTable("TradeTransaction");

            entity.Property(e => e.UserId).HasColumnName("UserID");
            entity.Property(e => e.TradingCrypto)
                .HasMaxLength(10)
                .IsUnicode(false);
            entity.Property(e => e.Amount).HasColumnType("decimal(18, 6)");
            entity.Property(e => e.Tdate).HasColumnName("TDate");
            entity.Property(e => e.TradingAction)
                .HasMaxLength(10)
                .IsUnicode(false);

            entity.HasOne(d => d.User).WithMany(p => p.TradeTransactions)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_TradeTransaction_Users");
        });

        modelBuilder.Entity<Transaction>(entity =>
        {
            entity.HasKey(e => e.TransactionId).HasName("PK__Transact__55433A4B85D73249");

            entity.Property(e => e.TransactionId).HasColumnName("TransactionID");
            entity.Property(e => e.Amount).HasColumnType("decimal(18, 6)");
            entity.Property(e => e.Receiver)
                .HasMaxLength(16)
                .IsUnicode(false);
            entity.Property(e => e.Sender)
                .HasMaxLength(16)
                .IsUnicode(false);
            entity.Property(e => e.Tdate).HasColumnName("TDate");

            entity.HasOne(d => d.ReceiverNavigation).WithMany(p => p.TransactionReceiverNavigations)
                .HasForeignKey(d => d.Receiver)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_TradeTransaction_UserCryptoR");

            entity.HasOne(d => d.SenderNavigation).WithMany(p => p.TransactionSenderNavigations)
                .HasForeignKey(d => d.Sender)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_TradeTransaction_UserCryptoS");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.UserId).HasName("PK__Users__1788CCAC615BC8EF");

            entity.Property(e => e.UserId).HasColumnName("UserID");
            entity.Property(e => e.Email)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.PasswordHash)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.UserBalance).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.UserDesc).HasColumnType("text");
            entity.Property(e => e.UserName)
                .HasMaxLength(50)
                .IsUnicode(false);
        });

        modelBuilder.Entity<UserCrypto>(entity =>
        {
            entity.HasKey(e => e.WalletId).HasName("PK__UserCryp__84D4F92EE2FC5E0F");

            entity.ToTable("UserCrypto");

            entity.Property(e => e.WalletId)
                .HasMaxLength(16)
                .IsUnicode(false)
                .HasDefaultValueSql("(CONVERT([varchar](16),Crypt_Gen_Random((8)),(2)))")
                .HasColumnName("WalletID");
            entity.Property(e => e.CryptoId)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasColumnName("CryptoID");
            entity.Property(e => e.Ucbalance)
                .HasColumnType("decimal(18, 6)")
                .HasColumnName("UCBalance");
            entity.Property(e => e.UserId).HasColumnName("UserID");

            entity.HasOne(d => d.Crypto).WithMany(p => p.UserCryptos)
                .HasForeignKey(d => d.CryptoId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_UserCrypto_Crypto");

            entity.HasOne(d => d.User).WithMany(p => p.UserCryptos)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_UserCrypto_User");
        });

        modelBuilder.Entity<UserRole>(entity =>
        {
            entity.HasKey(e => e.Urid).HasName("PK__UserRole__AA3DE2FC85BB2D04");

            entity.ToTable("UserRole");

            entity.Property(e => e.Urid).HasColumnName("URId");
            entity.Property(e => e.RoleId)
                .HasMaxLength(30)
                .IsUnicode(false)
                .HasColumnName("RoleID");
            entity.Property(e => e.UserId).HasColumnName("UserID");

            entity.HasOne(d => d.Role).WithMany(p => p.UserRoles)
                .HasForeignKey(d => d.RoleId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_UserRole_Roles");

            entity.HasOne(d => d.User).WithMany(p => p.UserRoles)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_UserRole_Users");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
