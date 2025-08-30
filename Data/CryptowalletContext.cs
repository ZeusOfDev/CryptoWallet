using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace CryptoWalletApp.Data;

public partial class CryptowalletContext : DbContext
{
    public CryptowalletContext()
    {
    }

    public CryptowalletContext(DbContextOptions<CryptowalletContext> options)
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

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        => optionsBuilder.UseSqlServer("Data Source=DESKTOP-Q0MPFHP\\SQLEXPRESS;Initial Catalog=cryptowallet;Integrated Security=True;Encrypt=True;Trust Server Certificate=True");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Crypto>(entity =>
        {
            entity.HasKey(e => e.CryptoId).HasName("PK__Crypto__44023B1DA18660BE");

            entity.ToTable("Crypto");

            entity.Property(e => e.CryptoId)
                .ValueGeneratedNever()
                .HasColumnName("CryptoID");
            entity.Property(e => e.CryptoName)
                .HasMaxLength(30)
                .IsUnicode(false);
            entity.Property(e => e.IconPath)
                .HasMaxLength(200)
                .IsUnicode(false);
            entity.Property(e => e.ShortName)
                .HasMaxLength(10)
                .IsUnicode(false);
        });

        modelBuilder.Entity<RefreshToken>(entity =>
        {
            entity.HasKey(e => e.Token).HasName("PK__RefreshT__1EB4F81601375AA8");

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
            entity.HasKey(e => e.RoleId).HasName("PK__Roles__8AFACE3AD64D95FA");

            entity.Property(e => e.RoleId).HasColumnName("RoleID");
            entity.Property(e => e.RoleDesc).HasColumnType("text");
            entity.Property(e => e.RoleName)
                .HasMaxLength(50)
                .IsUnicode(false);
        });

        modelBuilder.Entity<TradeTransaction>(entity =>
        {
            entity.HasKey(e => new { e.UserId, e.TradingCrypto }).HasName("PK__TradeTra__B9F7BF9BCB3B56FB");

            entity.ToTable("TradeTransaction");

            entity.Property(e => e.UserId).HasColumnName("UserID");
            entity.Property(e => e.TradingCrypto)
                .HasMaxLength(10)
                .IsUnicode(false);
            entity.Property(e => e.Amount)
                .HasMaxLength(20)
                .IsUnicode(false);
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
            entity.HasKey(e => e.TransactionId).HasName("PK__Transact__55433A4BD68E2DAC");

            entity.Property(e => e.TransactionId).HasColumnName("TransactionID");
            entity.Property(e => e.Amount)
                .HasMaxLength(20)
                .IsUnicode(false);
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
            entity.HasKey(e => e.UserId).HasName("PK__Users__1788CCAC5AD4733F");

            entity.Property(e => e.UserId).HasColumnName("UserID");
            entity.Property(e => e.Email)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.PasswordHash)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.UserBalance)
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.UserDesc).HasColumnType("text");
            entity.Property(e => e.UserName)
                .HasMaxLength(50)
                .IsUnicode(false);

            entity.HasMany(d => d.Roles).WithMany(p => p.Users)
                .UsingEntity<Dictionary<string, object>>(
                    "UserRole",
                    r => r.HasOne<Role>().WithMany()
                        .HasForeignKey("RoleId")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("fk_UserRole_Roles"),
                    l => l.HasOne<User>().WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("fk_UserRole_Users"),
                    j =>
                    {
                        j.HasKey("UserId", "RoleId").HasName("PK__UserRole__AF27604F4FF018A7");
                        j.ToTable("UserRole");
                        j.IndexerProperty<int>("UserId").HasColumnName("UserID");
                        j.IndexerProperty<int>("RoleId").HasColumnName("RoleID");
                    });
        });

        modelBuilder.Entity<UserCrypto>(entity =>
        {
            entity.HasKey(e => e.WalletId).HasName("PK__UserCryp__84D4F92E8D8FE44F");

            entity.ToTable("UserCrypto");

            entity.Property(e => e.WalletId)
                .HasMaxLength(16)
                .IsUnicode(false)
                .HasDefaultValueSql("(CONVERT([varchar](16),Crypt_Gen_Random((8)),(2)))")
                .HasColumnName("WalletID");
            entity.Property(e => e.CryptoId).HasColumnName("CryptoID");
            entity.Property(e => e.Ucbalance)
                .HasMaxLength(20)
                .IsUnicode(false)
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

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
