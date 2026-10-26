using RentFlow.Domain.Customers;

namespace RentFlow.Api.Services.Requests;
public sealed record UpdateVerificationStatusRequest(VerificationStatus Status);