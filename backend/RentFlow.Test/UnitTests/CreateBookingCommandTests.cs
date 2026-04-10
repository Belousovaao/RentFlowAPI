using Moq;
using RentFlow.Application.Bookings.Handlers;
using RentFlow.Application.Interfaces;
using Microsoft.Extensions.Logging.Abstractions;
using RentFlow.Domain.Assets;
using RentFlow.Domain.Customers;
using RentFlow.Application.Bookings.Commands;
using RentFlow.Domain.Bookings;
using FluentAssertions;
using RentFlow.Domain.Common;
using RentFlow.Test.IntegrationTests.Builders;

namespace RentFlow.Test;

public class CreateBookingCommandTests
{
    private readonly Mock<IAssetRepository> _assetRepo = new();
    private readonly Mock<ICustomerRepository> _customerRepo = new();
    private readonly Mock<IBookingRepository> _bookingRepo = new();
    private readonly Mock<IUnitOfWork> _uow = new();
    private readonly CreateBookingHandler _handler;
    NullLogger<CreateBookingHandler> logger = NullLogger<CreateBookingHandler>.Instance;

    public CreateBookingCommandTests()
    {
        _handler = new CreateBookingHandler(
            _uow.Object,
            _bookingRepo.Object,
            _assetRepo.Object,
            _customerRepo.Object,
            logger
        );
    }

    [Fact]
    public async Task Should_Create_Booing_If_Data_Is_Valid()
    {
        Asset asset = new AssetBuilder().Build();

        asset.AddPhoto("https://freeimage.host/i/fHyQffe");

        Customer customer = Customer.CreateIndividual(
            email: "vasyapupkin@mail.ru",
            phone: "+7 (800) 0000-00-00",
            name: new PersonName("Vasya", "Pupkin", "Nikitich"),
            dateOfBirth: new DateOnly(1992, 12, 2),
            passport: new Passport("1111", "222222", "Otdelom UFMS Rossii", new DateOnly(2000,1,1), "Moscow, Tsvetochanaya st., 555 apt."),
            driverLicense: new DriverLicense("11 22 333333", new List<char>{'A', 'B'}, new DateOnly(2001, 2, 2), new DateOnly(2030, 12, 31)), 
            bankAccount: new BankAccount("11111111111111111", "222222222222222222222222", "09454950", "VTB-Bank"));

        _assetRepo.Setup(x => x.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>())).ReturnsAsync(asset);
        _customerRepo.Setup(x => x.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>())).ReturnsAsync(customer);
        _bookingRepo.Setup(x => x.GetOverlappingAsync(
            It.IsAny<Guid>(),
            It.IsAny<RentalPeriod>(),
            It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Booking>());

        CreateBookingCommand command = new CreateBookingCommand(asset.Id, customer.Id, DateTime.UtcNow.AddDays(1), DateTime.UtcNow.AddDays(3), null);

        var result = await _handler.Handle(command, CancellationToken.None);

        _bookingRepo.Verify(x => x.AddAsync(It.IsAny<Booking>(),It.IsAny<CancellationToken>()));
        _uow.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()));
    }

    [Fact]
    public async Task Should_Throw_When_Asset_Not_Found()
    {
        _assetRepo.Setup(x => x.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Asset?)null);

        var command = new CreateBookingCommand(Guid.NewGuid(), Guid.NewGuid(), DateTime.UtcNow.AddDays(1), DateTime.UtcNow.AddDays(3), null);

        Func<Task> act = async () => await _handler.Handle(command, CancellationToken.None);

        await act.Should().ThrowAsync<AssetNotFoundException>();
    }

    [Fact]
    public async Task Should_Throw_When_Customer_Not_Found()
    {
        Asset asset = new AssetBuilder().Build();
        _assetRepo.Setup(x => x.GetByIdAsync(asset.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(asset);
        _customerRepo.Setup(x => x.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Customer?)null);

        var command = new CreateBookingCommand(asset.Id, Guid.NewGuid(), DateTime.UtcNow.AddDays(1), DateTime.UtcNow.AddDays(3), null);

        Func<Task> act = async () => await _handler.Handle(command, CancellationToken.None);

        await act.Should().ThrowAsync<CustomerNotFoundException>();
    }

    [Fact]
    public async Task Should_Throw_When_Overlapping_Exists()
    {
        Asset asset = new AssetBuilder().Build();

        _assetRepo.Setup(x => x.GetByIdAsync(asset.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(asset);

        Customer customer = Customer.CreateIndividual(
            email: "vasyapupkin@mail.ru",
            phone: "+7 (800) 0000-00-00",
            name: new PersonName("Vasya", "Pupkin", "Nikitich"),
            dateOfBirth: new DateOnly(1992, 12, 2),
            passport: new Passport("1111", "222222", "Otdelom UFMS Rossii", new DateOnly(2000,1,1), "Moscow, Tsvetochanaya st., 555 apt."),
            driverLicense: new DriverLicense("11 22 333333", new List<char>{'A', 'B'}, new DateOnly(2001, 2, 2), new DateOnly(2030, 12, 31)), 
            bankAccount: new BankAccount("11111111111111111", "222222222222222222222222", "09454950", "VTB-Bank"));

        _customerRepo.Setup(x => x.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>())).ReturnsAsync(customer);

        _bookingRepo.Setup(x => x.GetOverlappingAsync(
            It.IsAny<Guid>(),
            It.IsAny<RentalPeriod>(),
            It.IsAny<CancellationToken>()))
                .ReturnsAsync(new List<Booking> {Booking.Create(
                    asset, customer, null, new RentalPeriod(DateTime.UtcNow, DateTime.UtcNow.AddDays(3))
                )});

        var command = new CreateBookingCommand(asset.Id, customer.Id, DateTime.UtcNow, DateTime.UtcNow.AddDays(3), null);

        Func<Task> act = () => _handler.Handle(command, CancellationToken.None);

        await act.Should().ThrowAsync<BookingConflictException>();
    }

    [Fact]
    public void Should_Throw_When_RentalPeriod_Is_Invalid()
    {
        Asset asset = new AssetBuilder().Build();

        asset.AddPhoto("https://freeimage.host/i/fHyQffe");

        Customer customer = Customer.CreateIndividual(
            email: "vasyapupkin@mail.ru",
            phone: "+7 (800) 0000-00-00",
            name: new PersonName("Vasya", "Pupkin", "Nikitich"),
            dateOfBirth: new DateOnly(1992, 12, 2),
            passport: new Passport("1111", "222222", "Otdelom UFMS Rossii", new DateOnly(2000,1,1), "Moscow, Tsvetochanaya st., 555 apt."),
            driverLicense: new DriverLicense("11 22 333333", new List<char>{'A', 'B'}, new DateOnly(2001, 2, 2), new DateOnly(2030, 12, 31)), 
            bankAccount: new BankAccount("11111111111111111", "222222222222222222222222", "09454950", "VTB-Bank"));

        _assetRepo.Setup(x => x.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>())).ReturnsAsync(asset);
        _customerRepo.Setup(x => x.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>())).ReturnsAsync(customer);

        var command = new CreateBookingCommand(asset.Id, customer.Id, DateTime.Now, DateTime.Now, null);

        Func<Task> act = () =>  _handler.Handle(command, CancellationToken.None);
        act.Should().ThrowAsync<InvalidRentalPeriodException>();

        _bookingRepo.Verify(x => x.AddAsync(It.IsAny<Booking>(), It.IsAny<CancellationToken>()), Times.Never);
        _uow.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
        _uow.Verify(x => x.CommitAsync(It.IsAny<CancellationToken>()), Times.Never);
        _uow.Verify(x => x.RollbackAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

}
