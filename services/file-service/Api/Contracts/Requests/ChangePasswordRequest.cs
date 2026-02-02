namespace Api.Contracts.Requests
{
    public record ChangePasswordRequest(string Email, string CurrentPassword, string NewPassword);
}
