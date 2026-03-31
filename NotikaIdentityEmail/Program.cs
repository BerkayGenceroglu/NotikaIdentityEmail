using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using NotikaIdentityEmail.Context;
using NotikaIdentityEmail.Entities;
using NotikaIdentityEmail.Models;
using NuGet.Common;
using System.Net;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<EmailContext>();

builder.Services.AddIdentity<AppUser, IdentityRole>().AddEntityFrameworkStores<EmailContext>().AddErrorDescriber<CustomIdentityValidator>().AddTokenProvider<DataProtectorTokenProvider<AppUser>>(TokenOptions.DefaultProvider);


builder.Services.Configure<JwtSettingModel>(builder.Configuration.GetSection("JwtSettingsKey"));

// Cookie + JWT birlikte authentication
builder.Services.AddAuthentication(options =>
{
    options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;/*“Varsayılan olarak hangi yöntemle kullanıcıyı tanıyayım?”*/
    options.DefaultChallengeScheme = CookieAuthenticationDefaults.AuthenticationScheme; /*Giriş yapılmamışsa Cookie'nin login sayfasına yönlendir*/
    //DefaultScheme → "Kim bu kullanıcı?" sorusunu Cookie ile sor
    //DefaultChallengeScheme → "Yetkisi yok, ne yapacağız?" kararını Cookie'ye bırak
})
.AddCookie(CookieAuthenticationDefaults.AuthenticationScheme, options =>
{
    options.LoginPath = "/Login/UserLogin";
    options.AccessDeniedPath = "/ErrorPages/Page403";
    options.ExpireTimeSpan = TimeSpan.FromMinutes(5); // ⏱️ 5 dakika
    options.SlidingExpiration = false; // süre uzamasın
})
.AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, opt =>
{
    var jwtSettings = builder.Configuration.GetSection("JwtSettingsKey").Get<JwtSettingModel>();
    opt.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = jwtSettings.Issuer,
        ValidAudience = jwtSettings.Audience,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.Key))
    };
});
//.AddGoogle(options =>
//{
//    options.ClientId = "x " ;
//    options.ClientSecret = " y";
//    options.CallbackPath = "";
//}); ;




//builder.Services.AddAuthentication(options =>
//{
//    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme; /*Hangi yöntemi kullanarak kullanıcıyı doğrulayacağımızı söylüyor. Burada JwtBearer seçilmiş, yani JWT token ile.*/
//    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme; /*Eğer kullanıcı doğrulanamazsa hangi yöntemin çalışacağını belirler.Yine JWT token.*/
//}).AddJwtBearer(options =>
//{
//    var jwtSettings = builder.Configuration.GetSection("JwtSettingsKey").Get<JwtSettingModel>();
//    options.TokenValidationParameters = new TokenValidationParameters()
//    {
//        //True Olması Kontrolleri Aktif Kıl Demek
//        ValidateIssuer = true,
//        ValidateAudience = true,
//        ValidateLifetime = true,
//        ValidateIssuerSigningKey = true,

//        //Gönderen ve Alan Değeleri GÖsterilmektedir
//        ValidIssuer = jwtSettings.Issuer,
//        ValidAudience = jwtSettings.Audience,
//        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.Key))
//    };
//});


builder.Services.AddControllersWithViews();

var app = builder.Build();

app.UseStatusCodePagesWithReExecute("/Error/{0}");

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();


app.Run();
