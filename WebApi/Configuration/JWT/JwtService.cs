using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using WebApi.DataLayer;
using WebApi.Models;
using WebApi.Utilities;

namespace WebApi.Configuration.JWT
{
    public class JwtService : IJwtService
    {
        private readonly SiteSettings _siteSetting;

        public JwtService(IOptionsSnapshot<SiteSettings> settings)
        {
            _siteSetting = settings.Value;
        }

        public async Task<AccessToken> GenerateAsync(User user, Guid stamp)
        {
            var secretKey = Encoding.UTF8.GetBytes(_siteSetting.SecretKey); // longer that 16 character
            var signingCredentials = new SigningCredentials(new SymmetricSecurityKey(secretKey), SecurityAlgorithms.HmacSha256Signature);

            var encryptionkey = Encoding.UTF8.GetBytes(_siteSetting.EncryptKey); //must be 16 character
            var encryptingCredentials = new EncryptingCredentials(new SymmetricSecurityKey(encryptionkey), SecurityAlgorithms.Aes128KW, SecurityAlgorithms.Aes128CbcHmacSha256);

            var claims =GetClaims(user, stamp);

            var descriptor = new SecurityTokenDescriptor
            {
                Issuer = _siteSetting.Issuer,
                Audience = _siteSetting.Audience,
                IssuedAt = DateTime.Now,
                NotBefore = DateTime.Now.AddMinutes(_siteSetting.NotBeforeMinutes),
                Expires = DateTime.Now.AddMinutes(_siteSetting.ExpirationMinutes),
                SigningCredentials = signingCredentials,
                EncryptingCredentials = encryptingCredentials,
                Subject = new ClaimsIdentity(claims)
            };

         
            var tokenHandler = new JwtSecurityTokenHandler();

            var securityToken = tokenHandler.CreateJwtSecurityToken(descriptor);

            return new AccessToken(securityToken, user.Stamp);
        }

        private IEnumerable<Claim> GetClaims(User user, Guid stamp)
        {
            var securityStampClaimType = new ClaimsIdentityOptions().SecurityStampClaimType;

            var list = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.UserName),
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(securityStampClaimType, stamp.ToString())
            };


            return list;
        }
    }
}
