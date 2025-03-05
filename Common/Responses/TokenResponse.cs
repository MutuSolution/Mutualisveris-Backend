namespace Common.Responses;

public record TokenResponse
(
     string Token,
     string RefreshToken,
     DateTime RefreshTokenExpiryTime
);