namespace Common.Requests.Identity;

public record ChangePasswordRequest
{
    public string UserId { get; init; }
    public string CurrentPassword { get; init; }
    public string NewPassword { get; init; }
    public string ConfirmedNewPassword { get; init; }
}
