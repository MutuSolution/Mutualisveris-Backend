﻿namespace Application.Services.Identity;

public interface ICurrentUserService
{
    public string UserId { get; }
    public string UserName { get; }
    public string Email { get; }
}
