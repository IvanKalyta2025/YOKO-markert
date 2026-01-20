using Api.Models;
using Api.Repositories;
using BCrypt.Net;
using System.ComponentModel.DataAnnotations;
using System.Security.Cryptography;
using System.Text;
using Api.Controllers;


namespace Api.Services;

public class UserAuthorizationService
{
    private readonly IUserRepository _repository;


    public UserAuthorizationService(IUserRepository repository)
    {
        _repository = repository;
    }

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
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password)
        };
        await _repository.AddAsync(user);
        await _repository.SaveChangesAsync();

        return new AuthResult(true, "Registration successful.", user);
    }

    public async Task<AuthResult> LoginAsync(LoginRequest request)
    {
        var user = await _repository.GetByEmailAsync(request.Email);

        if (user == null)
            return new AuthResult(false, "User not found.");

        bool isPasswordValid = BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash);
        if (!isPasswordValid)
            return new AuthResult(false, "Invalid password.");

        return new AuthResult(true, "Login successful.", user);
    }

    public async Task<AuthResult> ChangePasswordAsync(ChangePasswordRequest request)
    {

    }

}
