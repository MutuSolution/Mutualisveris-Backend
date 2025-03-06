using System.ComponentModel;

namespace Common.Requests.Identity;

public record TokenRequest
{
    [DefaultValue("yunus@mail.com")]
    public string Email { get; init; }

    [DefaultValue("108484Yg.//")]
    public string Password { get; init; }
}
