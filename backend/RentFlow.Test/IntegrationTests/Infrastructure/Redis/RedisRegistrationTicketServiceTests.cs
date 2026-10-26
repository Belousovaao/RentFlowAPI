using FluentAssertions;
using Microsoft.Extensions.Options;
using RentFlow.Application.Auth.Otp;
using RentFlow.Application.Auth.Registration;
using RentFlow.Infrastructure.Redis;

namespace RentFlow.Test.IntegrationTests.Infrastructure.Redis;

public sealed class RedisRegistrationTicketServiceTests : IClassFixture<RedisContainerFixture>
{
    private readonly RedisContainerFixture _fixture;

    public RedisRegistrationTicketServiceTests(RedisContainerFixture fixture)
    {
        _fixture = fixture;
    }

    private RedisRegistrationTicketService CreateService(int ticketTtlSeconds = 600)
    {
        var options = Options.Create(new RegistrationOptions
        {
            TicketTtlSeconds = ticketTtlSeconds
        });

        return new RedisRegistrationTicketService(
            _fixture.Connection,
            options);
    }

    [Fact]
    public async Task CreateAsync_ReturnsNonEmptyTicket()
    {
        var service = CreateService();
        var ticket = await service.CreateAsync(
            OtpChannel.Email,
            "destination-hash");

        ticket
            .Should()
            .NotBeNullOrWhiteSpace();
    }

    [Fact]
    public async Task CreateAsync_CreatesUniqueTickets()
    {
        var service = CreateService();
        var first = await service.CreateAsync(
            OtpChannel.Email,
            "destination-hash");
        var second = await service.CreateAsync(
            OtpChannel.Email,
            "destination-hash");
        second
            .Should()
            .NotBe(first);
    }

    [Fact]
    public async Task ConsumeAsync_ReturnsTicketWithCorrectData()
    {
        var service = CreateService();
        var ticket = await service.CreateAsync(
            OtpChannel.Email,
            "destination-hash");
        var result = await service.ConsumeAsync(ticket);

        result.Should().NotBeNull();
        result!.Ticket
            .Should()
            .Be(ticket);
        result.Channel
            .Should()
            .Be(OtpChannel.Email);
        result.DestinationHash
            .Should()
            .Be("destination-hash");
    }

    [Fact]
    public async Task ConsumeAsync_WithPhoneTicket_ReturnsCorrectChannel()
    {
        var service = CreateService();
        var ticket = await service.CreateAsync(
            OtpChannel.Phone,
            "phone-destination-hash");
        var result = await service.ConsumeAsync(ticket);
        result.Should().NotBeNull();
        result!.Channel
            .Should()
            .Be(OtpChannel.Phone);
        result.DestinationHash
            .Should()
            .Be("phone-destination-hash");
    }

    [Fact]
    public async Task ConsumeAsync_IsOneTime()
    {
        var service = CreateService();
        var ticket = await service.CreateAsync(
            OtpChannel.Email,
            "destination-hash");
        var first = await service.ConsumeAsync(ticket);
        var second = await service.ConsumeAsync(ticket);
        first.Should().NotBeNull();
        second.Should().BeNull();
    }

    [Fact]
    public async Task ConsumeAsync_WithUnknownTicket_ReturnsNull()
    {
        var service = CreateService();
        var result = await service.ConsumeAsync(
            "unknown-registration-ticket");
        result.Should().BeNull();
    }

    [Fact]
    public async Task ConsumeAsync_AfterExpiration_ReturnsNull()
    {
        var service = CreateService(ticketTtlSeconds: 1);
        var ticket = await service.CreateAsync(
            OtpChannel.Email,
            "destination-hash");
        await Task.Delay(TimeSpan.FromSeconds(2));
        var result = await service.ConsumeAsync(ticket);
        result.Should().BeNull();
    }

    [Fact]
    public async Task ConsumeAsync_Concurrently_AllowsOnlyOneConsumer()
    {
        var service = CreateService();
        var ticket = await service.CreateAsync(
            OtpChannel.Email,
            "destination-hash");
        var tasks = Enumerable
            .Range(0, 20)
            .Select(_ => service.ConsumeAsync(ticket))
            .ToArray();
        var results = await Task.WhenAll(tasks);
        results.Count(x => x is not null)
            .Should()
            .Be(1);
        results.Count(x => x is null)
            .Should()
            .Be(19);
    }

    [Fact]
    public async Task CreateAsync_WithEmptyDestinationHash_Throws()
    {
        var service = CreateService();
        var action = () => service.CreateAsync(
            OtpChannel.Email,
            "");
        await action.Should().ThrowAsync<ArgumentException>();
    }

    [Fact]
    public async Task CreateAsync_WithInvalidChannel_Throws()
    {
        var service = CreateService();
        var action = () => service.CreateAsync(
            (OtpChannel)999,
            "destination-hash");

        await action.Should().ThrowAsync<ArgumentOutOfRangeException>();
    }

    [Fact]
    public async Task GetAsync_WithExistingTicket_ReturnsTicket()
    {
        const string destinationHash = "destination-hash";
        var service = CreateService();
        var ticket = await service.CreateAsync(
            OtpChannel.Email,
            destinationHash);

        var result = await service.GetAsync(ticket);

        result.Should().NotBeNull();
        result!.Ticket.Should().Be(ticket);
        result.Channel.Should().Be(OtpChannel.Email);
        result.DestinationHash.Should().Be(destinationHash);
    }

    [Fact]
    public async Task GetAsync_DoesNotConsumeTicket()
    {
        const string destinationHash = "destination-hash";
        var service = CreateService();
        var ticket = await service.CreateAsync(
            OtpChannel.Email,
            destinationHash);

        var result = await service.GetAsync(ticket);

        result.Should().NotBeNull();

        var consumed = await service.ConsumeAsync(ticket);

        consumed.Should().NotBeNull();
        consumed!.Ticket.Should().Be(ticket);
        consumed.Channel.Should().Be(OtpChannel.Email);
        consumed.DestinationHash.Should().Be(destinationHash);
    }

    [Fact]
    public async Task GetAsync_CanBeCalledMultipleTimes()
    {
        const string destinationHash = "destination-hash";
        var service = CreateService();
        var ticket = await service.CreateAsync(
            OtpChannel.Phone,
            destinationHash);

        var first = await service.GetAsync(ticket);
        var second = await service.GetAsync(ticket);

        first.Should().NotBeNull();
        second.Should().NotBeNull();

        second!.Ticket.Should().Be(first!.Ticket);
        second.Channel.Should().Be(first.Channel);
        second.DestinationHash.Should().Be(first.DestinationHash);
    }

    [Fact]
    public async Task GetAsync_WithUnknownTicket_ReturnsNull()
    {
        var service = CreateService();
        var result = await service.GetAsync("unknown-ticket");

        result.Should().BeNull();
    }

    [Fact]
    public async Task GetAsync_AfterConsume_ReturnsNull()
    {
        const string destinationHash = "destination-hash";
        var service = CreateService();
        var ticket = await service.CreateAsync(
            OtpChannel.Email,
            destinationHash);

        var consumed = await service.ConsumeAsync(ticket);

        consumed.Should().NotBeNull();

        var result = await service.GetAsync(ticket);

        result.Should().BeNull();
    }
}