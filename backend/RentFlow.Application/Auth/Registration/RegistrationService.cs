using RentFlow.Application.Auth.Otp;
using RentFlow.Application.Common.Exceptions;
using RentFlow.Application.Interfaces;
using RentFlow.Domain.Auth;
using RentFlow.Domain.Customers;

namespace RentFlow.Application.Auth.Registration;

public sealed class RegistrationService : IRegistrationService
{
    private readonly IRegistrationTicketService _registrationTickets;
    private readonly IAppUserRepository _users;
    private readonly ICustomerRepository _customers;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IOtpDestinationNormalizer _destinationNormalizer;
    private readonly IOtpSecurity _security;
    private readonly IConsentDocumentRepository _consentDocuments;

    public RegistrationService(
        IRegistrationTicketService registrationTickets,
        IAppUserRepository users,
        ICustomerRepository customers,
        IUnitOfWork unitOfWork,
        IOtpDestinationNormalizer destinationNormalizer,
        IOtpSecurity security,
        IConsentDocumentRepository consentDocuments)
    {
        ArgumentNullException.ThrowIfNull(registrationTickets);
        ArgumentNullException.ThrowIfNull(users);
        ArgumentNullException.ThrowIfNull(customers);
        ArgumentNullException.ThrowIfNull(unitOfWork);
        ArgumentNullException.ThrowIfNull(destinationNormalizer);
        ArgumentNullException.ThrowIfNull(security);
        ArgumentNullException.ThrowIfNull(consentDocuments);
        _registrationTickets = registrationTickets;
        _users = users;
        _customers = customers;
        _unitOfWork = unitOfWork;
        _destinationNormalizer = destinationNormalizer;
        _security = security;
        _consentDocuments = consentDocuments;
    }

    public async Task<RegistrationResult> RegisterAsync(
        string registrationTicket,
        RegisterCustomerData data,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(registrationTicket);

        if (!ValidateRequest(data))
        {
            return RegistrationResult.InvalidRequest();
        }

        var normalizedEmail = NormalizeEmail(data.Email);
        var normalizedPhone = _destinationNormalizer.Normalize(
            OtpChannel.Phone,
            data.Phone);

        var ticket = await _registrationTickets.GetAsync(
            registrationTicket,
            cancellationToken);

        if (ticket is null)
        {
            return RegistrationResult.InvalidTicket();
        }

        if (ticket.Channel == OtpChannel.Email)
        {
            if (normalizedEmail is null)
            {
                return RegistrationResult.InvalidRequest();
            }

            var contactHash = _security.HashDestination(normalizedEmail);

            if (!string.Equals(
                    contactHash,
                    ticket.DestinationHash,
                    StringComparison.Ordinal))
            {
                return RegistrationResult.ContactMismatch();
            }
        }
        else if (ticket.Channel == OtpChannel.Phone)
        {
            var submittedDestinationHash = _security.HashDestination(normalizedPhone);

            if (!string.Equals(
                    submittedDestinationHash,
                    ticket.DestinationHash,
                    StringComparison.Ordinal))
            {
                return RegistrationResult.ContactMismatch();
            }
        }
        else
        {
            throw new ArgumentOutOfRangeException(
                nameof(ticket.Channel),
                ticket.Channel,
                "Unsupported OTP channel.");
        }

        if (normalizedEmail is not null)
        {
            var existingByEmail = await _users.GetByEmailAsync(
                normalizedEmail,
                cancellationToken);

            if (existingByEmail is not null)
            {
                return RegistrationResult.AlreadyExists();
            }
        }

        var existingByPhone = await _users.GetByPhoneAsync(
            normalizedPhone,
            cancellationToken);

        if (existingByPhone is not null)
        {
            return RegistrationResult.AlreadyExists();
        }

        var consentDocument = await _consentDocuments.GetProcessingConsentByIdAsync(
            data.ConsentDocumentId,
            cancellationToken);

        if (consentDocument is null)
        {
            return RegistrationResult.InvalidRequest();
        }

        var customer = CreateCustomer(
            data,
            normalizedEmail,
            normalizedPhone);

        var consent = PersonalDataConsent.Grant(
            customer.Id,
            consentDocument,
            DateTime.UtcNow,
            ConsentGrantMethod.WebCheckbox);

        customer.AddPersonalDataConsent(consent);

        await _customers.AddAsync(
            customer,
            cancellationToken);

        var user = new AppUser
        {
            Id = Guid.NewGuid(),
            Email = normalizedEmail,
            Phone = normalizedPhone,
            FirstName = data.FirstName ?? string.Empty,
            LastName = data.LastName ?? string.Empty,
            Role = "user",
            CustomerId = customer.Id,
            CreatedAt = DateTime.UtcNow
        };

        await _users.AddAsync(
            user,
            cancellationToken);
        try
        {
            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }
        catch (UniqueConstraintViolationException ex) when (ex.ConstraintName is "UX_AppUsers_Email" or "UX_AppUsers_Phone")
        {
            return RegistrationResult.AlreadyExists();
        }
        
        var consumedTicket = await _registrationTickets.ConsumeAsync(
            registrationTicket,
            cancellationToken);

        if (consumedTicket is null)
        {
            throw new InvalidOperationException("Registration ticket could not be consumed after successful registration.");
        }

        return RegistrationResult.Success(user);
    }

    private static bool ValidateRequest(RegisterCustomerData data)
    {
        if (!data.PersonalDataConsentAccepted || data.ConsentDocumentId == Guid.Empty)
        {
            return false;
        }

        if (!Enum.IsDefined(data.CustomerType))
        {
            return false;
        }

        if (string.IsNullOrWhiteSpace(data.Phone))
        {
            return false;
        }

        return data.CustomerType switch
        {
            CustomerType.Individual =>
                !string.IsNullOrWhiteSpace(data.FirstName) &&
                !string.IsNullOrWhiteSpace(data.LastName),

            CustomerType.IndividualEntrepreneur =>
                !string.IsNullOrWhiteSpace(data.FirstName) &&
                !string.IsNullOrWhiteSpace(data.LastName) &&
                !string.IsNullOrWhiteSpace(data.Inn),

            CustomerType.Organization => !string.IsNullOrWhiteSpace(data.Inn),

            _ => false
        };
    }

    private Customer CreateCustomer(
        RegisterCustomerData data,
        string? email,
        string phone)
    {
        return data.CustomerType switch
        {
            CustomerType.Individual => Customer.CreateIndividual(
                    email,
                    phone,
                    new PersonName(
                        data.FirstName!,
                        data.LastName!,
                        null),
                    null,
                    null,
                    null,
                    null),

            CustomerType.IndividualEntrepreneur =>
                Customer.CreateIndividualEntrepreneur(
                    email,
                    phone,
                    new PersonName(
                        data.FirstName!,
                        data.LastName!,
                        null),
                    data.Inn!),

            CustomerType.Organization =>
                Customer.CreateOrganization(
                    email,
                    phone,
                    data.Inn!,
                    null),

            _ => throw new ArgumentOutOfRangeException(
                nameof(data.CustomerType),
                data.CustomerType,
                "Unsupported customer type.")
        };
    }

    private static string? NormalizeEmail(string? email)
    {
        if (string.IsNullOrWhiteSpace(email))
        {
            return null;
        }

        return email.Trim().ToLowerInvariant();
    }
}