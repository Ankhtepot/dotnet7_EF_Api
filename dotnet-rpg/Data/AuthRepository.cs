using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using dotnet_rpg.Models;
using Microsoft.IdentityModel.Tokens;

namespace dotnet_rpg.Data;

public class AuthRepository : IAuthRepository
{
    private readonly DataContext _context;
    private readonly IConfiguration _configuration;

    public AuthRepository(DataContext context, IConfiguration configuration)
    {
        _context = context;
        _configuration = configuration;
    }

    public async Task<ServiceResponse<int>> Register(User user, string password)
    {
        ServiceResponse<int> response = new();

        if (await UserExists(user.Username))
        {
            response.Success = false;
            response.Message = "User already exists";
            return response;
        }

        CreatePasswordHash(password, out byte[] passwordHash, out byte[] passwordSalt);
        user.PasswordHash = passwordHash;
        user.PasswordSalt = passwordSalt;

        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        response.Data = user.Id;
        response.Message = "User registered";
        response.Success = true;

        return response;
    }

    public async Task<ServiceResponse<string>> Login(string username, string password)
    {
        ServiceResponse<string> response = new();
        
        User? user = await _context.Users.FirstOrDefaultAsync(u => u.Username.ToLower().Equals(username.ToLower()));

        if (user == null)
        {
            response.Success = false;
            response.Message = "User not found";
        }
        else if (!VerifyPasswordHash(password, user.PasswordHash, user.PasswordSalt))
        {
            response.Success = false;
            response.Message = "Wrong password";
        }
        else
        {
            response.Data = CreateToken(user);
            response.Message = "Login successful";
            response.Success = true;
        }

        return response;
    }

    public async Task<bool> UserExists(string username)
    {
        return await _context.Users.AnyAsync(u => u.Username.ToLower().Equals(username.ToLower()));
    }

    private void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt)
    {
        using HMACSHA512 hmac = new();
        passwordSalt = hmac.Key;
        passwordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
    }

    private bool VerifyPasswordHash(string password, byte[] passwordHash, byte[] passwordSalt)
    {
        using HMACSHA512 hmac = new(passwordSalt);
        byte[] computedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
        return computedHash.SequenceEqual(passwordHash);
    }

    private string CreateToken(User user)
    {
        List<Claim> claims = new()
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Name, user.Username),
        };
        
        string? appSettingsToken = _configuration.GetSection("AppSettings:Token").Value;
        
        if (appSettingsToken is null)
        {
            throw new ArgumentNullException(nameof(appSettingsToken));
        }
        
        SymmetricSecurityKey key = new(Encoding.UTF8.GetBytes(appSettingsToken));
        
        SigningCredentials credentials = new(key, SecurityAlgorithms.HmacSha512Signature);
        
        SecurityTokenDescriptor tokenDescriptor = new()
        {
            Subject = new ClaimsIdentity(claims),
            Expires = DateTime.Now.AddDays(1),
            SigningCredentials = credentials,
        };
        
        JwtSecurityTokenHandler tokenHandler = new();
        
        SecurityToken securityToken = tokenHandler.CreateToken(tokenDescriptor);
        
        return tokenHandler.WriteToken(securityToken);
    }
}