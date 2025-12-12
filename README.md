# Spor Salonu Yönetim ve Randevu Sistemi

ASP.NET Core MVC kullanılarak geliştirilmiş modern bir spor salonu yönetim sistemi. Üyelerin randevu alabildiği, antrenörlerin yönetilebildiği ve yapay zeka destekli egzersiz önerileri sunulan kapsamlı bir web uygulaması.

## 👨‍🎓 Öğrenci Bilgileri

- **Ad Soyad:** MERVE Yılmaz
- **Öğrenci No:** G231210091
- **Ders:** Web Programlama
- **Akademik Yıl:** 2025-2026 Güz Dönemi

## 📋 Proje Hakkında

Bu proje, spor salonlarının günlük operasyonlarını dijitalleştiren, üyelerin kolayca randevu alabildiği ve yapay zeka desteğiyle kişiselleştirilmiş egzersiz önerileri alabildikleri bir yönetim sistemidir. Sistem, antrenör müsaitlik takibi, otomatik randevu doğrulama ve rol bazlı yetkilendirme gibi gelişmiş özellikler sunmaktadır.

### Temel Özellikler

✅ **Spor Salonu Yönetimi**
- Çoklu salon desteği
- Çalışma saatleri ve hizmet türleri tanımlama
- Ücret ve süre yönetimi

✅ **Antrenör Yönetimi**
- Antrenör profilleri ve uzmanlık alanları
- Müsaitlik takvimi yönetimi
- Hizmet türü atamaları

✅ **Randevu Sistemi**
- Çakışma kontrolü ile akıllı randevu oluşturma
- Randevu onay mekanizması
- Detaylı randevu geçmişi

✅ **Yapay Zeka Entegrasyonu**
- Kişiselleştirilmiş egzersiz önerileri
- Vücut analizi ve diyet planı oluşturma
- Görsel tabanlı öneri sistemi

✅ **REST API**
- LINQ tabanlı filtreleme
- JSON formatında veri transferi
- Salon  ve randevu sorgulama

✅ **Güvenlik ve Yetkilendirme**
- Rol bazlı erişim kontrolü (Admin, Üye)
- Güvenli kimlik doğrulama
- Form ve sunucu tarafı validasyon

## 🛠️ Kullanılan Teknolojiler

### Backend
- **ASP.NET Core MVC** - Web framework
- **C#** - Programlama dili
- **Entity Framework Core** - ORM
- **LINQ** - Veri sorgulama
- **SQL Server / PostgreSQL** - Veritabanı

### Frontend
- **HTML5, CSS3** - Yapısal tasarım
- **Bootstrap 5** - Responsive tasarım
- **JavaScript & jQuery** - İnteraktif özellikler

### Diğer
- **OpenAI API** - Yapay zeka entegrasyonu
- **RESTful API** - Servis mimarisi

## 📊 Veritabanı Modeli

### Ana Tablolar

- **Gyms** - Spor salonu bilgileri
- **Trainers** - Antrenör profilleri ve uzmanlık alanları
- **Members** - Üye bilgileri
- **Appointments** - Randevu kayıtları
- **Services** - Hizmet türleri ve fiyatlandırma
- **Schedules** - Antrenör müsaitlik takvimi
- **AIRecommendations** - Yapay zeka önerileri

### İlişkiler
- Bir salonda birden fazla antrenör çalışabilir (1-N)
- Bir antrenör birden fazla hizmet sunabilir (N-N)
- Bir üye birden fazla randevu alabilir (1-N)
- Her randevu bir antrenör ve bir hizmete bağlıdır (N-1)





