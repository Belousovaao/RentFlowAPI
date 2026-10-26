namespace RentFlow.Application.Auth.Registration;

public sealed class RegistrationOptions
{
    public const string SectionName = "Registration";

    public int TicketTtlSeconds { get; set; } = 1800;
}