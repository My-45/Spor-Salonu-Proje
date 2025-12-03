using Microsoft.AspNetCore.Identity;
using SporSalonu.Models.Entities;

namespace SporSalonu.Data
{
    public static class DbSeeder
    {
        public static async Task SeedAsync(IServiceProvider serviceProvider)
        {
            var context = serviceProvider.GetRequiredService<ApplicationDbContext>();
            var userManager = serviceProvider.GetRequiredService<UserManager<User>>();
            var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();

            // Roller oluşturma kısmı
            string[] roles = { "Admin", "Uye" };
            foreach (var role in roles)
            {
                if (!await roleManager.RoleExistsAsync(role))
                {
                    await roleManager.CreateAsync(new IdentityRole(role));
                }
            }

            // Admin kullanıcısı oluşturma 
            var adminEmail = "g231210091@sakarya.edu.tr";
            var adminUser = await userManager.FindByEmailAsync(adminEmail);

            if (adminUser == null)
            {
                adminUser = new User
                {
                    UserName = adminEmail,
                    Email = adminEmail,
                    AdSoyad = "Admin User",
                    EmailConfirmed = true
                };

                var result = await userManager.CreateAsync(adminUser, "Sau12345");

                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(adminUser, "Admin");
                }
            }

            // Örnek Salon
            if (!context.Salonlar.Any())
            {
                var salon = new Salon
                {
                    Ad = "FitLife Spor Merkezi",
                    Adres = "Sakarya Üniversitesi Kampüsü",
                    AcilisSaati = new TimeSpan(6, 0, 0),
                    KapanisSaati = new TimeSpan(23, 0, 0),
                    Aktif = true
                };
                context.Salonlar.Add(salon);
                await context.SaveChangesAsync();

                // Örnek Uzmanlık Alanları
                var uzmanliklar = new List<UzmanlikAlani>
                {
                    new UzmanlikAlani { Ad = "Fitness", Aciklama = "Genel fitness antrenmanı" },
                    new UzmanlikAlani { Ad = "Yoga", Aciklama = "Yoga ve meditasyon" },
                    new UzmanlikAlani { Ad = "Pilates", Aciklama = "Pilates dersleri" },
                    new UzmanlikAlani { Ad = "Kilo Verme", Aciklama = "Kilo verme programları" },
                    new UzmanlikAlani { Ad = "Kas Geliştirme", Aciklama = "Vücut geliştirme" }
                };
                context.UzmanlikAlanlari.AddRange(uzmanliklar);
                await context.SaveChangesAsync();

                // Örnek Hizmetler
                var hizmetler = new List<HizmetTuru>
                {
                    new HizmetTuru { Ad = "Kişisel Antrenman", Aciklama = "Birebir antrenman", Sure = 60, Ucret = 200, SalonId = salon.Id },
                    new HizmetTuru { Ad = "Grup Dersi", Aciklama = "Grup fitness dersi", Sure = 45, Ucret = 100, SalonId = salon.Id },
                    new HizmetTuru { Ad = "Yoga Seansı", Aciklama = "Yoga dersi", Sure = 90, Ucret = 150, SalonId = salon.Id }
                };
                context.HizmetTurleri.AddRange(hizmetler);
                await context.SaveChangesAsync();

                // Örnek Antrenör
                var antrenor = new Antrenor
                {
                    AdSoyad = "Ahmet Yılmaz",
                    Email = "ahmet@fitlife.com",
                    Telefon = "05551234567",
                    Biyografi = "10 yıllık deneyimli fitness antrenörü",
                    SalonId = salon.Id,
                    Aktif = true
                };
                context.Antrenorler.Add(antrenor);
                await context.SaveChangesAsync();

                // Antrenör uzmanlıkları
                context.AntrenorUzmanliklar.AddRange(
                    new AntrenorUzmanlik { AntrenorId = antrenor.Id, UzmanlikAlaniId = uzmanliklar[0].Id },
                    new AntrenorUzmanlik { AntrenorId = antrenor.Id, UzmanlikAlaniId = uzmanliklar[4].Id }
                );

                // Müsaitlik saatleri (Pazartesi-Cuma, 09:00-18:00)
                for (int i = 1; i <= 5; i++)
                {
                    context.MusaitlikSaatleri.Add(new MusaitlikSaati
                    {
                        AntrenorId = antrenor.Id,
                        Gun = (DayOfWeek)i,
                        BaslangicSaati = new TimeSpan(9, 0, 0),
                        BitisSaati = new TimeSpan(18, 0, 0),
                        Aktif = true
                    });
                }

                await context.SaveChangesAsync();
            }
        }
    }
}