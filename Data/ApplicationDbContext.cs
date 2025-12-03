using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using SporSalonu.Models.Entities;
using System.Collections.Generic;
using System.Reflection.Emit;

namespace SporSalonu.Data
{
    public class ApplicationDbContext : IdentityDbContext<User>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Salon> Salonlar { get; set; }
        public DbSet<Antrenor> Antrenorler { get; set; }
        public DbSet<UzmanlikAlani> UzmanlikAlanlari { get; set; }
        public DbSet<AntrenorUzmanlik> AntrenorUzmanliklar { get; set; }
        public DbSet<HizmetTuru> HizmetTurleri { get; set; }
        public DbSet<MusaitlikSaati> MusaitlikSaatleri { get; set; }
        public DbSet<Randevu> Randevular { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // Çoka Çok ilişki
            builder.Entity<AntrenorUzmanlik>()
                .HasKey(au => new { au.AntrenorId, au.UzmanlikAlaniId });

            builder.Entity<AntrenorUzmanlik>()
                .HasOne(au => au.Antrenor)
                .WithMany(a => a.AntrenorUzmanliklar)
                .HasForeignKey(au => au.AntrenorId);

            builder.Entity<AntrenorUzmanlik>()
                .HasOne(au => au.UzmanlikAlani)
                .WithMany(u => u.AntrenorUzmanliklar)
                .HasForeignKey(au => au.UzmanlikAlaniId);

            // Randevu ilişki
            builder.Entity<Randevu>()
                .HasOne(r => r.Uye)
                .WithMany(u => u.Randevular)
                .HasForeignKey(r => r.UyeId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Randevu>()
                .HasOne(r => r.Antrenor)
                .WithMany(a => a.Randevular)
                .HasForeignKey(r => r.AntrenorId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}