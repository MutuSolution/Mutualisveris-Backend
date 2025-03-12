namespace Common.Responses;

public record TokenResponse
(
     string id,
     string email,
     string firstName,
     string lastName,
     string userName,
     string phone,
     string exp,
     string[] roles,
     string Token,
     string RefreshToken,
     DateTime RefreshTokenExpiryTime
);