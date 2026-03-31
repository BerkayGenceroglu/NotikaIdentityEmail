using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using NotikaIdentityEmail.Models;
using NuGet.Common;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace NotikaIdentityEmail.Controllers
{
    public class TokenController : Controller
    {
        private readonly JwtSettingModel _jwtSettingModel;

        public TokenController(IOptions<JwtSettingModel> options)
        {
            _jwtSettingModel = options.Value;
        }

        [HttpGet]
        public IActionResult Generate()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Generate(SimpleUserViewModel model)
        {
            var claim = new Claim[]
            {
                new Claim("Name",model.Name),
                new Claim("Surname",model.Surname),
                new Claim("City",model.City),
                new Claim("Username",model.Username),
                new Claim(JwtRegisteredClaimNames.Jti,Guid.NewGuid().ToString()),
                //her token’i benzersiz yapmak.
                //Guid.NewGuid().ToString() ile her token’a eşsiz bir kimlik veriyoruz.
            };

            var symetricKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettingModel.Key));

            var creds = new SigningCredentials(symetricKey, SecurityAlgorithms.HmacSha256);
            //Token’i bu gizli anahtarla ve bu algoritmayla imzalayacağız
            var token = new JwtSecurityToken(
                issuer: _jwtSettingModel.Issuer,
                audience: _jwtSettingModel.Audience,
                claims: claim,
                expires: DateTime.UtcNow.AddMinutes(_jwtSettingModel.ExpireMinutes),
                signingCredentials: creds);

            model.Token = new JwtSecurityTokenHandler().WriteToken(token);
            return View(model);
            
        }
    }
}
