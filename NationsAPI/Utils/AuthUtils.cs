using NationsAPI.Models;
using NationsAPI.Repositories;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace NationsAPI.Utils
{
    public class AuthUtils : IAuthUtils
    {
        private readonly IConfiguration _config;
        public AuthUtils(IConfiguration config)
        {
            _config = config;
        }

        public string CreateToken(Player player)
        {
            var key = _config.GetValue<string>("Security:AuthKey");
            Console.WriteLine(key == null);
            var secretKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));
            var signingCredentials = new SigningCredentials(secretKey, SecurityAlgorithms.HmacSha256);

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, player.Nick),
                new Claim(ClaimTypes.Role, "Player"),
                new Claim(type: "playerId",value: player.Id.ToString())
            };

            var tokenOptions = new JwtSecurityToken(
                issuer: "http://localhost:53191",
                audience: "http://localhost:53191",
                expires: DateTime.Now.AddSeconds(10),
                claims: claims,
                signingCredentials: signingCredentials
                );
            var tokenString = new JwtSecurityTokenHandler().WriteToken(tokenOptions);
            return tokenString;
        }
    }
}
