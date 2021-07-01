using System;
using System.IdentityModel.Tokens.Jwt;

namespace WebApi.Configuration.JWT
{
    public class AccessToken
    {
        public string access_token { get; set; }
        public string refresh_token { get; set; }
        public string token_type { get; set; }
        public string expires_in { get; set; }
        public string expires { get; set; }
        public Guid Guid { get; set; }

        public AccessToken(JwtSecurityToken securityToken, Guid guid)
        {
            access_token = new JwtSecurityTokenHandler().WriteToken(securityToken);
            token_type = "Bearer";
            expires_in = $"{(int)(securityToken.ValidTo - DateTime.UtcNow).TotalSeconds}(برحسب ثانیه)";
            Guid = guid;
        }
    }
}
