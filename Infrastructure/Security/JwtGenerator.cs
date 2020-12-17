using System;
using System.Collections.Generic;
using System.Security.Claims;
using Application.Interfaces;
using Domain.Models;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.Extensions.Configuration;

namespace Infrastructure.Security
{
  public class JwtGenerator : IJwtGenerator
  {
    private readonly SymmetricSecurityKey _key;

    public JwtGenerator(IConfiguration configuration)
    {
      _key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["TokenKey"]));
    }

    public string CreateToken(AppUser user)
    {
      var claims = new List<Claim>
      {
          new Claim(JwtRegisteredClaimNames.NameId, user.UserName)
      };

      var creds = new SigningCredentials(_key, SecurityAlgorithms.HmacSha512Signature);

      var tokenDescriptor = new SecurityTokenDescriptor
      {
        Subject = new ClaimsIdentity(claims),
        Expires = DateTime.Now.AddDays(7),
        SigningCredentials = creds
      };

      var tokenHandler = new JwtSecurityTokenHandler();

      var token = tokenHandler.CreateToken(tokenDescriptor);

      return tokenHandler.WriteToken(token);
    }
  }
}