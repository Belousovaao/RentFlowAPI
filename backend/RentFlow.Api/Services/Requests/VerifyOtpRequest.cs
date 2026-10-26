using System.ComponentModel.DataAnnotations;

namespace RentFlow.Api.Services.Requests;

public sealed record VerifyOtpRequest(
    Guid ChallengeId,

    [param: Required]
    string Channel,

    [param: Required]
    string Destination,

    [param: Required]
    string Code);