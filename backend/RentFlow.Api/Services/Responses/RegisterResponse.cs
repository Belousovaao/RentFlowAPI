namespace RentFlow.Api.Services.Responses;

public sealed record RegisterResponse(
    bool Registered,
    string Token);