using FluentAssertions;
using Moq;
using RentFlow.Application.Auth.Otp;
using RentFlow.Application.Auth.Registration;
using RentFlow.Application.Common.Exceptions;
using RentFlow.Application.Interfaces;
using RentFlow.Domain.Auth;
using RentFlow.Domain.Customers;
using RentFlow.Domain.Customers.ValueObjects;

namespace RentFlow.Test.UnitTests.Application.Auth.Registration;

public sealed class RegistrationServiceTests
{
    private readonly Mock<IRegistrationTicketService> _registrationTickets = new();
    private readonly Mock<IAppUserRepository> _users = new();
    private readonly Mock<ICustomerRepository> _customers = new();
    private readonly Mock<IUnitOfWork> _unitOfWork = new();
    private readonly Mock<IOtpDestinationNormalizer> _destinationNormalizer = new();
    private readonly Mock<IOtpSecurity> _security = new();

    private RegistrationService CreateService()
    {
        return new RegistrationService(
            _registrationTickets.Object,
            _users.Object,
            _customers.Object,
            _unitOfWork.Object,
            _destinationNormalizer.Object,
            _security.Object);
    }

    [Fact]
    public async Task RegisterAsync_WithIndividual_CreatesCustomerAndUser()
    {
        const string ticket = "ticket";
        const string email = "user@example.com";
        const string phone = "+79991234567";
        const string emailHash = "email-hash";

        SetupEmailRegistration(
            ticket,
            email,
            emailHash);

        AppUser? addedUser = null;
        Customer? addedCustomer = null;

        _customers
            .Setup(x => x.AddAsync(
                It.IsAny<Customer>(),
                It.IsAny<CancellationToken>()))
            .Callback<Customer, CancellationToken>(
                (customer, _) => addedCustomer = customer)
            .Returns(Task.CompletedTask);

        _users
            .Setup(x => x.AddAsync(
                It.IsAny<AppUser>(),
                It.IsAny<CancellationToken>()))
            .Callback<AppUser, CancellationToken>(
                (user, _) => addedUser = user)
            .Returns(Task.CompletedTask);

        var service = CreateService();

        var result = await service.RegisterAsync(
            ticket,
            new RegisterCustomerData(
                CustomerType.Individual,
                email,
                phone,
                "Anna",
                "Volodina",
                null));

        result.Status
            .Should()
            .Be(RegistrationResultStatus.Success);

        result.User
            .Should()
            .NotBeNull();

        addedCustomer
            .Should()
            .BeOfType<Individual>();

        var individual = (Individual)addedCustomer!;

        individual.Email.Should().Be(email);
        individual.Phone.Should().Be(phone);
        individual.Name.FirstName.Should().Be("Anna");
        individual.Name.LastName.Should().Be("Volodina");
        individual.Name.MiddleName.Should().BeNull();

        addedUser.Should().NotBeNull();
        addedUser!.Email.Should().Be(email);
        addedUser.Phone.Should().Be(phone);
        addedUser.FirstName.Should().Be("Anna");
        addedUser.LastName.Should().Be("Volodina");
        addedUser.CustomerId.Should().Be(individual.Id);

        _unitOfWork.Verify(
            x => x.SaveChangesAsync(It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task RegisterAsync_WithIndividualEntrepreneur_CreatesCustomerAndUser()
    {
        const string ticket = "ticket";
        const string email = "ip@example.com";
        const string phone = "+79991234567";
        const string emailHash = "email-hash";
        const string inn = "123456789012";

        SetupEmailRegistration(
            ticket,
            email,
            emailHash);

        Customer? addedCustomer = null;

        _customers
            .Setup(x => x.AddAsync(
                It.IsAny<Customer>(),
                It.IsAny<CancellationToken>()))
            .Callback<Customer, CancellationToken>(
                (customer, _) => addedCustomer = customer)
            .Returns(Task.CompletedTask);

        var service = CreateService();

        var result = await service.RegisterAsync(
            ticket,
            new RegisterCustomerData(
                CustomerType.IndividualEntrepreneur,
                email,
                phone,
                "Anna",
                "Volodina",
                inn));

        result.Status
            .Should()
            .Be(RegistrationResultStatus.Success);

        addedCustomer
            .Should()
            .BeOfType<IndividualEntrepreneur>();

        var entrepreneur = (IndividualEntrepreneur)addedCustomer!;

        entrepreneur.Email.Should().Be(email);
        entrepreneur.Phone.Should().Be(phone);
        entrepreneur.Name.FirstName.Should().Be("Anna");
        entrepreneur.Name.LastName.Should().Be("Volodina");
        entrepreneur.INN.Should().Be(inn);
    }

    [Fact]
    public async Task RegisterAsync_WithOrganization_CreatesCustomerAndUser()
    {
        const string ticket = "ticket";
        const string email = "company@example.com";
        const string phone = "+79991234567";
        const string emailHash = "email-hash";
        const string inn = "1234567890";

        SetupEmailRegistration(
            ticket,
            email,
            emailHash);

        Customer? addedCustomer = null;

        _customers
            .Setup(x => x.AddAsync(
                It.IsAny<Customer>(),
                It.IsAny<CancellationToken>()))
            .Callback<Customer, CancellationToken>(
                (customer, _) => addedCustomer = customer)
            .Returns(Task.CompletedTask);

        var service = CreateService();

        var result = await service.RegisterAsync(
            ticket,
            new RegisterCustomerData(
                CustomerType.Organization,
                email,
                phone,
                null,
                null,
                inn));

        result.Status
            .Should()
            .Be(RegistrationResultStatus.Success);

        addedCustomer
            .Should()
            .BeOfType<Organization>();

        var organization = (Organization)addedCustomer!;

        organization.Email.Should().Be(email);
        organization.Phone.Should().Be(phone);
        organization.INN.Should().Be(inn);
    }

    [Fact]
    public async Task RegisterAsync_WithPhoneTicket_CreatesUserUsingPhone()
    {
        const string ticket = "ticket";
        const string phone = "+79991234567";
        const string phoneHash = "phone-hash";

        SetupPhoneRegistration(
            ticket,
            phone,
            phoneHash);

        var service = CreateService();

        var result = await service.RegisterAsync(
            ticket,
            new RegisterCustomerData(
                CustomerType.Individual,
                null,
                phone,
                "Anna",
                "Volodina",
                null));

        result.Status
            .Should()
            .Be(RegistrationResultStatus.Success);

        result.User.Should().NotBeNull();
        result.User!.Phone.Should().Be(phone);
        result.User.Email.Should().BeNull();

        _users.Verify(
            x => x.GetByEmailAsync(
                It.IsAny<string>(),
                It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [Fact]
    public async Task RegisterAsync_WithUnknownTicket_ReturnsInvalidTicket()
    {
        const string ticket = "unknown-ticket";

        _registrationTickets
            .Setup(x => x.GetAsync(
                ticket,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync((RegistrationTicket?)null);

        var service = CreateService();

        var result = await service.RegisterAsync(
            ticket,
            ValidIndividualData());

        result.Status
            .Should()
            .Be(RegistrationResultStatus.InvalidTicket);

        result.User.Should().BeNull();

        _registrationTickets.Verify(
            x => x.GetAsync(
                ticket,
                It.IsAny<CancellationToken>()),
            Times.Once);

        _registrationTickets.Verify(
            x => x.ConsumeAsync(
                It.IsAny<string>(),
                It.IsAny<CancellationToken>()),
            Times.Never);

        _users.VerifyNoOtherCalls();
        _customers.VerifyNoOtherCalls();
        _unitOfWork.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task RegisterAsync_WithEmailTicketAndDifferentEmail_ReturnsContactMismatch()
    {
        const string ticket = "ticket";
        const string ticketEmailHash = "original-email-hash";
        const string submittedEmail = "different@example.com";

       _registrationTickets
        .Setup(x => x.GetAsync(
            ticket,
            It.IsAny<CancellationToken>()))
        .ReturnsAsync(
            new RegistrationTicket(
                ticket,
                OtpChannel.Email,
                ticketEmailHash));

        _destinationNormalizer
            .Setup(x => x.Normalize(
                OtpChannel.Phone,
                It.IsAny<string>()))
            .Returns("+79991234567");

        _security
            .Setup(x => x.HashDestination(submittedEmail))
            .Returns("different-email-hash");

        var service = CreateService();

        var result = await service.RegisterAsync(
            ticket,
            new RegisterCustomerData(
                CustomerType.Individual,
                submittedEmail,
                "+79991234567",
                "Anna",
                "Volodina",
                null));

        result.Status
            .Should()
            .Be(RegistrationResultStatus.ContactMismatch);
        
        _registrationTickets.Verify(
            x => x.GetAsync(
                ticket,
                It.IsAny<CancellationToken>()),
            Times.Once);

        _registrationTickets.Verify(
            x => x.ConsumeAsync(
                It.IsAny<string>(),
                It.IsAny<CancellationToken>()),
            Times.Never);

        _users.VerifyNoOtherCalls();
        _customers.VerifyNoOtherCalls();
        _unitOfWork.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task RegisterAsync_WithPhoneTicketAndDifferentPhone_ReturnsContactMismatch()
    {
        const string ticket = "ticket";
        const string ticketPhoneHash = "original-phone-hash";
        const string submittedPhone = "+79990000000";

        _registrationTickets
            .Setup(x => x.GetAsync(
                ticket,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(new RegistrationTicket(
                ticket,
                OtpChannel.Phone,
                ticketPhoneHash));

        _destinationNormalizer
            .Setup(x => x.Normalize(
                OtpChannel.Phone,
                submittedPhone))
            .Returns(submittedPhone);

        _security
            .Setup(x => x.HashDestination(submittedPhone))
            .Returns("different-phone-hash");

        var service = CreateService();

        var result = await service.RegisterAsync(
            ticket,
            new RegisterCustomerData(
                CustomerType.Individual,
                "user@example.com",
                submittedPhone,
                "Anna",
                "Volodina",
                null));

        result.Status
            .Should()
            .Be(RegistrationResultStatus.ContactMismatch);

        _registrationTickets.Verify(
            x => x.GetAsync(
                ticket,
                It.IsAny<CancellationToken>()),
            Times.Once);

        _registrationTickets.Verify(
            x => x.ConsumeAsync(
                It.IsAny<string>(),
                It.IsAny<CancellationToken>()),
            Times.Never);

        _users.VerifyNoOtherCalls();
        _customers.VerifyNoOtherCalls();
        _unitOfWork.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task RegisterAsync_WithExistingEmail_ReturnsAlreadyExists()
    {
        const string ticket = "ticket";
        const string email = "existing@example.com";

        SetupEmailRegistration(
            ticket,
            email,
            "email-hash");

        _users
            .Setup(x => x.GetByEmailAsync(
                email,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(new AppUser());

        var service = CreateService();

        var result = await service.RegisterAsync(
            ticket,
            ValidIndividualData(email));

        result.Status
            .Should()
            .Be(RegistrationResultStatus.AlreadyExists);

        _registrationTickets.Verify(
            x => x.GetAsync(
                ticket,
                It.IsAny<CancellationToken>()),
            Times.Once);

        _registrationTickets.Verify(
            x => x.ConsumeAsync(
                It.IsAny<string>(),
                It.IsAny<CancellationToken>()),
            Times.Never);

        _customers.VerifyNoOtherCalls();

        _unitOfWork.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task RegisterAsync_WithExistingPhone_ReturnsAlreadyExists()
    {
        const string ticket = "ticket";
        const string email = "new@example.com";
        const string phone = "+79991234567";

        SetupEmailRegistration(
            ticket,
            email,
            "email-hash");

        _users
            .Setup(x => x.GetByPhoneAsync(
                phone,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(new AppUser());

        var service = CreateService();

        var result = await service.RegisterAsync(
            ticket,
            new RegisterCustomerData(
                CustomerType.Individual,
                email,
                phone,
                "Anna",
                "Volodina",
                null));

        result.Status
            .Should()
            .Be(RegistrationResultStatus.AlreadyExists);

        _registrationTickets.Verify(
            x => x.GetAsync(
                ticket,
                It.IsAny<CancellationToken>()),
            Times.Once);

        _registrationTickets.Verify(
            x => x.ConsumeAsync(
                It.IsAny<string>(),
                It.IsAny<CancellationToken>()),
            Times.Never);

        _customers.VerifyNoOtherCalls();

        _unitOfWork.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task RegisterAsync_WithInvalidIndividualData_ReturnsInvalidRequest()
    {
        var service = CreateService();

        var result = await service.RegisterAsync(
            "ticket",
            new RegisterCustomerData(
                CustomerType.Individual,
                "user@example.com",
                "+79991234567",
                null,
                "Volodina",
                null));

        result.Status
            .Should()
            .Be(RegistrationResultStatus.InvalidRequest);

        _registrationTickets.VerifyNoOtherCalls();
        _users.VerifyNoOtherCalls();
        _customers.VerifyNoOtherCalls();
        _unitOfWork.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task RegisterAsync_WithInvalidEntrepreneurData_ReturnsInvalidRequest()
    {
        var service = CreateService();

        var result = await service.RegisterAsync(
            "ticket",
            new RegisterCustomerData(
                CustomerType.IndividualEntrepreneur,
                "ip@example.com",
                "+79991234567",
                "Anna",
                "Volodina",
                null));

        result.Status
            .Should()
            .Be(RegistrationResultStatus.InvalidRequest);

        _registrationTickets.VerifyNoOtherCalls();
        _users.VerifyNoOtherCalls();
        _customers.VerifyNoOtherCalls();
        _unitOfWork.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task RegisterAsync_WithInvalidOrganizationData_ReturnsInvalidRequest()
    {
        var service = CreateService();

        var result = await service.RegisterAsync(
            "ticket",
            new RegisterCustomerData(
                CustomerType.Organization,
                "company@example.com",
                "+79991234567",
                null,
                null,
                null));

        result.Status
            .Should()
            .Be(RegistrationResultStatus.InvalidRequest);

        _registrationTickets.VerifyNoOtherCalls();
        _users.VerifyNoOtherCalls();
        _customers.VerifyNoOtherCalls();
        _unitOfWork.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task RegisterAsync_WhenSuccessful_ConsumesTicketAndSavesChanges()
    {
        const string ticket = "ticket";
        const string email = "user@example.com";

        SetupEmailRegistration(
            ticket,
            email,
            "email-hash");

        var service = CreateService();

        var result = await service.RegisterAsync(
            ticket,
            ValidIndividualData(email));

        result.Status
            .Should()
            .Be(RegistrationResultStatus.Success);

        _registrationTickets.Verify(
        x => x.GetAsync(
            ticket,
            It.IsAny<CancellationToken>()),
        Times.Once);

        _customers.Verify(
            x => x.AddAsync(
                It.IsAny<Customer>(),
                It.IsAny<CancellationToken>()),
            Times.Once);

        _users.Verify(
            x => x.AddAsync(
                It.IsAny<AppUser>(),
                It.IsAny<CancellationToken>()),
            Times.Once);

        _unitOfWork.Verify(
            x => x.SaveChangesAsync(
                It.IsAny<CancellationToken>()),
            Times.Once);

        _registrationTickets.Verify(
            x => x.ConsumeAsync(
                ticket,
                It.IsAny<CancellationToken>()),
            Times.Once);
    }

   [Fact]
    public async Task RegisterAsync_WhenSaveChangesFails_DoesNotConsumeTicket()
    {
        const string ticket = "ticket";
        const string email = "user@example.com";

        SetupEmailRegistration(
            ticket,
            email,
            "email-hash");

        _unitOfWork
            .Setup(x => x.SaveChangesAsync(
                It.IsAny<CancellationToken>()))
            .ThrowsAsync(new InvalidOperationException(
                "Database failure."));

        var service = CreateService();

        var action = () => service.RegisterAsync(
            ticket,
            ValidIndividualData(email));

        await action.Should()
            .ThrowAsync<InvalidOperationException>();

        _registrationTickets.Verify(
            x => x.GetAsync(
                ticket,
                It.IsAny<CancellationToken>()),
            Times.Once);

        _unitOfWork.Verify(
            x => x.SaveChangesAsync(
                It.IsAny<CancellationToken>()),
            Times.Once);

        _registrationTickets.Verify(
            x => x.ConsumeAsync(
                It.IsAny<string>(),
                It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [Fact]
    public async Task RegisterAsync_WithMissingPhone_ReturnsInvalidRequest()
    {
        var service = CreateService();

        var result = await service.RegisterAsync(
            "ticket",
            new RegisterCustomerData(
                CustomerType.Organization,
                "company@example.com",
                "",
                null,
                null,
                "1234567890"));

        result.Status
            .Should()
            .Be(RegistrationResultStatus.InvalidRequest);

        _registrationTickets.VerifyNoOtherCalls();
        _users.VerifyNoOtherCalls();
        _customers.VerifyNoOtherCalls();
        _unitOfWork.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task RegisterAsync_WithEmailTicketAndMissingEmail_ReturnsInvalidRequest()
    {
        const string ticket = "ticket";
        const string phone = "+79991234567";

        _registrationTickets
            .Setup(x => x.GetAsync(
                ticket,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(new RegistrationTicket(
                ticket,
                OtpChannel.Email,
                "email-hash"));

        _destinationNormalizer
            .Setup(x => x.Normalize(
                OtpChannel.Phone,
                phone))
            .Returns(phone);

        var service = CreateService();

        var result = await service.RegisterAsync(
            ticket,
            new RegisterCustomerData(
                CustomerType.Organization,
                null,
                phone,
                null,
                null,
                "1234567890"));

        result.Status
            .Should()
            .Be(RegistrationResultStatus.InvalidRequest);

        _registrationTickets.Verify(
            x => x.GetAsync(
                ticket,
                It.IsAny<CancellationToken>()),
            Times.Once);

        _registrationTickets.Verify(
            x => x.ConsumeAsync(
                It.IsAny<string>(),
                It.IsAny<CancellationToken>()),
            Times.Never);

        _users.VerifyNoOtherCalls();
        _customers.VerifyNoOtherCalls();
        _unitOfWork.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task RegisterAsync_WithEmailContainingWhitespaceAndUppercase_NormalizesEmail()
    {
        const string ticket = "ticket";
        const string submittedEmail = "  USER@EXAMPLE.COM  ";
        const string normalizedEmail = "user@example.com";
        const string phone = "+79991234567";
        const string emailHash = "email-hash";

        SetupEmailRegistration(
            ticket,
            normalizedEmail,
            emailHash);

        AppUser? addedUser = null;

        _users
            .Setup(x => x.AddAsync(
                It.IsAny<AppUser>(),
                It.IsAny<CancellationToken>()))
            .Callback<AppUser, CancellationToken>(
                (user, _) => addedUser = user)
            .Returns(Task.CompletedTask);

        var service = CreateService();

        var result = await service.RegisterAsync(
            ticket,
            new RegisterCustomerData(
                CustomerType.Individual,
                submittedEmail,
                phone,
                "Anna",
                "Volodina",
                null));

        result.Status
            .Should()
            .Be(RegistrationResultStatus.Success);

        addedUser
            .Should()
            .NotBeNull();

        addedUser!.Email
            .Should()
            .Be(normalizedEmail);

        _users.Verify(
            x => x.GetByEmailAsync(
                normalizedEmail,
                It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task RegisterAsync_WithUnnormalizedPhone_UsesNormalizedPhone()
    {
        const string ticket = "ticket";
        const string email = "user@example.com";
        const string submittedPhone = "+7 (999) 123-45-67";
        const string normalizedPhone = "+79991234567";
        const string emailHash = "email-hash";

        SetupEmailRegistration(
            ticket,
            email,
            emailHash);

        _destinationNormalizer
            .Setup(x => x.Normalize(
                OtpChannel.Phone,
                submittedPhone))
            .Returns(normalizedPhone);

        AppUser? addedUser = null;
        Customer? addedCustomer = null;

        _users
            .Setup(x => x.AddAsync(
                It.IsAny<AppUser>(),
                It.IsAny<CancellationToken>()))
            .Callback<AppUser, CancellationToken>(
                (user, _) => addedUser = user)
            .Returns(Task.CompletedTask);

        _customers
            .Setup(x => x.AddAsync(
                It.IsAny<Customer>(),
                It.IsAny<CancellationToken>()))
            .Callback<Customer, CancellationToken>(
                (customer, _) => addedCustomer = customer)
            .Returns(Task.CompletedTask);

        var service = CreateService();

        var result = await service.RegisterAsync(
            ticket,
            new RegisterCustomerData(
                CustomerType.Individual,
                email,
                submittedPhone,
                "Anna",
                "Volodina",
                null));

        result.Status
            .Should()
            .Be(RegistrationResultStatus.Success);

        addedUser
            .Should()
            .NotBeNull();

        addedUser!.Phone
            .Should()
            .Be(normalizedPhone);

        addedCustomer
            .Should()
            .NotBeNull();

        addedCustomer!.Phone
            .Should()
            .Be(normalizedPhone);

        _destinationNormalizer.Verify(
            x => x.Normalize(
                OtpChannel.Phone,
                submittedPhone),
            Times.Once);
    }

    [Fact]
    public async Task RegisterAsync_WithNormalizedEmailMatchingExistingUser_ReturnsAlreadyExists()
    {
        const string ticket = "ticket";
        const string submittedEmail = "  USER@EXAMPLE.COM  ";
        const string normalizedEmail = "user@example.com";
        const string phone = "+79991234567";
        const string emailHash = "email-hash";

        SetupEmailRegistration(
            ticket,
            normalizedEmail,
            emailHash);

        _users
            .Setup(x => x.GetByEmailAsync(
                normalizedEmail,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(new AppUser
            {
                Email = normalizedEmail
            });

        var service = CreateService();

        var result = await service.RegisterAsync(
            ticket,
            new RegisterCustomerData(
                CustomerType.Individual,
                submittedEmail,
                phone,
                "Anna",
                "Volodina",
                null));

        result.Status
            .Should()
            .Be(RegistrationResultStatus.AlreadyExists);

        result.User
            .Should()
            .BeNull();

        _users.Verify(
            x => x.GetByEmailAsync(
                normalizedEmail,
                It.IsAny<CancellationToken>()),
            Times.Once);

        _customers.VerifyNoOtherCalls();

        _unitOfWork.VerifyNoOtherCalls();

        _registrationTickets.Verify(
            x => x.ConsumeAsync(
                It.IsAny<string>(),
                It.IsAny<CancellationToken>()),
            Times.Never);
    }

   [Fact]
    public async Task RegisterAsync_WithOrganizationWithoutInn_ReturnsInvalidRequest()
    {
        var service = CreateService();

        var result = await service.RegisterAsync(
            "ticket",
            new RegisterCustomerData(
                CustomerType.Organization,
                "company@example.com",
                "+79991234567",
                null,
                null,
                null));

        result.Status
            .Should()
            .Be(RegistrationResultStatus.InvalidRequest);

        _registrationTickets.VerifyNoOtherCalls();
        _users.VerifyNoOtherCalls();
        _customers.VerifyNoOtherCalls();
        _unitOfWork.VerifyNoOtherCalls();
    }
    
    [Fact]
    public async Task RegisterAsync_WhenEmailUniqueConstraintIsViolated_ReturnsAlreadyExists()
    {
        const string ticket = "ticket";
        const string email = "user@example.com";
        const string emailHash = "email-hash";

        SetupEmailRegistration(
            ticket,
            email,
            emailHash);

        _unitOfWork
            .Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ThrowsAsync(
                new UniqueConstraintViolationException(
                    "UX_AppUsers_Email"));

        var service = CreateService();

        var result = await service.RegisterAsync(
            ticket,
            ValidIndividualData(email));

        result.Status
            .Should()
            .Be(RegistrationResultStatus.AlreadyExists);

        result.User
            .Should()
            .BeNull();

        _registrationTickets.Verify(
            x => x.ConsumeAsync(
                ticket,
                It.IsAny<CancellationToken>()),
            Times.Never);
    }
    private void SetupEmailRegistration(
        string ticket,
        string email,
        string emailHash)
    {
        var registrationTicket = new RegistrationTicket(
            ticket,
            OtpChannel.Email,
            emailHash);

        _registrationTickets
            .Setup(x => x.GetAsync(
                ticket,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(registrationTicket);

        _registrationTickets
            .Setup(x => x.ConsumeAsync(
                ticket,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(registrationTicket);

        _destinationNormalizer
            .Setup(x => x.Normalize(
                OtpChannel.Phone,
                It.IsAny<string>()))
            .Returns((OtpChannel _, string phone) => phone);

        _security
            .Setup(x => x.HashDestination(email))
            .Returns(emailHash);

        _users
            .Setup(x => x.GetByEmailAsync(
                email,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync((AppUser?)null);

        _users
            .Setup(x => x.GetByPhoneAsync(
                It.IsAny<string>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync((AppUser?)null);
    }

    [Fact]
    public async Task RegisterAsync_WhenPhoneUniqueConstraintIsViolated_ReturnsAlreadyExists()
    {
        const string ticket = "ticket";
        const string phone = "+79991234567";
        const string phoneHash = "phone-hash";

        SetupPhoneRegistration(
            ticket,
            phone,
            phoneHash);

        _unitOfWork
            .Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ThrowsAsync(
                new UniqueConstraintViolationException(
                    "UX_AppUsers_Phone"));

        var service = CreateService();

        var result = await service.RegisterAsync(
            ticket,
            new RegisterCustomerData(
                CustomerType.Individual,
                null,
                phone,
                "Anna",
                "Volodina",
                null));

        result.Status
            .Should()
            .Be(RegistrationResultStatus.AlreadyExists);

        result.User
            .Should()
            .BeNull();

        _registrationTickets.Verify(
            x => x.ConsumeAsync(
                ticket,
                It.IsAny<CancellationToken>()),
            Times.Never);
    }

    private void SetupPhoneRegistration(
        string ticket,
        string phone,
        string phoneHash)
    {
        var registrationTicket = new RegistrationTicket(
            ticket,
            OtpChannel.Phone,
            phoneHash);

        _registrationTickets
            .Setup(x => x.GetAsync(
                ticket,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(registrationTicket);

        _registrationTickets
            .Setup(x => x.ConsumeAsync(
                ticket,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(registrationTicket);

        _destinationNormalizer
            .Setup(x => x.Normalize(
                OtpChannel.Phone,
                It.IsAny<string>()))
            .Returns((OtpChannel _, string value) => value);

        _security
            .Setup(x => x.HashDestination(phone))
            .Returns(phoneHash);

        _users
            .Setup(x => x.GetByEmailAsync(
                It.IsAny<string>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync((AppUser?)null);

        _users
            .Setup(x => x.GetByPhoneAsync(
                phone,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync((AppUser?)null);
    }
    private RegisterCustomerData ValidIndividualData(
        string email = "user@example.com")
    {
        return new RegisterCustomerData(
            CustomerType.Individual,
            email,
            "+79991234567",
            "Anna",
            "Volodina",
            null);
    }
}