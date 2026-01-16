using Api.Models;
using Api.Repositories;
using System.ComponentModel.DataAnnotations;
using System.Security.Cryptography;
using System.Text;
using Api.Controllers;

namespace Api.Services;

public class UserService
{
    private readonly IUserRepository _repository;

    // private readonly RegisterRequest _registerRequest;

    public UserService(IUserRepository repository)
    {
        _repository = repository;
    }

    // public async Task<User> RegisterAsync(string email, string password)
    // {
    //     var existing = await _repository.GetByEmailAsync(email);
    //     if (existing != null)
    //         throw new InvalidOperationException("Email already in use.");

    //     else if (string.IsNullOrWhiteSpace(password) || password.Length < 6)
    //         throw new ArgumentException("Invalid password.");

    //     var user = new User
    //     {
    //         Email = email,
    //         PasswordHash = HashPassword(password)
    //     };

    //     await _repository.AddAsync(user);
    //     return user;

    // }

    public async Task<AuthResult> RegisterAsync(RegisterRequest request)
    {
        var existing = await _repository.GetByEmailAsync(request.Email);
        if (existing != null)
            return new AuthResult(false, "Email already in use.");

        if (string.IsNullOrWhiteSpace(request.Password) || request.Password.Length < 6)
            return new AuthResult(false, "Invalid password. Minimum 6 characters required.");

        var user = new User
        {
            Email = request.Email,
            PasswordHash = HashPassword(request.Password)
        };

        return new AuthResult(true, "Registration successful.", user);
    }

    public async Task<User?> LoginAsync(string email, string password)
    {
        var user = await _repository.GetByEmailAsync(email);
        if (user == null)
            return null;

        var hash = HashPassword(password);

        if (user.PasswordHash != hash)
            return null;

        return user;
    }

    private string HashPassword(string password)
    {
        using var sha256 = SHA256.Create();
        var bytes = Encoding.UTF8.GetBytes(password);
        var hash = sha256.ComputeHash(bytes);
        return Convert.ToBase64String(hash);
    }
}
