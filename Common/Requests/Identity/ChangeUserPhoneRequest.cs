namespace Common.Requests.Identity;

public class ChangeUserPhoneRequest
{
    public string UserId { get; init; }
    public string PhoneNumber { get; init; }
}