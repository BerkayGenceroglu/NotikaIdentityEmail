<h1 align="center">📬 Notika — Gerçek Zamanlı Mail Yönetim & Kimlik Doğrulama Sistemi</h1>

<p align="center">
  ASP.NET Core MVC ve ASP.NET Core Identity altyapısı ile geliştirilmiş, ToxicBERT yapay zekâ modeli entegrasyonu sayesinde kullanıcı yorumları ve mesajlar üzerinde zehirli içerik tespiti yapabilen; gelen kutusu, giden kutusu, taslaklar ve silinenler gibi tam özellikli mail yönetimi, rol tabanlı yetkilendirme, detaylı authorization kontrolleri, ModelState doğrulamaları ve admin paneli ile kullanıcı ve yetki yönetimi sunan modern, güvenli ve ölçeklenebilir bir web uygulamasıdır. Redis ile hızlı veri erişimi ve cache mekanizması, çoklu veritabanı desteği (MSSQL, PostgreSQL, MongoDB) ve Docker desteği sayesinde uygulama hem performanslı hem de esnek bir altyapıya sahiptir.
</p>

---

## 🧾 Proje Tanıtımı

**NotikaIdentityEmail**, modern bir **e-posta yönetim sistemi** ile güçlü bir **kimlik doğrulama altyapısını** tek çatı altında birleştiren kapsamlı bir web uygulamasıdır.

Bu proje sayesinde:

- Kullanıcılar **Gelen Kutusu**, **Giden Kutusu**, **Taslaklar** ve **Silinenler** olmak üzere tam posta kutusu deneyimi yaşayabilir,
- Kayıt sonrasında kullanıcıya **e-posta doğrulama bağlantısı** otomatik olarak gönderilir,
- Şifresini unutan kullanıcılar **e-posta üzerinden şifre sıfırlayabilir**,
- Yöneticiler kullanıcılara **rol atayabilir** ve yetki yönetimini merkezi olarak yapabilir,
- Kullanıcı yorumları **ToxicBERT yapay zeka modeli** ile analiz edilerek **toxiclik (zehirlilik) skoru** hesaplanır,
- Tüm formlar **ModelState doğrulaması** ile sunucu tarafında korunur,
- Uygulama, **Notika Admin UI** temasıyla sunulan sade ve kullanıcı dostu bir arayüze sahiptir.

---

## 🚀 Kullanılan Teknolojiler

| Katman | Teknolojiler |
|--------|-------------|
| Backend | `ASP.NET Core MVC`, `C#`, `ASP.NET Core Identity` |
| Yapay Zeka | `ToxicBERT` (Hugging Face), `Python`, `Flask/FastAPI` |
| Frontend | `HTML5`, `CSS3`, `JavaScript`, `Bootstrap`, `jQuery` |
| Veritabanı | `MS SQL Server`, `Entity Framework Core`, `Code First Migration` |
| E-posta | `SMTP`, `MailKit` / `System.Net.Mail` |
| Yetkilendirme | `Role-Based Authorization`, `[Authorize]`, `Claims` |
| Doğrulama | `ModelState`, `Data Annotations`, `Fluent Validation` |
| UI Tema | `Notika Admin Template` |
| IDE | `Visual Studio 2022` |

---

## 🧱 Proje Mimarisi

<pre>
NotikaIdentityEmail/
│
├── NotikaIdentityEmail.sln
│
└── NotikaIdentityEmail/
    ├── Controllers/
    │   ├── AccountController.cs         → Kayıt, Giriş, Çıkış, Şifre Sıfırlama
    │   ├── MailController.cs            → Gelen/Giden/Taslak/Silinenler Kutusu
    │   ├── AdminController.cs           → Kullanıcı ve Rol Yönetimi
    │   └── HomeController.cs
    │
    ├── Models/
    │   ├── AppUser.cs                   → Identity kullanıcı modeli
    │   ├── Mail.cs                      → Posta kutusu veri modeli
    │   ├── UserRole.cs                  → Rol modeli
    │   ├── ToxicityResult.cs            → ToxicBERT sonuç modeli
    │   └── ViewModels/
    │       ├── RegisterViewModel.cs
    │       ├── LoginViewModel.cs
    │       ├── ForgotPasswordViewModel.cs
    │       ├── ResetPasswordViewModel.cs
    │       ├── ComposeMailViewModel.cs
    │       └── UserRoleViewModel.cs
    │
    ├── Services/
    │   ├── IMailService.cs              → Mail servis arayüzü
    │   ├── MailService.cs               → SMTP gönderim servisi
    │   ├── IToxicityService.cs          → ToxicBERT servis arayüzü
    │   └── ToxicityService.cs           → Python API ile iletişim
    │
    ├── Data/
    │   └── AppDbContext.cs              → EF Core DbContext
    │
    ├── Views/
    │   ├── Account/
    │   │   ├── Register.cshtml
    │   │   ├── Login.cshtml
    │   │   ├── ForgotPassword.cshtml
    │   │   └── ResetPassword.cshtml
    │   ├── Mail/
    │   │   ├── Inbox.cshtml             → Gelen Kutusu
    │   │   ├── Sent.cshtml              → Giden Kutusu
    │   │   ├── Draft.cshtml             → Taslaklar
    │   │   ├── Trash.cshtml             → Silinenler
    │   │   └── Compose.cshtml           → Yeni Mail Yaz
    │   └── Admin/
    │       ├── Users.cshtml
    │       └── AssignRole.cshtml
    │
    ├── wwwroot/
    │   ├── css/
    │   ├── js/
    │   └── images/
    │
    ├── appsettings.json
    └── Program.cs
</pre>

---

## 📬 Posta Kutusu Sistemi

Uygulama, kullanıcılara tam teşekküllü bir e-posta deneyimi sunar. Her kullanıcı aşağıdaki posta kutularına sahiptir:

### 📥 Gelen Kutusu (Inbox)
- Kullanıcıya gelen tüm mailler listelenir.
- Okundu / Okunmadı durumu takip edilir.
- Mail detayı açıldığında otomatik olarak **okundu** olarak işaretlenir.
- Gelen mailler **tarih, gönderici ve konu** bilgileriyle görüntülenir.

### 📤 Giden Kutusu (Sent)
- Kullanıcının gönderdiği tüm mailler listelenir.
- Gönderim tarihi ve alıcı bilgisi görüntülenebilir.

### 📝 Taslaklar (Drafts)
- Kullanıcı yazmaya başladığı ama göndermediği mailleri taslak olarak kaydedebilir.
- Taslaklar daha sonra açılarak düzenlenebilir ve gönderilebilir.

### 🗑️ Silinenler (Trash)
- Silinen mailler kalıcı olarak silinmez; önce **Silinenler** kutusuna taşınır.
- Kullanıcı isterse silinen maili **geri yükleyebilir** ya da **kalıcı olarak silebilir**.

### ✉️ Mail Yazma (Compose)
- Alıcı adresi, konu ve içerik alanlarından oluşan mail yazma formu.
- Kullanıcı isterse **taslak olarak kaydedebilir** ya da doğrudan gönderebilir.

---

## 🤖 ToxicBERT ile Yorum Analizi

Uygulama, kullanıcıların yaptığı yorumları **ToxicBERT** modeli aracılığıyla analiz ederek içeriğin ne ölçüde zararlı/zehirli olduğunu belirler.

### ToxicBERT Nedir?
**ToxicBERT**, [Hugging Face](https://huggingface.co/) üzerinde yayımlanan ve özellikle metin bazlı **toxiclik (zehirlilik) tespiti** için ince ayar (fine-tune) yapılmış bir BERT tabanlı dil modelidir. Hakaret, tehdit, iftira gibi zararlı içerikleri yüksek doğrulukla tespit edebilir.

### Çalışma Akışı

```
Kullanıcı Yorum Gönderir
         │
         ▼
  ASP.NET Core Controller Yorumu Alır
         │
         ▼
  ToxicityService → Python Flask/FastAPI API'ye HTTP İsteği
         │
         ▼
  ToxicBERT Modeli Metni Analiz Eder
         │
         ▼
  Toxiclik Skoru Döner (0.0 - 1.0 arası)
         │
         ▼
  Skora Göre İçerik Etiketlenir:
    - 0.0 – 0.3 → ✅ Temiz
    - 0.3 – 0.6 → ⚠️ Şüpheli
    - 0.6 – 1.0 → 🚫 Zararlı / Engellendi
```

### Özellikler

- Yorumlar gönderilmeden önce **gerçek zamanlı analiz** yapılır.
- Zararlı bulunan içerikler **otomatik olarak engellenir** veya yönetici onayına gönderilir.
- Admin panelinde her yorumun toxiclik skoru ve etiketi görüntülenebilir.
- Analiz sonuçları veritabanında saklanarak raporlanabilir.

---

## 👥 Rol Tabanlı Yetkilendirme (Role-Based Authorization)

Uygulama, **ASP.NET Core Identity** üzerine inşa edilmiş kapsamlı bir rol yönetim sistemi içermektedir.

### Roller

| Rol | Yetkiler |
|-----|----------|
| `Admin` | Tüm kullanıcıları ve rolleri yönetebilir, tüm maillere erişebilir, toxiclik raporlarını görebilir |
| `User` | Kendi posta kutusunu yönetebilir, yorum yapabilir |

---
### Rol Atama Süreci

1. Admin panelinden kullanıcı listesi görüntülenir.
2. İlgili kullanıcının yanındaki **"Rol Ata"** butonuna tıklanır.
3. Açılan formdan mevcut roller arasından seçim yapılır.
4. Rol kaydedilir; kullanıcı bir sonraki girişinde yeni yetkilerle sisteme dahil olur.

---

## 🔐 Kimlik Doğrulama Akışı

```
Kullanıcı Kayıt Formunu Doldurur
            │
            ▼ ModelState Kontrolü
  ASP.NET Core Identity ile Kayıt
            │
            ▼
  Doğrulama E-postası Otomatik Gönderilir
            │
            ▼
  Kullanıcı E-postasındaki Linke Tıklar
            │
            ▼
        E-posta Onaylandı ✅
            │
            ▼
  Kullanıcıya Varsayılan "User" Rolü Atanır
            │
            ▼
      Sisteme Giriş Yapılabilir
            │
      ┌─────┴─────┐
      ▼           ▼
   Admin        User
  Paneli     Posta Kutusu
      │           │
  Kullanıcı    Gelen/Giden/
  & Rol Yönet  Taslak/Silinenler
```

---

## 🖥️ Ekran Görüntüleri

### 📝 Kayıt Ol (Register)
Kullanıcıların sisteme kayıt olabilmesi için Ad, Soyad, Kullanıcı Adı, E-posta ve Şifre bilgilerini girdiği formdur.
Tüm alanlar ModelState ile doğrulanır; kayıt sonrası doğrulama e-postası otomatik olarak gönderilir.

> <img width="1381" height="965" alt="Register" src="https://github.com/user-attachments/assets/8cec2f29-fd21-47c5-811c-3bc95f4585a0" />

<img width="878" height="793" alt="Onay" src="https://github.com/user-attachments/assets/26f47c8e-4a8a-43c5-bb04-34e369ce4f2e" />

<img width="1669" height="807" alt="emailKod" src="https://github.com/user-attachments/assets/5c62bd5f-cec0-4c31-83d7-07b6f647fd3b" />

<img width="1211" height="693" alt="Mail Doğrulandı" src="https://github.com/user-attachments/assets/051fe12f-46c3-4b6d-ba34-89a201199da5" />

---

### 🔐 Giriş Yap (Login)
E-posta / kullanıcı adı ve şifre ile güvenli giriş ekranı.
E-postası doğrulanmamış kullanıcılara uyarı mesajı gösterilir.
<img width="811" height="737" alt="image" src="https://github.com/user-attachments/assets/a6e090ed-ec5a-41a4-8504-9c9acb99881e" />

<img width="896" height="676" alt="ForgetPassword" src="https://github.com/user-attachments/assets/eda8680f-53cc-432e-882b-af50d35275f8" />

<img width="1624" height="667" alt="notikaşifreonay" src="https://github.com/user-attachments/assets/27780337-6fd5-429f-93cd-b0c1edf163c9" />

<img width="966" height="728" alt="newpassword" src="https://github.com/user-attachments/assets/8f232faa-2e4e-4034-82a6-081a309396c6" />

<img width="1263" height="552" alt="google2" src="https://github.com/user-attachments/assets/6aab71db-cef6-47a4-8ae2-97aa48e47fd1" />

<img width="1198" height="662" alt="google" src="https://github.com/user-attachments/assets/78555aad-1a86-4944-95e5-41815b3006d9" />

---
### 📥 Profilim (Profile)
Kullanıcının bilgilerini içeren kısımdır
<img width="1612" height="955" alt="profile" src="https://github.com/user-attachments/assets/184c9f6d-1913-4271-bb88-f5e70c699db4" />



---
### 📥 Gelen Kutusu (Inbox)
Kullanıcıya gelen tüm maillerin listelendiği ekran.
Okundu/okunmadı durumu, gönderici ve konu bilgisi görüntülenir.
okundu okunmadı soldan tıklayarak yapabiliriz
<img width="1602" height="956" alt="gelen kutusu" src="https://github.com/user-attachments/assets/6b7e7ffc-3655-43bf-b326-58e80daf542b" />
mesaj içeriğine tıklayınca mesaj açılır

<img width="1583" height="961" alt="image" src="https://github.com/user-attachments/assets/b0271771-dd16-4b0e-beaf-a90b4ab3e862" />


---

### 📤 Giden Kutusu (Sent)
Kullanıcının gönderdiği tüm maillerin listelendiği ekran.

<img width="1280" height="833" alt="image" src="https://github.com/user-attachments/assets/9fcc14e8-cd35-48cf-bfde-819fb7872a90" />
<img width="1533" height="955" alt="image" src="https://github.com/user-attachments/assets/8aa9ec7d-35d0-4982-b628-52467bc4b4db" />


---

### 📝 Taslaklar (Drafts)
Kullanıcının kaydettiği taslak maillerin listelendiği ve düzenlenebildiği ekran.
<img width="1471" height="953" alt="taslak list" src="https://github.com/user-attachments/assets/fcd890b9-a147-4c51-b83d-ff73ce83b998" />

<img width="1247" height="826" alt="Taslak Olarak Kaydet" src="https://github.com/user-attachments/assets/cfc0a2c3-1696-4b9c-8191-b714fc83cda7" />

<img width="1280" height="842" alt="taslak düzenle" src="https://github.com/user-attachments/assets/70ad4c91-b7a2-444e-969c-1965f5b5ff6f" />
<img width="1411" height="959" alt="image" src="https://github.com/user-attachments/assets/f9c98c93-fe54-4e6e-b422-06ae566659c0" />

---

### 🗑️ Silinenler (Trash)
Silinen maillerin kalıcı silinmeden önce tutulduğu ekran.
Geri yükleme ve kalıcı silme işlemleri bu ekrandan yapılır.

<img width="1613" height="957" alt="trashbox" src="https://github.com/user-attachments/assets/0a69cd23-eb4f-46a1-936a-9b12964e76a2" />

### 🗑️ Kategori Liste (Trash)
Silinen maillerin kalıcı silinmeden önce tutulduğu ekran.
Geri yükleme ve kalıcı silme işlemleri bu ekrandan yapılır.


### 🗑️ Silinenler (Trash)
Silinen maillerin kalıcı silinmeden önce tutulduğu ekran.
Geri yükleme ve kalıcı silme işlemleri bu ekrandan yapılır.

<img width="1613" height="957" alt="trashbox" src="https://github.com/user-attachments/assets/0a69cd23-eb4f-46a1-936a-9b12964e76a2" />

---

###  Kullanıcı Listesi
kullanıcılar listelenir
<img width="1564" height="954" alt="image" src="https://github.com/user-attachments/assets/6ee0d193-b6b3-4532-9d69-ff561e9118b1" />
<img width="1510" height="638" alt="image" src="https://github.com/user-attachments/assets/e34fc790-5714-4f7d-a737-83da039f60e1" />

<img width="1638" height="881" alt="image" src="https://github.com/user-attachments/assets/15733eb9-f2af-4bd0-aaff-f59b4ed89093" />
<img width="1457" height="705" alt="image" src="https://github.com/user-attachments/assets/590b48c7-3234-458f-8ff7-026bd69cd89c" />
<img width="1258" height="751" alt="image" src="https://github.com/user-attachments/assets/a13e8e16-b0c5-4423-8711-ccc914236ecb" />



---
### 🤖 Toxiclik Analizi
Kullanıcı yorumlarının ToxicBERT tarafından analiz edildiği ve sonuçların görüntülendiği ekran.
Her yoruma ait skor ve etiket (Temiz / Şüpheli / Zararlı) admin panelinden takip edilebilir.

<img width="1436" height="955" alt="image" src="https://github.com/user-attachments/assets/bdc9c4d8-be2c-47ca-b709-1e7611750ff4" />
<img width="1608" height="956" alt="image" src="https://github.com/user-attachments/assets/99539000-0fad-40e9-9fb5-6d1c7ee9196c" />
<img width="1392" height="818" alt="image" src="https://github.com/user-attachments/assets/f50da8e0-bb17-4561-b09b-0e654a9be908" />


---

### 👥 Yorum Yönetimi (Admin)
Yöneticinin yorumlara göre listeledi.

> 📸 *(Ekran görüntüsü buraya eklenecek)*
<img width="1588" height="958" alt="image" src="https://github.com/user-attachments/assets/396b007b-87eb-4479-99dc-c12e95757f81" />

---

## 🛡️ Güvenlik Özellikleri

| Özellik | Açıklama |
|---------|----------|
| E-posta Doğrulama | Kayıt sonrası zorunlu e-posta onayı |
| Şifre Hash'leme | ASP.NET Core Identity ile otomatik PBKDF2 hash |
| Token Tabanlı Sıfırlama | Şifre sıfırlama tek kullanımlık token ile yapılır |
| Giriş Kilitleme | Başarısız deneme limitinde hesap kilitlenir (Lockout) |
| Role-Based Authorization | Controller ve Action bazında `[Authorize(Roles="")]` koruması |
| ModelState Doğrulama | Tüm formlarda sunucu tarafı doğrulama zorunluluğu |
| ToxicBERT Koruması | Zararlı içerikler AI modeli tarafından tespit edilip engellenir |
| HTTPS | Üretim ortamında HTTPS yönlendirmesi aktif |


---


## 📄 Lisans

Bu proje açık kaynaklıdır. Herhangi bir ticari veya kişisel amaçla kullanılabilir,Tamamen Proje Yapma Amaçlı Kullanılmıştır.

---

## 📫 İletişim

Proje hakkında sorularınız, önerileriniz ya da katkı istekleriniz için benimle iletişime geçebilirsiniz:

- 🔗 GitHub: [@BerkayGenceroglu](https://github.com/BerkayGenceroglu)
---

## 💬 Son Söz

<img width="699" height="680" alt="image" src="https://github.com/user-attachments/assets/afc1d776-50f9-4b6e-85d2-63f8e706f53f" />


**İyi kodlamalar! 🚀**

---
