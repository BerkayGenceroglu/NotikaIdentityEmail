using Humanizer;
using Microsoft.AspNetCore.Identity;

namespace NotikaIdentityEmail.Models
{
    public class CustomIdentityValidator : IdentityErrorDescriber
    {
        public override IdentityError PasswordTooShort(int length)
        {
            return new IdentityError()
            {
                Code = "PasswordTooShort",
                Description=$"Şifreniz En Az {length} Karakter içermelidir"
            };
        }

        public override IdentityError PasswordRequiresLower()
        {
            return new IdentityError()
            {
                Code = "PasswordRequiresLower",
                Description = "Şifreniz En Az 1 Tane Küçük Harf İçermelidir"
            };
        }

        public override IdentityError PasswordRequiresUpper()
        {
            return new IdentityError()
            {
                Code = "PasswordRequiresUpper",
                Description = "Şifreniz En Az 1 Tane Büyük Harf İçermelidir"
            };
        }

        public override IdentityError PasswordRequiresDigit()
        {
            return new IdentityError()
            {
                Code = "PasswordRequiresDigit",
                Description = "Şifreniz En Az 1 Tane Rakam İçermelidir"
            };
        }

        public override IdentityError PasswordRequiresNonAlphanumeric()
        {
            return new IdentityError()
            {
                Code = "PasswordRequiresNonAlphanumeric",
                Description = "Şifreniz En Az 1 Tane Sembol İçermelidir"
            };
        }

        public override IdentityError DuplicateUserName(string userName)
        {
            return new IdentityError()
            {
                Code = "DuplicateUserName",
                Description = $"{userName} adlı Kullanıcı Adı, Zaten Alınmış.Lütfen Farklı Bir Kullanıcı Adı Deneyin"
            };
        }

    }
}
//Code: Bu, hatanın programatik adıdır. Yazılımcıların veya sistemin hatayı tanıması için kullanılan "benzersiz" bir etikettir.Kısaca Hatanın benzersiz tanımlayıcısı 
//Description: Bu, son kullanıcıya gösterilecek olan metindir. Hatayı insan dilinde açıklar.