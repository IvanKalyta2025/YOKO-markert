using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Api.Models;
using System.Text;

namespace Api.Services
{
    public class JwtTokenService
    {
        private readonly IConfiguration _configuration;
        public JwtTokenService(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        public string GenerateToken(User user)
        {
            // 1. Создаем Claims (данные внутри паспорта)
            var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()), // Самое важное для ProfileController
            new Claim(ClaimTypes.Email, user.Email)
        };

            // 2. Ключ подписи (Secret Key)
            // Берем из конфига или используем хардкод для dev-режима (но лучше в appsettings.json!)
            var keyString = _configuration["Jwt:Key"] ?? "super_secret_key_must_be_very_long_at_least_32_chars";
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(keyString));

            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            // 3. Выписываем токен
            var token = new JwtSecurityToken(
                issuer: null,
                audience: null,
                claims: claims,
                expires: DateTime.Now.AddDays(7), // Живет 7 дней
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}