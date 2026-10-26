namespace RentFlow.Application.Auth.Registration;

public enum RegistrationResultStatus
{
    Success = 0,
    InvalidTicket = 1,
    ContactMismatch = 2,
    AlreadyExists = 3,
    InvalidRequest = 4
}