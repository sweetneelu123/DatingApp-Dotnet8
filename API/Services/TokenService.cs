using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using API.Entities;
using API.Interfaces;
using Microsoft.IdentityModel.Tokens;

namespace API.Services;

public class TokenService(IConfiguration config) : ITokenService
{
    public string createToken(AppUser user)
    {
         var tokenKey = config["TokenKey"] ?? throw new Exception("Cannot access Token Key from appSetting");
         if(tokenKey.Length <64 ) throw new Exception ("Your Token Key need to be longer");
         var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(tokenKey));

         var claim = new List<Claim>
         {
            new (ClaimTypes.NameIdentifier, user.userName)
         };

         var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);

         var tokenDescripter = new SecurityTokenDescriptor
         {
            Subject = new ClaimsIdentity(claim),
            Expires =  DateTime.UtcNow.AddDays(5),
            SigningCredentials = creds

         };

      var tokenHandler = new JwtSecurityTokenHandler();
      var token = tokenHandler.CreateToken(tokenDescripter);

      return tokenHandler.WriteToken(token);
    }
}
