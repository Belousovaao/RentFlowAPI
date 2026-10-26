using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Security.Claims;
using System.Text.Json;
using System.Text.Json.Serialization;
using FluentAssertions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using RentFlow.Api.Services.Requests;
using RentFlow.Api.Services.Responses;
using RentFlow.Application.Auth.Otp;
using RentFlow.Application.Customers.Dtos;
using RentFlow.Application.Interfaces;
using RentFlow.Domain.Auth;
using RentFlow.Domain.Customers;
using RentFlow.Domain.Customers.ValueObjects;
using RentFlow.Persistance;
using RentFlow.Test.IntegrationTests.Infrastructure;
using RentFlow.Test.IntegrationTests.Infrastructure.Redis;
using Xunit;

namespace RentFlow.Test.IntegrationTests.Auth;

public sealed class RegisterTests : IClassFixture<PostgreSqlContainerFixture>, IClassFixture<RedisContainerFixture>
{
    private readonly PostgreSqlContainerFixture _postgresFixture;
    private readonly RedisContainerFixture _redisFixture;
    private static int _testIpCounter = 1;

    public RegisterTests(
        PostgreSqlContainerFixture postgresFixture,
        RedisContainerFixture redisFixture)
    {
        _postgresFixture = postgresFixture;
        _redisFixture = redisFixture;
    }

    private WebApplicationFactory<Program> CreateFactory()
    {
        var testIp = IPAddress.Parse($"10.0.0.{Interlocked.Increment(ref _testIpCounter)}");
        return new CustomWebApplicationFactory(_postgresFixture.ConnectionString)
            .WithWebHostBuilder(builder =>
            {
                builder.UseEnvironment("Testing");
                builder.ConfigureAppConfiguration((_, configuration) =>
                {
                    configuration.AddInMemoryCollection(
                        new Dictionary<string, string?>
                        {
                            ["Redis:ConnectionString"] = _redisFixture.ConnectionString,
                            ["OtpSecurity:HmacKey"] = Convert.ToBase64String(new byte[32])
                        });
                });

                builder.ConfigureTestServices(services =>
                {
                    services.AddSingleton<TestEmailOtpSender>();
                    services.AddSingleton<IEmailOtpSender>(sp => sp.GetRequiredService<TestEmailOtpSender>());
                    services.AddSingleton<TestPhoneOtpSender>();
                    services.AddSingleton<IPhoneOtpSender>(sp => sp.GetRequiredService<TestPhoneOtpSender>());
                    services.AddSingleton<IStartupFilter>(new TestRemoteIpStartupFilter(testIp));
                });
            });
    }

    private sealed class TestRemoteIpStartupFilter : IStartupFilter
    {
        private readonly IPAddress _ipAddress;
        public TestRemoteIpStartupFilter(IPAddress ipAddress)
        {
            _ipAddress = ipAddress;
        }

        public Action<IApplicationBuilder> Configure(Action<IApplicationBuilder> next)
        {
            return app =>
            {
                app.Use(async (context, nextMiddleware) =>
                {
                    context.Connection.RemoteIpAddress = _ipAddress;
                    await nextMiddleware();
                });

                next(app);
            };
        }
    }

    private static async Task<SendOtpResponse> SendEmailOtpAsync(
        HttpClient client,
        string email)
    {
        var response = await client.PostAsJsonAsync(
            "/api/auth/otp/send",
            new
            {
                channel = "email",
                destination = email
            });

        response.StatusCode
            .Should()
            .Be(HttpStatusCode.OK);

        var body = await response.Content.ReadFromJsonAsync<SendOtpResponse>();

        body.Should().NotBeNull();

        return body!;
    }

    private static async Task<string> VerifyEmailOtpAsync(
        HttpClient client,
        TestEmailOtpSender sender,
        SendOtpResponse sendResponse,
        string email)
    {
        sender.LastCode
            .Should()
            .NotBeNullOrWhiteSpace();

        var response = await client.PostAsJsonAsync(
            "/api/auth/otp/verify",
            new
            {
                challengeId = sendResponse.ChallengeId,
                channel = "email",
                destination = email,
                code = sender.LastCode
            });

        response.StatusCode
            .Should()
            .Be(HttpStatusCode.OK);

        var body =
            await response.Content
                .ReadFromJsonAsync<VerifyOtpResponse>(
                    new JsonSerializerOptions
                    {
                        PropertyNamingPolicy =
                            JsonNamingPolicy.CamelCase,
                        Converters =
                        {
                            new JsonStringEnumConverter()
                        }
                    });

        body.Should().NotBeNull();

        body!.Verified
            .Should()
            .BeTrue();

        body.Status
            .Should()
            .Be(OtpAuthenticationStatus.RegistrationRequired);

        body.Token
            .Should()
            .BeNull();

        body.RegistrationTicket
            .Should()
            .NotBeNullOrWhiteSpace();

        return body.RegistrationTicket!;
    }

    [Fact]
    public async Task Register_WithValidEmailTicket_CreatesCustomerAndUser()
    {
        await _postgresFixture.ResetDatabase();

        using var factory = CreateFactory();
        using var client = factory.CreateClient();

        var email = $"register-{Guid.NewGuid():N}@example.com";
        var phone = $"+7999{Random.Shared.Next(1000000, 9999999)}";
        var sender = factory.Services.GetRequiredService<TestEmailOtpSender>();
        var sendResponse = await SendEmailOtpAsync(
            client,
            email);
        var registrationTicket = await VerifyEmailOtpAsync(
            client,
            sender,
            sendResponse,
            email);

        var registerResponse = await client.PostAsJsonAsync(
            "/api/auth/register",
            new
            {
                registrationTicket,
                customerType = "Individual",
                email,
                phone,
                firstName = "Иван",
                lastName = "Иванов",
                inn = (string?)null
            });
        var body = await registerResponse.Content.ReadAsStringAsync();
        registerResponse.StatusCode
            .Should()
            .Be(HttpStatusCode.OK, $"response body: {body}");
        
        var registerBody = await registerResponse.Content.ReadFromJsonAsync<RegisterResponse>();
        registerBody.Should().NotBeNull();
        registerBody!.Registered
            .Should()
            .BeTrue();
        registerBody.Token
            .Should()
            .NotBeNullOrWhiteSpace();

        await using var db = await CreateDbContextAsync();

        var users = await db.AppUsers
            .Where(x =>
                x.Email == email &&
                x.Phone == phone)
            .ToListAsync();

        users.Should().ContainSingle();

        var user = users.Single();

        user.FirstName
            .Should()
            .Be("Иван");

        user.LastName
            .Should()
            .Be("Иванов");

        user.Role
            .Should()
            .Be("user");

        user.CustomerId
            .Should()
            .NotBeNull();

        var customer = await db.Customers
            .SingleAsync(x => x.Id == user.CustomerId);

        customer.Email
            .Should()
            .Be(email);

        customer.Phone
            .Should()
            .Be(phone);

        customer.Should()
            .BeOfType<Individual>();
    }

    [Fact]
    public async Task Register_WithValidEmailTicket_CreatesIndividualEntrepreneur()
    {
        await _postgresFixture.ResetDatabase();

        using var factory = CreateFactory();
        using var client = factory.CreateClient();

        var email = $"ip-{Guid.NewGuid():N}@example.com";

        var phone = $"+7999{Random.Shared.Next(1000000, 9999999)}";

        var sender = factory.Services.GetRequiredService<TestEmailOtpSender>();

        var sendResponse = await SendEmailOtpAsync(
            client,
            email);

        var registrationTicket = await VerifyEmailOtpAsync(
            client,
            sender,
            sendResponse,
            email);

        var registrationResponse = await client.PostAsJsonAsync(
            "/api/auth/register",
            new RegisterRequest(
                RegistrationTicket: registrationTicket,
                CustomerType: CustomerType.IndividualEntrepreneur,
                Email: email,
                Phone: phone,
                FirstName: "Иван",
                LastName: "Иванов",
                Inn: "123456789012"));

        var responseBody = await registrationResponse.Content.ReadAsStringAsync();

        registrationResponse.StatusCode
            .Should()
            .Be(HttpStatusCode.OK, $"response body: {responseBody}");

        var registrationResult = JsonSerializer.Deserialize<RegisterResponse>(
            responseBody,
            JsonSerializerOptions.Web);

        registrationResult.Should().NotBeNull();
        registrationResult!.Registered
            .Should()
            .BeTrue();

        registrationResult.Token
            .Should()
            .NotBeNullOrWhiteSpace();

        await using var db = await CreateDbContextAsync();

        var user = await db.AppUsers.SingleAsync(x => x.Email == email);

        user.Phone
            .Should()
            .Be(phone);

        user.FirstName
            .Should()
            .Be("Иван");

        user.LastName
            .Should()
            .Be("Иванов");

        user.CustomerId
            .Should()
            .NotBeNull();

        var customer =  await db.Customers.SingleAsync(x => x.Id == user.CustomerId);

        customer
            .Should()
            .BeOfType<IndividualEntrepreneur>();

        var entrepreneur = (IndividualEntrepreneur)customer;

        entrepreneur.Email
            .Should()
            .Be(email);

        entrepreneur.Phone
            .Should()
            .Be(phone);

        entrepreneur.Name.FirstName
            .Should()
            .Be("Иван");

        entrepreneur.Name.LastName
            .Should()
            .Be("Иванов");

        entrepreneur.INN
            .Should()
            .Be("123456789012");

        entrepreneur.IpLegalForm.FullName
            .Should()
            .Be(OrganizationLegalForm.IP.FullName);

        entrepreneur.IpLegalForm.ShortName
            .Should()
            .Be(OrganizationLegalForm.IP.ShortName);
    }

    [Fact]
    public async Task Register_WithValidEmailTicket_CreatesOrganization()
    {
        await _postgresFixture.ResetDatabase();

        using var factory = CreateFactory();
        using var client = factory.CreateClient();

        var email = $"org-{Guid.NewGuid():N}@example.com";
        var phone = $"+7999{Random.Shared.Next(1000000, 9999999)}";
        var sender = factory.Services.GetRequiredService<TestEmailOtpSender>();
        var sendResponse = await SendEmailOtpAsync(
            client,
            email);
        var registrationTicket = await VerifyEmailOtpAsync(
            client,
            sender,
            sendResponse,
            email);
        var registrationResponse = await client.PostAsJsonAsync(
            "/api/auth/register",
            new RegisterRequest(
                RegistrationTicket: registrationTicket,
                CustomerType: CustomerType.Organization,
                Email: email,
                Phone: phone,
                FirstName: null,
                LastName: null,
                Inn: "7701234567"));
        var responseBody = await registrationResponse.Content.ReadAsStringAsync();
        registrationResponse.StatusCode
            .Should()
            .Be(HttpStatusCode.OK, $"response body: {responseBody}");

        var registrationResult = JsonSerializer.Deserialize<RegisterResponse>(
            responseBody,
            JsonSerializerOptions.Web);

        registrationResult.Should().NotBeNull();
        registrationResult!.Registered
            .Should()
            .BeTrue();
        registrationResult.Token
            .Should()
            .NotBeNullOrWhiteSpace();

        await using var db = await CreateDbContextAsync();
        var user = await db.AppUsers.SingleAsync(x => x.Email == email);

        user.Phone
            .Should()
            .Be(phone);
        user.FirstName
            .Should()
            .BeEmpty();
        user.LastName
            .Should()
            .BeEmpty();
        user.Role
            .Should()
            .Be("user");
        user.CustomerId
            .Should()
            .NotBeNull();
        var customer = await db.Customers.SingleAsync(x => x.Id == user.CustomerId);
 
        customer
            .Should()
            .BeOfType<Organization>();

        var organization =(Organization)customer;

        organization.Email
            .Should()
            .Be(email);

        organization.Phone
            .Should()
            .Be(phone);

        organization.INN
            .Should()
            .Be("7701234567");
    }

    [Fact]
    public async Task Register_IndividualWithoutFirstName_ReturnsBadRequest()
    {
        await _postgresFixture.ResetDatabase();

        using var factory = CreateFactory();
        using var client = factory.CreateClient();

        var email = $"invalid-{Guid.NewGuid():N}@example.com";
        var phone = $"+7999{Random.Shared.Next(1000000, 9999999)}";
        var sender = factory.Services.GetRequiredService<TestEmailOtpSender>();
        var sendResponse = await SendEmailOtpAsync(
            client,
            email);
        var registrationTicket = await VerifyEmailOtpAsync(
            client,
            sender,
            sendResponse,
            email);
        var registrationResponse = await client.PostAsJsonAsync(
            "/api/auth/register",
            new RegisterRequest(
                RegistrationTicket: registrationTicket,
                CustomerType: CustomerType.Individual,
                Email: email,
                Phone: phone,
                FirstName: null,
                LastName: "Иванов",
                Inn: null));

        var responseBody = await registrationResponse.Content.ReadAsStringAsync();

        registrationResponse.StatusCode
            .Should()
            .Be(HttpStatusCode.BadRequest, $"response body: {responseBody}");

        await using var db = await CreateDbContextAsync();

        (await db.AppUsers.CountAsync())
            .Should()
            .Be(0);

        (await db.Customers.CountAsync())
            .Should()
            .Be(0);
    }

    [Fact]
    public async Task Register_IndividualWithoutLastName_ReturnsBadRequest()
    {
        await _postgresFixture.ResetDatabase();

        using var factory = CreateFactory();
        using var client = factory.CreateClient();

        var email = $"invalid-{Guid.NewGuid():N}@example.com";
        var phone = $"+7999{Random.Shared.Next(1000000, 9999999)}";
        var sender = factory.Services.GetRequiredService<TestEmailOtpSender>();

        var sendResponse = await SendEmailOtpAsync(
            client,
            email);

        var registrationTicket = await VerifyEmailOtpAsync(
            client,
            sender,
            sendResponse,
            email);

        var registrationResponse = await client.PostAsJsonAsync(
            "/api/auth/register",
            new RegisterRequest(
                RegistrationTicket: registrationTicket,
                CustomerType: CustomerType.Individual,
                Email: email,
                Phone: phone,
                FirstName: "Иван",
                LastName: null,
                Inn: null));

        var responseBody = await registrationResponse.Content.ReadAsStringAsync();

        registrationResponse.StatusCode
            .Should()
            .Be(HttpStatusCode.BadRequest, $"response body: {responseBody}");

        await using var db = await CreateDbContextAsync();

        (await db.AppUsers.CountAsync())
            .Should()
            .Be(0);

        (await db.Customers.CountAsync())
            .Should()
            .Be(0);
    }

    [Fact]
    public async Task Register_IndividualEntrepreneurWithoutFirstName_ReturnsBadRequest()
    {
        await _postgresFixture.ResetDatabase();

        using var factory = CreateFactory();
        using var client = factory.CreateClient();

        var email = $"invalid-ip-{Guid.NewGuid():N}@example.com";
        var phone = $"+7999{Random.Shared.Next(1000000, 9999999)}";
        var sender = factory.Services.GetRequiredService<TestEmailOtpSender>();

        var sendResponse = await SendEmailOtpAsync(
            client,
            email);

        var registrationTicket = await VerifyEmailOtpAsync(
            client,
            sender,
            sendResponse,
            email);

        var registrationResponse = await client.PostAsJsonAsync(
            "/api/auth/register",
            new RegisterRequest(
                RegistrationTicket: registrationTicket,
                CustomerType: CustomerType.IndividualEntrepreneur,
                Email: email,
                Phone: phone,
                FirstName: null,
                LastName: "Иванов",
                Inn: "123456789012"));

        var responseBody = await registrationResponse.Content.ReadAsStringAsync();

        registrationResponse.StatusCode
            .Should()
            .Be(HttpStatusCode.BadRequest, $"response body: {responseBody}");

        await using var db = await CreateDbContextAsync();

        (await db.AppUsers.CountAsync())
            .Should()
            .Be(0);

        (await db.Customers.CountAsync())
            .Should()
            .Be(0);
    }

    [Fact]
    public async Task Register_IndividualEntrepreneurWithoutLastName_ReturnsBadRequest()
    {
        await _postgresFixture.ResetDatabase();

        using var factory = CreateFactory();
        using var client = factory.CreateClient();

        var email = $"invalid-ip-{Guid.NewGuid():N}@example.com";
        var phone = $"+7999{Random.Shared.Next(1000000, 9999999)}";

        var sender = factory.Services.GetRequiredService<TestEmailOtpSender>();
        var sendResponse = await SendEmailOtpAsync(
            client,
            email);

        var registrationTicket = await VerifyEmailOtpAsync(
            client,
            sender,
            sendResponse,
            email);

        var registrationResponse = await client.PostAsJsonAsync(
            "/api/auth/register",
            new RegisterRequest(
                RegistrationTicket: registrationTicket,
                CustomerType: CustomerType.IndividualEntrepreneur,
                Email: email,
                Phone: phone,
                FirstName: "Иван",
                LastName: null,
                Inn: "123456789012"));

        var responseBody = await registrationResponse.Content.ReadAsStringAsync();

        registrationResponse.StatusCode
            .Should()
            .Be(HttpStatusCode.BadRequest, $"response body: {responseBody}");

        await using var db = await CreateDbContextAsync();

        (await db.AppUsers.CountAsync())
            .Should()
            .Be(0);

        (await db.Customers.CountAsync())
            .Should()
            .Be(0);
    }

    [Fact]
    public async Task Register_IndividualEntrepreneurWithoutInn_ReturnsBadRequest()
    {
        await _postgresFixture.ResetDatabase();

        using var factory = CreateFactory();
        using var client = factory.CreateClient();

        var email = $"invalid-ip-{Guid.NewGuid():N}@example.com";

        var phone = $"+7999{Random.Shared.Next(1000000, 9999999)}";

        var sender = factory.Services.GetRequiredService<TestEmailOtpSender>();

        var sendResponse = await SendEmailOtpAsync(
            client,
            email);

        var registrationTicket = await VerifyEmailOtpAsync(
            client,
            sender,
            sendResponse,
            email);

        var registrationResponse = await client.PostAsJsonAsync(
            "/api/auth/register",
            new RegisterRequest(
                RegistrationTicket: registrationTicket,
                CustomerType: CustomerType.IndividualEntrepreneur,
                Email: email,
                Phone: phone,
                FirstName: "Иван",
                LastName: "Иванов",
                Inn: null));

        var responseBody = await registrationResponse.Content.ReadAsStringAsync();

        registrationResponse.StatusCode
            .Should()
            .Be(HttpStatusCode.BadRequest, $"response body: {responseBody}");

        await using var db = await CreateDbContextAsync();

        (await db.AppUsers.CountAsync())
            .Should()
            .Be(0);

        (await db.Customers.CountAsync())
            .Should()
            .Be(0);
    }

    [Fact]
    public async Task Register_OrganizationWithoutInn_ReturnsBadRequest()
    {
        await _postgresFixture.ResetDatabase();

        using var factory = CreateFactory();
        using var client = factory.CreateClient();

        var email = $"invalid-org-inn-{Guid.NewGuid():N}@example.com";
        var phone = $"+7999{Random.Shared.Next(1000000, 9999999)}";

        var sender = factory.Services.GetRequiredService<TestEmailOtpSender>();
        var sendResponse = await SendEmailOtpAsync(
            client,
            email);

        var registrationTicket = await VerifyEmailOtpAsync(
            client,
            sender,
            sendResponse,
            email);

        var registrationResponse = await client.PostAsJsonAsync(
            "/api/auth/register",
            new RegisterRequest(
                RegistrationTicket: registrationTicket,
                CustomerType: CustomerType.Organization,
                Email: email,
                Phone: phone,
                FirstName: null,
                LastName: null,
                Inn: null));

        var responseBody =
            await registrationResponse.Content
                .ReadAsStringAsync();

        registrationResponse.StatusCode
            .Should()
            .Be(
                HttpStatusCode.BadRequest,
                $"response body: {responseBody}");

        await using var db =
            await CreateDbContextAsync();

        (await db.AppUsers.CountAsync())
            .Should()
            .Be(0);

        (await db.Customers.CountAsync())
            .Should()
            .Be(0);
    }
    
    [Fact]
    public async Task Register_WithValidEmailTicket_ReturnsJwtForCreatedUser()
    {
        await _postgresFixture.ResetDatabase();

        using var factory = CreateFactory();
        using var client = factory.CreateClient();

        var email = $"jwt-{Guid.NewGuid():N}@example.com";
        var phone = $"+7999{Random.Shared.Next(1000000, 9999999)}";
        var sender = factory.Services.GetRequiredService<TestEmailOtpSender>();
        var sendResponse = await SendEmailOtpAsync(
            client,
            email);
        var registrationTicket = await VerifyEmailOtpAsync(
            client,
            sender,
            sendResponse,
            email);
        var registerResponse = await client.PostAsJsonAsync(
            "/api/auth/register",
            new
            {
                registrationTicket,
                customerType = "Individual",
                email,
                phone,
                firstName = "Пётр",
                lastName = "Петров"
            });

        registerResponse.StatusCode
            .Should()
            .Be(HttpStatusCode.OK);

        var body = await registerResponse.Content.ReadFromJsonAsync<RegisterResponse>();

        body.Should().NotBeNull();
        body!.Token.Should().NotBeNullOrWhiteSpace();

        await using var db = await CreateDbContextAsync();

        var user = await db.AppUsers.SingleAsync(x => x.Email == email);

        var handler = new JwtSecurityTokenHandler();

        var token = handler.ReadJwtToken(body.Token);

        var nameIdentifier = token.Claims.FirstOrDefault(
            claim => claim.Type == ClaimTypes.NameIdentifier);

        nameIdentifier.Should().NotBeNull();

        nameIdentifier!.Value
            .Should()
            .Be(user.Id.ToString());
    }

    [Fact]
    public async Task Register_WithPhoneTicket_CreatesUserWithoutEmail()
    {
        await _postgresFixture.ResetDatabase();

        using var factory = CreateFactory();
        using var client = factory.CreateClient();

        var phone = $"+7999{Random.Shared.Next(1000000, 9999999)}";

        var sender = factory.Services.GetRequiredService<TestPhoneOtpSender>();

        var sendResponse = await client.PostAsJsonAsync(
            "/api/auth/otp/send",
            new
            {
                channel = "phone",
                destination = phone
            });

        sendResponse.StatusCode
            .Should()
            .Be(HttpStatusCode.OK);

        var sendBody = await sendResponse.Content.ReadFromJsonAsync<SendOtpResponse>();

        sendBody.Should().NotBeNull();
        sender.LastPhone
            .Should()
            .Be(phone);

        sender.LastCode
            .Should()
            .NotBeNullOrWhiteSpace();

        var verifyResponse = await client.PostAsJsonAsync(
            "/api/auth/otp/verify",
            new
            {
                challengeId = sendBody!.ChallengeId,
                channel = "phone",
                destination = phone,
                code = sender.LastCode
            });

        verifyResponse.StatusCode
            .Should()
            .Be(HttpStatusCode.OK);

        var verifyBody = await verifyResponse.Content.ReadFromJsonAsync<VerifyOtpResponse>(
            new JsonSerializerOptions
            {
                PropertyNamingPolicy =
                    JsonNamingPolicy.CamelCase,
                Converters =
                {
                    new JsonStringEnumConverter()
                }
            });

        verifyBody.Should().NotBeNull();

        verifyBody!.Verified
            .Should()
            .BeTrue();

        verifyBody.Status
            .Should()
            .Be(OtpAuthenticationStatus.RegistrationRequired);

        verifyBody.Token
        .Should()
        .BeNull();

        verifyBody.RegistrationTicket
            .Should()
            .NotBeNullOrWhiteSpace();

        var registrationResponse = await client.PostAsJsonAsync(
            "/api/auth/register",
            new RegisterRequest(
                RegistrationTicket: verifyBody.RegistrationTicket!,
                CustomerType: CustomerType.Individual,
                Email: null,
                Phone: phone,
                FirstName: "Иван",
                LastName: "Иванов",
                Inn: null));

        var registrationBody = await registrationResponse.Content.ReadAsStringAsync();

        registrationResponse.StatusCode
            .Should()
            .Be(HttpStatusCode.OK, $"response body: {registrationBody}");

        var registrationResult = JsonSerializer.Deserialize<RegisterResponse>(
            registrationBody,
            JsonSerializerOptions.Web);

        registrationResult.Should().NotBeNull();
        registrationResult!.Registered
            .Should()
            .BeTrue();
        registrationResult.Token
            .Should()
            .NotBeNullOrWhiteSpace();

        await using var db = await CreateDbContextAsync();
        var user = await db.AppUsers.SingleAsync(x => x.Phone == phone);
        user.Email
            .Should()
            .BeNull();
        user.Phone
            .Should()
            .Be(phone);
        user.FirstName
            .Should()
            .Be("Иван");
        user.LastName
            .Should()
            .Be("Иванов");
        user.Role
            .Should()
            .Be("user");
        user.CustomerId
            .Should()
            .NotBeNull();

        var customer = await db.Customers.SingleAsync(x => x.Id == user.CustomerId);
        customer.Email
            .Should()
            .BeNull();
        customer.Phone
            .Should()
            .Be(phone);
        customer
            .Should()
            .BeOfType<Individual>();
    }

    [Fact]
    public async Task Register_WithDifferentEmailThanVerifiedContact_ReturnsBadRequest()
    {
        await _postgresFixture.ResetDatabase();

        using var factory = CreateFactory();
        using var client = factory.CreateClient();

        var verifiedEmail = $"verified-{Guid.NewGuid():N}@example.com";
        var submittedEmail = $"different-{Guid.NewGuid():N}@example.com";
        var phone = $"+7999{Random.Shared.Next(1000000, 9999999)}";
        var sender = factory.Services.GetRequiredService<TestEmailOtpSender>();
        var sendResponse = await SendEmailOtpAsync(
            client,
            verifiedEmail);

        var registrationTicket = await VerifyEmailOtpAsync(
            client,
            sender,
            sendResponse,
            verifiedEmail);

        var registerResponse = await client.PostAsJsonAsync(
            "/api/auth/register",
            new
            {
                registrationTicket,
                customerType = "Individual",
                email = submittedEmail,
                phone,
                firstName = "Иван",
                lastName = "Иванов"
            });

        registerResponse.StatusCode
            .Should()
            .Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task Register_WithInvalidTicket_ReturnsBadRequest()
    {
        await _postgresFixture.ResetDatabase();

        using var factory = CreateFactory();
        using var client = factory.CreateClient();

        var response = await client.PostAsJsonAsync(
            "/api/auth/register",
            new
            {
                registrationTicket =
                    "invalid-registration-ticket",

                customerType = "Individual",
                email = "test@example.com",
                phone = "+79991234567",
                firstName = "Иван",
                lastName = "Иванов"
            });
    
        var body = await response.Content.ReadAsStringAsync();

        response.StatusCode
            .Should()
            .Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task Register_WithUsedTicket_ReturnsBadRequest()
    {
        await _postgresFixture.ResetDatabase();

        using var factory = CreateFactory();
        using var client = factory.CreateClient();

        var email = $"used-ticket-{Guid.NewGuid():N}@example.com";
        var phone = $"+7999{Random.Shared.Next(1000000, 9999999)}";
        var sender = factory.Services.GetRequiredService<TestEmailOtpSender>();
        var sendResponse = await SendEmailOtpAsync(
            client,
            email);

        var registrationTicket = await VerifyEmailOtpAsync(
            client,
            sender,
            sendResponse,
            email);

        var request = new
        {
            registrationTicket,
            customerType = "Individual",
            email,
            phone,
            firstName = "Иван",
            lastName = "Иванов"
        };

        var firstResponse = await client.PostAsJsonAsync(
            "/api/auth/register",
            request);

        firstResponse.StatusCode
            .Should()
            .Be(HttpStatusCode.OK);

        var secondResponse = await client.PostAsJsonAsync(
            "/api/auth/register",
            new
            {
                registrationTicket,
                customerType = "Individual",
                email,
                phone = $"+7999{Random.Shared.Next(1000000, 9999999)}",
                firstName = "Пётр",
                lastName = "Петров"
            });

        secondResponse.StatusCode
            .Should()
            .Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task Register_WithValidEmailTicket_ReturnsJwtThatAuthenticatesUser()
    {
        await _postgresFixture.ResetDatabase();

        using var factory = CreateFactory();
        using var client = factory.CreateClient();

        var email = $"auth-me-{Guid.NewGuid():N}@example.com";
        var phone = $"+7999{Random.Shared.Next(1000000, 9999999)}";

        var sender = factory.Services
            .GetRequiredService<TestEmailOtpSender>();

        var sendResponse = await SendEmailOtpAsync(
            client,
            email);

        var registrationTicket = await VerifyEmailOtpAsync(
            client,
            sender,
            sendResponse,
            email);

        var registerResponse =await client.PostAsJsonAsync(
            "/api/auth/register",
            new RegisterRequest(
                RegistrationTicket: registrationTicket,
                CustomerType: CustomerType.Individual,
                Email: email,
                Phone: phone,
                FirstName: "Auth",
                LastName: "Me",
                Inn: null));

        var registerBody = await registerResponse.Content.ReadAsStringAsync();

        registerResponse.StatusCode
            .Should()
            .Be(
                HttpStatusCode.OK,
                $"response body: {registerBody}");

        var registerResult = JsonSerializer.Deserialize<RegisterResponse>(
            registerBody,
            JsonSerializerOptions.Web);

        registerResult.Should().NotBeNull();
        registerResult!.Registered.Should().BeTrue();
        registerResult.Token.Should().NotBeNullOrWhiteSpace();

        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(
            "Bearer",
            registerResult.Token);

        var meResponse = await client.GetAsync("/api/auth/me");

        var meBody = await meResponse.Content.ReadAsStringAsync();

        meResponse.StatusCode
            .Should()
            .Be(HttpStatusCode.OK, $"response body: {meBody}");

        var me = JsonSerializer.Deserialize<CurrentUserDto>(
            meBody,
            JsonSerializerOptions.Web);

        me.Should().NotBeNull();

        me!.Email
            .Should()
            .Be(email);

        me.Phone
            .Should()
            .Be(phone);

        me.FirstName
            .Should()
            .Be("Auth");

        me.LastName
            .Should()
            .Be("Me");

        me.Role
            .Should()
            .Be("user");

        me.CustomerId
            .Should()
            .NotBeNull();
    }

    [Fact]
    public async Task Register_WithEmailContainingWhitespaceAndUppercase_NormalizesEmail()
    {
        await _postgresFixture.ResetDatabase();

        using var factory = CreateFactory();
        using var client = factory.CreateClient();

        var normalizedEmail = $"user-{Guid.NewGuid():N}@example.com";
        var submittedEmail = $"  {normalizedEmail.ToUpperInvariant()}  ";
        var phone = $"+7999{Random.Shared.Next(1000000, 9999999)}";

        var sender = factory.Services
            .GetRequiredService<TestEmailOtpSender>();

        var sendResponse = await SendEmailOtpAsync(
            client,
            normalizedEmail);

        var registrationTicket = await VerifyEmailOtpAsync(
            client,
            sender,
            sendResponse,
            normalizedEmail);

        var registrationResponse = await client.PostAsJsonAsync(
            "/api/auth/register",
            new RegisterRequest(
                RegistrationTicket: registrationTicket,
                CustomerType: CustomerType.Individual,
                Email: submittedEmail,
                Phone: phone,
                FirstName: "Anna",
                LastName: "Volodina",
                Inn: null));

        var responseBody =
            await registrationResponse.Content.ReadAsStringAsync();

        registrationResponse.StatusCode
            .Should()
            .Be(HttpStatusCode.OK, $"response body: {responseBody}");

        await using var db = await CreateDbContextAsync();

        var user = await db.AppUsers
            .SingleAsync();

        user.Email
            .Should()
            .Be(normalizedEmail);

        user.Phone
            .Should()
            .Be(phone);

        var customer = await db.Customers
            .SingleAsync(x => x.Id == user.CustomerId);

        customer.Email
            .Should()
            .Be(normalizedEmail);
    }

    [Fact]
    public async Task Register_WithUnnormalizedPhone_NormalizesPhone()
    {
        await _postgresFixture.ResetDatabase();

        using var factory = CreateFactory();
        using var client = factory.CreateClient();

        var normalizedPhone = $"+7999{Random.Shared.Next(1000000, 9999999)}";

        var submittedPhone =
            $"+7 ({normalizedPhone[2..5]}) " +
            $"{normalizedPhone[5..8]}-" +
            $"{normalizedPhone[8..10]}-" +
            normalizedPhone[10..];

        var sender = factory.Services.GetRequiredService<TestPhoneOtpSender>();

        var sendResponse = await client.PostAsJsonAsync(
            "/api/auth/otp/send",
            new
            {
                channel = "phone",
                destination = submittedPhone
            });

        var sendBody = await sendResponse.Content.ReadFromJsonAsync<SendOtpResponse>();

        sendResponse.StatusCode
            .Should()
            .Be(HttpStatusCode.OK);

        sendBody.Should().NotBeNull();

        sender.LastPhone
            .Should()
            .Be(normalizedPhone);

        sender.LastCode
            .Should()
            .NotBeNullOrWhiteSpace();

        var verifyResponse = await client.PostAsJsonAsync(
            "/api/auth/otp/verify",
            new
            {
                challengeId = sendBody!.ChallengeId,
                channel = "phone",
                destination = submittedPhone,
                code = sender.LastCode
            });

        var verifyBody =
            await verifyResponse.Content.ReadFromJsonAsync<VerifyOtpResponse>(
                new JsonSerializerOptions
                {
                    PropertyNamingPolicy =
                        JsonNamingPolicy.CamelCase,
                    Converters =
                    {
                        new JsonStringEnumConverter()
                    }
                });

        verifyResponse.StatusCode
            .Should()
            .Be(HttpStatusCode.OK);

        verifyBody.Should().NotBeNull();

        verifyBody!.Verified
            .Should()
            .BeTrue();

        verifyBody.Status
            .Should()
            .Be(OtpAuthenticationStatus.RegistrationRequired);

        verifyBody.RegistrationTicket
            .Should()
            .NotBeNullOrWhiteSpace();

        var registrationResponse = await client.PostAsJsonAsync(
            "/api/auth/register",
            new RegisterRequest(
                RegistrationTicket: verifyBody.RegistrationTicket!,
                CustomerType: CustomerType.Individual,
                Email: null,
                Phone: submittedPhone,
                FirstName: "Иван",
                LastName: "Иванов",
                Inn: null));

        var registrationBody = await registrationResponse.Content.ReadAsStringAsync();

        registrationResponse.StatusCode
            .Should()
            .Be(
                HttpStatusCode.OK,
                $"response body: {registrationBody}");

        await using var db = await CreateDbContextAsync();

        var user = await db.AppUsers.SingleAsync();

        user.Phone
            .Should()
            .Be(normalizedPhone);

        user.Email
            .Should()
            .BeNull();

        var customer = await db.Customers.SingleAsync(x => x.Id == user.CustomerId);

        customer.Phone
            .Should()
            .Be(normalizedPhone);

        customer.Email
            .Should()
            .BeNull();
    }

    [Fact]
    public async Task VerifyOtp_WithExistingUserFromPostgres_ReturnsAuthenticatedWithToken()
    {
        await _postgresFixture.ResetDatabase();

        using var factory = CreateFactory();
        using var client = factory.CreateClient();

        var email = $"existing-login-{Guid.NewGuid():N}@example.com";
        var phone = $"+7999{Random.Shared.Next(1000000, 9999999)}";

        await using (var db = await CreateDbContextAsync())
        {
            db.AppUsers.Add(new AppUser
            {
                Id = Guid.NewGuid(),
                Email = email,
                Phone = phone,
                FirstName = "Existing",
                LastName = "User",
                Role = "user",
                CreatedAt = DateTime.UtcNow
            });

            await db.SaveChangesAsync();
        }

        var sender =
            factory.Services.GetRequiredService<TestEmailOtpSender>();

        var sendResponse = await SendEmailOtpAsync(
            client,
            email);

        var verifyResponse = await client.PostAsJsonAsync(
            "/api/auth/otp/verify",
            new
            {
                challengeId = sendResponse.ChallengeId,
                channel = "email",
                destination = email,
                code = sender.LastCode
            });

        var responseBody =
            await verifyResponse.Content.ReadAsStringAsync();

        verifyResponse.StatusCode
            .Should()
            .Be(
                HttpStatusCode.OK,
                $"response body: {responseBody}");

        var result = await verifyResponse.Content
            .ReadFromJsonAsync<VerifyOtpResponse>(
                new JsonSerializerOptions
                {
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                    Converters =
                    {
                        new JsonStringEnumConverter()
                    }
                });

        result.Should().NotBeNull();

        result!.Verified
            .Should()
            .BeTrue();

        result.Status
            .Should()
            .Be(OtpAuthenticationStatus.Authenticated);

        result.Token
            .Should()
            .NotBeNullOrWhiteSpace();

        result.RegistrationTicket
            .Should()
            .BeNull();
    }

    [Fact]
    public async Task Register_WithExistingPhone_ReturnsConflict()
    {
        await _postgresFixture.ResetDatabase();

        using var factory = CreateFactory();
        using var client = factory.CreateClient();

        var existingEmail =
            $"existing-phone-{Guid.NewGuid():N}@example.com";

        var verifiedEmail =
            $"verified-phone-{Guid.NewGuid():N}@example.com";

        var existingPhone =
            $"+7999{Random.Shared.Next(1000000, 9999999)}";

        var sender =
            factory.Services.GetRequiredService<TestEmailOtpSender>();

        // Первый пользователь — с телефоном, который хотим занять.
        var firstSendResponse =
            await SendEmailOtpAsync(
                client,
                existingEmail);

        var firstTicket =
            await VerifyEmailOtpAsync(
                client,
                sender,
                firstSendResponse,
                existingEmail);

        var firstRegisterResponse =
            await client.PostAsJsonAsync(
                "/api/auth/register",
                new RegisterRequest(
                    RegistrationTicket: firstTicket,
                    CustomerType: CustomerType.Individual,
                    Email: existingEmail,
                    Phone: existingPhone,
                    FirstName: "Иван",
                    LastName: "Иванов",
                    Inn: null));

        firstRegisterResponse.StatusCode
            .Should()
            .Be(HttpStatusCode.OK);

        // Новый verified email.
        var secondSendResponse =
            await SendEmailOtpAsync(
                client,
                verifiedEmail);

        var secondTicket =
            await VerifyEmailOtpAsync(
                client,
                sender,
                secondSendResponse,
                verifiedEmail);

        // Тот же телефон, но другой verified email.
        var secondRegisterResponse =
            await client.PostAsJsonAsync(
                "/api/auth/register",
                new RegisterRequest(
                    RegistrationTicket: secondTicket,
                    CustomerType: CustomerType.Individual,
                    Email: verifiedEmail,
                    Phone: existingPhone,
                    FirstName: "Пётр",
                    LastName: "Петров",
                    Inn: null));

        var responseBody =
            await secondRegisterResponse.Content.ReadAsStringAsync();

        secondRegisterResponse.StatusCode
            .Should()
            .Be(
                HttpStatusCode.Conflict,
                $"response body: {responseBody}");

        await using var db = await CreateDbContextAsync();

        (await db.AppUsers.CountAsync())
            .Should()
            .Be(1);
    }

    [Fact]
    public async Task Register_WithContactMismatch_DoesNotConsumeTicket()
    {
        await _postgresFixture.ResetDatabase();

        using var factory = CreateFactory();
        using var client = factory.CreateClient();

        var verifiedEmail =
            $"verified-contact-{Guid.NewGuid():N}@example.com";

        var differentEmail =
            $"different-contact-{Guid.NewGuid():N}@example.com";

        var phone =
            $"+7999{Random.Shared.Next(1000000, 9999999)}";

        var sender =
            factory.Services.GetRequiredService<TestEmailOtpSender>();

        var sendResponse =
            await SendEmailOtpAsync(
                client,
                verifiedEmail);

        var registrationTicket =
            await VerifyEmailOtpAsync(
                client,
                sender,
                sendResponse,
                verifiedEmail);

        // Первая попытка: contact mismatch.
        var mismatchResponse =
            await client.PostAsJsonAsync(
                "/api/auth/register",
                new RegisterRequest(
                    RegistrationTicket: registrationTicket,
                    CustomerType: CustomerType.Individual,
                    Email: differentEmail,
                    Phone: phone,
                    FirstName: "Иван",
                    LastName: "Иванов",
                    Inn: null));

        var mismatchBody =
            await mismatchResponse.Content.ReadAsStringAsync();

        mismatchResponse.StatusCode
            .Should()
            .Be(
                HttpStatusCode.BadRequest,
                $"response body: {mismatchBody}");

        // Второй запрос использует тот же ticket,
        // но уже с подтверждённым email.
        var validResponse =
            await client.PostAsJsonAsync(
                "/api/auth/register",
                new RegisterRequest(
                    RegistrationTicket: registrationTicket,
                    CustomerType: CustomerType.Individual,
                    Email: verifiedEmail,
                    Phone: phone,
                    FirstName: "Иван",
                    LastName: "Иванов",
                    Inn: null));

        var validBody =
            await validResponse.Content.ReadAsStringAsync();

        validResponse.StatusCode
            .Should()
            .Be(
                HttpStatusCode.OK,
                $"response body: {validBody}");

        await using var db =
            await CreateDbContextAsync();

        (await db.AppUsers.CountAsync())
            .Should()
            .Be(1);
    }
    private async Task<RentFlowDbContext> CreateDbContextAsync()
    {
        var options =
            new Microsoft.EntityFrameworkCore.DbContextOptionsBuilder<RentFlowDbContext>()
                .UseNpgsql(_postgresFixture.ConnectionString)
                .Options;

        return new RentFlowDbContext(options);
    }
}