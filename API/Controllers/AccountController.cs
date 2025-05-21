using System.Security.Cryptography;
using System.Text;
using API.Data;
using API.DTOs;
using API.Entities;
using API.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers; 

public class AccountController(DataContext dataContext, ITokenService tokenService) : BaseApiController
{
    [HttpPost("Register")]
     public async Task<ActionResult<UserDto>> Register(RegisterDto registerDto)
    {
        if (await UserExist(registerDto.Username)) return BadRequest("Username is taken");

        using var hmac = new HMACSHA512();

        var user = new AppUser
        {
            userName = registerDto.Username.ToLower(),
            PasswordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(registerDto.Password)),
            PasswordSalt = hmac.Key
        };
        dataContext.Users.Add(user);
        await dataContext.SaveChangesAsync();

        return new UserDto
        {
            Username = user.userName,
            Token = tokenService.createToken(user)
        };
    }
    [HttpPost("login")]
    public async Task<ActionResult<UserDto>> Login(LoginDto loginDto)
    {

        var user = await dataContext.Users.FirstOrDefaultAsync(x =>
         x.userName == loginDto.Username.ToLower());

        if (User == null) return Unauthorized("Invalid Username");

        using var hmac = new HMACSHA512(user.PasswordSalt);
        var computeHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(loginDto.Password));
      
      for (int i = 0; i < computeHash.Length; i++)
      {if(computeHash[i]!= user.PasswordHash[i])
           return Unauthorized("Invalid Password");
      }

        return new UserDto
        {
            Username = user.userName,
            Token = tokenService.createToken(user)
        };
             
    }

    private async Task<bool> UserExist(string username)
    {
        return await dataContext.Users.AnyAsync(x => x.userName.ToLower() == username.ToLower());
    }

   
}
