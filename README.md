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

### Yetkilendirme Kullanımı

Controller seviyesinde `[Authorize]` attribute'u ile koruma sağlanır:

```csharp
// Sadece Admin rolü erişebilir
[Authorize(Roles = "Admin")]
public class AdminController : Controller
{
    // Kullanıcı yönetimi
    public IActionResult Users() { ... }

    // Rol atama
    public IActionResult AssignRole(string userId) { ... }
}

// Giriş yapmış her kullanıcı erişebilir
[Authorize]
public class MailController : Controller
{
    public IActionResult Inbox() { ... }
}

// Herkese açık
[AllowAnonymous]
public IActionResult Login() { ... }
```

### Rol Atama Süreci

1. Admin panelinden kullanıcı listesi görüntülenir.
2. İlgili kullanıcının yanındaki **"Rol Ata"** butonuna tıklanır.
3. Açılan formdan mevcut roller arasından seçim yapılır.
4. Rol kaydedilir; kullanıcı bir sonraki girişinde yeni yetkilerle sisteme dahil olur.

---

## ✅ ModelState Kontrolleri ve Form Doğrulama

Tüm formlar, hem **istemci tarafında** (JavaScript) hem de **sunucu tarafında** (ModelState) doğrulanır.

### Sunucu Tarafı Doğrulama

```csharp
[HttpPost]
public async Task<IActionResult> Register(RegisterViewModel model)
{
    if (!ModelState.IsValid)
    {
        // Hatalı alanlar View'e geri döndürülür
        return View(model);
    }
    // Kayıt işlemi...
}
```

### ViewModel Örneği — Kayıt Formu

```csharp
public class RegisterViewModel
{
    [Required(ErrorMessage = "Ad alanı zorunludur.")]
    [StringLength(50, ErrorMessage = "Ad en fazla 50 karakter olabilir.")]
    public string FirstName { get; set; }

    [Required(ErrorMessage = "E-posta alanı zorunludur.")]
    [EmailAddress(ErrorMessage = "Geçerli bir e-posta adresi giriniz.")]
    public string Email { get; set; }

    [Required(ErrorMessage = "Şifre alanı zorunludur.")]
    [MinLength(6, ErrorMessage = "Şifre en az 6 karakter olmalıdır.")]
    [DataType(DataType.Password)]
    public string Password { get; set; }

    [Required(ErrorMessage = "Şifre tekrar alanı zorunludur.")]
    [Compare("Password", ErrorMessage = "Şifreler eşleşmiyor.")]
    [DataType(DataType.Password)]
    public string ConfirmPassword { get; set; }
}
```

### Hata Mesajlarının View'de Gösterimi

```html
<div class="form-group">
    <input asp-for="Email" class="form-control" />
    <span asp-validation-for="Email" class="text-danger"></span>
</div>
<div asp-validation-summary="All" class="text-danger"></div>
```

### Doğrulama Kapsamı

| Form | Kontrol Edilen Alanlar |
|------|----------------------|
| Kayıt | Ad, Soyad, E-posta, Şifre, Şifre Tekrar |
| Giriş | E-posta / Kullanıcı Adı, Şifre |
| Şifre Sıfırlama | E-posta adresi geçerliliği |
| Mail Yazma | Alıcı, Konu, İçerik boşluk kontrolü |
| Rol Atama | Kullanıcı ve rol seçimi zorunluluğu |
| Yorum | Minimum karakter, boşluk ve toxiclik kontrolü |

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

## ⚙️ Kurulum ve Çalıştırma

### Ön Koşullar

- [.NET SDK 8.0+](https://dotnet.microsoft.com/download)
- [Visual Studio 2022](https://visualstudio.microsoft.com/)
- [Python 3.9+](https://www.python.org/) (ToxicBERT servisi için)
- MS SQL Server (LocalDB veya Express)
- SMTP destekli bir e-posta hesabı (Gmail, Outlook vb.)

### 1. Projeyi Klonlayın

```bash
git clone https://github.com/BerkayGenceroglu/NotikaIdentityEmail.git
cd NotikaIdentityEmail
```

### 2. ToxicBERT Python Servisini Kurun

```bash
cd ToxicityApi
pip install flask transformers torch
python app.py
# Servis http://localhost:5001 adresinde çalışmaya başlar
```

### 3. `appsettings.json` Dosyasını Yapılandırın

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=NotikaIdentityDb;Trusted_Connection=True;"
  },
  "EmailSettings": {
    "SmtpServer": "smtp.gmail.com",
    "SmtpPort": 587,
    "SenderEmail": "your-email@gmail.com",
    "SenderPassword": "your-app-password",
    "SenderName": "NotikaApp"
  },
  "ToxicityApi": {
    "BaseUrl": "http://localhost:5001/predict"
  }
}
```

### 4. Veritabanı Migration Uygulayın

```bash
dotnet ef database update
```

veya **Package Manager Console** üzerinden:

```powershell
Update-Database
```

### 5. Uygulamayı Başlatın

```bash
dotnet run
```

ya da Visual Studio'da `F5` tuşuna basın.

---

## 📬 E-posta (SMTP) Yapılandırması

**Gmail için adım adım kurulum:**

1. Google hesabınızda **2 Adımlı Doğrulama**'yı etkinleştirin.
2. [Google Hesabı Güvenlik](https://myaccount.google.com/security) sayfasından **Uygulama Şifresi (App Password)** oluşturun.
3. Oluşturulan 16 haneli şifreyi `appsettings.json` içindeki `SenderPassword` alanına yapıştırın.

> ⚠️ **Güvenlik Uyarısı:** Gerçek şifrenizi doğrudan `appsettings.json` dosyasına yazmayın.
> Bunun yerine `.NET User Secrets` veya ortam değişkenleri kullanmanız önerilir.

```bash
dotnet user-secrets set "EmailSettings:SenderPassword" "your-app-password"
```

---

## 🖥️ Ekran Görüntüleri

### 📝 Kayıt Ol (Register)
Kullanıcıların sisteme kayıt olabilmesi için Ad, Soyad, Kullanıcı Adı, E-posta ve Şifre bilgilerini girdiği formdur.
Tüm alanlar ModelState ile doğrulanır; kayıt sonrası doğrulama e-postası otomatik olarak gönderilir.

> 📸 *(Ekran görüntüsü buraya eklenecek)*

---

### 🔐 Giriş Yap (Login)
E-posta / kullanıcı adı ve şifre ile güvenli giriş ekranı.
E-postası doğrulanmamış kullanıcılara uyarı mesajı gösterilir.

> 📸 *(Ekran görüntüsü buraya eklenecek)*

---

### 📥 Gelen Kutusu (Inbox)
Kullanıcıya gelen tüm maillerin listelendiği ekran.
Okundu/okunmadı durumu, gönderici ve konu bilgisi görüntülenir.

> 📸 *(Ekran görüntüsü buraya eklenecek)*

---

### 📤 Giden Kutusu (Sent)
Kullanıcının gönderdiği tüm maillerin listelendiği ekran.

> 📸 *(Ekran görüntüsü buraya eklenecek)*

---

### 📝 Taslaklar (Drafts)
Kullanıcının kaydettiği taslak maillerin listelendiği ve düzenlenebildiği ekran.

> 📸 *(Ekran görüntüsü buraya eklenecek)*

---

### 🗑️ Silinenler (Trash)
Silinen maillerin kalıcı silinmeden önce tutulduğu ekran.
Geri yükleme ve kalıcı silme işlemleri bu ekrandan yapılır.

> 📸 *(Ekran görüntüsü buraya eklenecek)*

---

### 🤖 Toxiclik Analizi
Kullanıcı yorumlarının ToxicBERT tarafından analiz edildiği ve sonuçların görüntülendiği ekran.
Her yoruma ait skor ve etiket (Temiz / Şüpheli / Zararlı) admin panelinden takip edilebilir.

> 📸 *(Ekran görüntüsü buraya eklenecek)*

---

### 👥 Rol Yönetimi (Admin)
Yöneticinin kullanıcılara rol atayabildiği admin paneli ekranı.

> 📸 *(Ekran görüntüsü buraya eklenecek)*

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

## 📦 Kullanılan NuGet Paketleri

```
Microsoft.AspNetCore.Identity.EntityFrameworkCore
Microsoft.EntityFrameworkCore.SqlServer
Microsoft.EntityFrameworkCore.Tools
MailKit
Newtonsoft.Json
```

---

## 🤝 Katkıda Bulunma

1. Bu repoyu **fork** edin
2. Yeni bir branch oluşturun: `git checkout -b feature/yeni-ozellik`
3. Değişikliklerinizi commit edin: `git commit -m "Yeni özellik eklendi"`
4. Branch'i push edin: `git push origin feature/yeni-ozellik`
5. **Pull Request** açın

---

## 📄 Lisans

Bu proje açık kaynaklıdır. Herhangi bir ticari veya kişisel amaçla kullanılabilir.

---

## 📫 İletişim

Proje hakkında sorularınız, önerileriniz ya da katkı istekleriniz için benimle iletişime geçebilirsiniz:

- 🔗 GitHub: [@BerkayGenceroglu](https://github.com/BerkayGenceroglu)

---

## 💬 Son Söz

Teşekkürler! Bu projeyi incelediğiniz veya katkıda bulunduğunuz için memnuniyet duyarım.
Her türlü geri bildirime açığım.

**İyi kodlamalar! 🚀**

---
