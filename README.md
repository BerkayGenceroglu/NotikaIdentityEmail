<h1 align="center">🔐 Notika Identity & Email Doğrulama Sistemi</h1>

<p align="center">
  ASP.NET Core Identity altyapısı üzerine inşa edilmiş, e-posta doğrulama ve şifre sıfırlama özelliklerine sahip modern bir kimlik yönetim sistemi
</p>

---

## 🧾 Proje Tanıtımı

**NotikaIdentityEmail**, ASP.NET Core MVC teknolojisiyle geliştirilmiş, kullanıcı kimlik doğrulama süreçlerini kapsamlı biçimde ele alan bir web uygulamasıdır.

Bu proje sayesinde:

- Kullanıcılar sisteme güvenli şekilde **kayıt olabilir** ve **giriş yapabilir**,
- Kayıt sonrasında kullanıcıya **e-posta doğrulama bağlantısı** gönderilir,
- Şifresini unutan kullanıcılar **e-posta üzerinden şifre sıfırlayabilir**,
- Tüm bu süreçler **Notika Admin UI** temasıyla sunulan modern ve kullanıcı dostu bir arayüz üzerinden yürütülür.

Proje; **ASP.NET Core Identity**, **SMTP e-posta entegrasyonu**, **MVC mimarisi** ve **Razor View** kullanımı gibi temel .NET geliştirme pratiklerini bir arada sunmaktadır.

---

## 🚀 Kullanılan Teknolojiler

| Katman | Teknolojiler |
|--------|-------------|
| Backend | `ASP.NET Core MVC`, `ASP.NET Core Identity`, `C#` |
| Frontend | `HTML5`, `CSS3`, `JavaScript`, `Bootstrap` |
| Veritabanı | `MS SQL Server`, `Entity Framework Core`, `Code First Migration` |
| E-posta | `SMTP`, `MailKit` / `System.Net.Mail` |
| UI Tema | `Notika Admin Template` |
| IDE | `Visual Studio 2022` |
| Platform | `.NET (Any CPU)` |

---

## 🧱 Proje Mimarisi

<pre>
NotikaIdentityEmail/
│
├── NotikaIdentityEmail.sln              → Visual Studio Çözüm Dosyası
│
└── NotikaIdentityEmail/
    ├── NotikaIdentityEmail.csproj       → Proje Dosyası
    │
    ├── Controllers/                     → MVC Controller sınıfları
    │   ├── AccountController.cs         → Kayıt, Giriş, Çıkış işlemleri
    │   └── HomeController.cs            → Ana sayfa yönlendirmeleri
    │
    ├── Models/                          → Veri modelleri ve ViewModel'lar
    │   ├── AppUser.cs                   → Identity kullanıcı modeli
    │   ├── RegisterViewModel.cs
    │   ├── LoginViewModel.cs
    │   └── ForgotPasswordViewModel.cs
    │
    ├── Views/                           → Razor View (.cshtml) dosyaları
    │   ├── Account/
    │   │   ├── Register.cshtml
    │   │   ├── Login.cshtml
    │   │   ├── ForgotPassword.cshtml
    │   │   └── ResetPassword.cshtml
    │   └── Home/
    │       └── Index.cshtml
    │
    ├── wwwroot/                         → Statik dosyalar
    │   ├── css/
    │   ├── js/
    │   └── images/
    │
    ├── appsettings.json                 → Uygulama yapılandırması
    └── Program.cs                       → Uygulama giriş noktası
</pre>

---

## 🔐 Kimlik Doğrulama Akışı

```
Kullanıcı Kayıt Formunu Doldurur
            │
            ▼
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
      Sisteme Giriş Yapılabilir
            │
            ▼
  Şifre Unutulursa → Şifre Sıfırlama E-postası Gönderilir
            │
            ▼
    Yeni Şifre Belirlenir ve Giriş Yapılır
```


---

## 🖥️ Ekran Görüntüleri

### 📝 Kayıt Ol (Register)

Kullanıcıların sisteme kayıt olabilmesi için gerekli form alanlarını içerir.
Aşağıdaki bilgiler doldurularak üyelik tamamlanır:

- Ad Soyad
- Kullanıcı Adı
- E-posta
- Şifre ve Şifre Tekrarı

Kayıt sonrası kullanıcıya doğrulama e-postası otomatik olarak iletilir.

> 📸 *(Ekran görüntüsü buraya eklenecek)*

---

### 🔐 Giriş Yap (Login)

Mevcut kullanıcıların e-posta/kullanıcı adı ve şifre ile güvenli biçimde giriş yapmasını sağlar.
E-postası henüz doğrulanmamış kullanıcılara uyarı mesajı gösterilir.

> 📸 *(Ekran görüntüsü buraya eklenecek)*

---

### 📧 E-posta Doğrulama

Kullanıcı kayıt olduktan sonra belirttiği e-posta adresine doğrulama bağlantısı gönderilir.
Bağlantıya tıklayan kullanıcının hesabı aktif hale geçer.

- E-posta onaylanmadan sisteme giriş yapılamaz.
- Bağlantı süresi dolduğunda yeni doğrulama e-postası talep edilebilir.

> 📸 *(Ekran görüntüsü buraya eklenecek)*

---

### 🔑 Şifre Sıfırlama (Forgot Password)

Şifresini unutan kullanıcılar e-posta adreslerini girerek sıfırlama bağlantısı talep edebilir.

1. Kullanıcı e-posta adresini girer.
2. Sisteme kayıtlıysa sıfırlama bağlantısı gönderilir.
3. Bağlantıya tıklanarak yeni şifre belirlenir.
4. Şifre güncellendikten sonra giriş yapılabilir.

> 📸 *(Ekran görüntüsü buraya eklenecek)*

---

## 🛡️ Güvenlik Özellikleri

| Özellik | Açıklama |
|---------|----------|
| E-posta Doğrulama | Kayıt sonrası zorunlu e-posta onayı |
| Şifre Hash'leme | ASP.NET Core Identity ile otomatik PBKDF2 hash |
| Token Tabanlı Sıfırlama | Şifre sıfırlama tek kullanımlık token ile yapılır |
| Giriş Kilitleme | Başarısız deneme limitinde hesap kilitlenir (Lockout) |
| HTTPS | Üretim ortamında HTTPS yönlendirmesi aktif |


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
