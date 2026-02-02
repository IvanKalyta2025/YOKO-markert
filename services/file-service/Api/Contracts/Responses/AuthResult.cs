using Api.Domain.Entities;

namespace Api.Contracts.Responses
{
    public record AuthResult(bool Success, string Message, string Token = "", User? User = null);
}
