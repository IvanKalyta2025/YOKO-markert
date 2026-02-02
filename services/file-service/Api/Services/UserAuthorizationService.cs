using Api.Contracts.Requests;
using Api.Contracts.Responses;
using Api.Domain.Entities;
using Api.Interfaces;




namespace Api.Services;

public class UserAuthorizationService
{
    private readonly IUserRepository _repository;
    private readonly JwtTokenService _jwtTokenService;


    public UserAuthorizationService(IUserRepository repository, JwtTokenService jwtTokenService)
    {
        _repository = repository;
        _jwtTokenService = jwtTokenService;
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

        var token = _jwtTokenService.GenerateToken(user);
        return new AuthResult(true, "Registration successful.", token, user);
    }

    public async Task<AuthResult> LoginAsync(LoginRequest request)
    {
        var user = await _repository.GetByEmailAsync(request.Email);

        if (user == null)
            return new AuthResult(false, "User not found.");

        bool isPasswordValid = BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash);
        if (!isPasswordValid)
            return new AuthResult(false, "Invalid password.");

        var token = _jwtTokenService.GenerateToken(user);
        return new AuthResult(true, "Login successful.", token, user);
    }

    public async Task<AuthResult> ChangePasswordAsync(ChangePasswordRequest request)
    {
        var user = await _repository.GetByEmailAsync(request.Email);

        if (user == null)
            return new AuthResult(false, "User not found.");

        bool isPasswordCorrect = BCrypt.Net.BCrypt.Verify(request.CurrentPassword, user.PasswordHash);
        if (!isPasswordCorrect)
            return new AuthResult(false, "Invalid password, try again to change your password.");

        user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.NewPassword);

        await _repository.SaveChangesAsync();

        var token = _jwtTokenService.GenerateToken(user);
        return new AuthResult(true, "Change successful", token, user);
    }

    // public record ChangePasswordRequest(string Email, string currentPassword, string newPassword);

}
