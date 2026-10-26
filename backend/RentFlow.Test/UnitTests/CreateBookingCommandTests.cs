
using FluentAssertions;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using RentFlow.Application.Bookings.Commands;
using RentFlow.Application.Bookings.Dtos;
using RentFlow.Application.Bookings.Handlers;
using RentFlow.Application.Bookings.Services;
using RentFlow.Application.Interfaces;
using RentFlow.Domain.Assets;
using RentFlow.Domain.Bookings;
using RentFlow.Domain.Bookings.Pricing;
using RentFlow.Domain.Bookings.Services;
using RentFlow.Domain.Bookings.ValueObjects;
using RentFlow.Domain.Common;
using RentFlow.Domain.Customers;
using RentFlow.Test.IntegrationTests.Builders;

namespace RentFlow.Test;

public class CreateBookingCommandTests
{
    private readonly Mock<IAssetRepository> _assetRepo = new();
    private readonly Mock<ICustomerRepository> _customerRepo = new();
    private readonly Mock<IBookingRepository> _bookingRepo = new();
    private readonly Mock<IAdditionalServiceRepository> _additionalServiceRepo = new();
    private readonly Mock<IUnitOfWork> _uow = new();
    private readonly Mock<IDateTimeProvider> _dateTimeProvider = new();
    private readonly Mock<IBookingPricingService> _pricingService = new();
    private readonly Mock<IBookingDestinationResolver> _destinationResolver = new();

    private readonly CreateBookingHandler _handler;

    public CreateBookingCommandTests()
    {
        _handler = new CreateBookingHandler(
            _uow.Object,
            _bookingRepo.Object,
            _assetRepo.Object,
            _customerRepo.Object,
            _dateTimeProvider.Object,
            _pricingService.Object,
            _additionalServiceRepo.Object,
            NullLogger<CreateBookingHandler>.Instance,
            _destinationResolver.Object);
    }

    private static Customer CreateCustomer()
    {
        return Customer.CreateIndividual(
            email: "vasyapupkin@mail.ru",
            phone: "+7 (800) 0000-00-00",
            name: new PersonName("Vasya", "Pupkin", "Nikitich"),
            dateOfBirth: new DateOnly(1992, 12, 2),
            passport: new Passport(
                "1111",
                "222222",
                "Otdelom UFMS Rossii",
                new DateOnly(2000, 1, 1),
                "Moscow, Tsvetochnaya st., 555 apt."),
            driverLicense: new DriverLicense(
                "11 22 333333",
                new List<string> { "A", "B1" },
                new DateOnly(2001, 2, 2),
                new DateOnly(2030, 12, 31),
                2000),
            bankAccount: new BankAccount(
                "11111111111111111",
                "222222222222222222222222",
                "09454950",
                "VTB-Bank"));
    }

    private static RentalPrice CreateRentalPrice(int days = 2)
    {
        var dailyPrice = 500m;

        return new RentalPrice(
            new RentalPriceSegment(
                days: days,
                baseDailyPrice: dailyPrice,
                durationDiscountPercent: 0m,
                discountedDailyPrice: dailyPrice,
                baseRentalPrice: dailyPrice * days,
                discountAmount: 0m,
                total: dailyPrice * days,
                appliedForDays: days,
                order: 0));
    }

    private static CreateBookingCommand CreateCommand(
        Guid assetId,
        Guid customerId,
        DateOnly? pickupDate = null,
        DateOnly? returnDate = null)
    {
        return new CreateBookingCommand(
            AssetId: assetId,
            CustomerId: customerId,
            PickupDate: pickupDate ?? DateOnly.FromDateTime(DateTime.UtcNow.AddDays(1)),
            PickupTime: null,
            ReturnDate: returnDate ?? DateOnly.FromDateTime(DateTime.UtcNow.AddDays(3)),
            ReturnTime: null,
            Driver: null,
            Services: new List<BookingServiceRequestDto>(),
            DestinationType: DestinationType.Local,
            DestinationId: null,
            CustomRouteDescription: null);
    }

    private void SetupLocalDestination(decimal surcharge = 0m)
    {
        _destinationResolver
            .Setup(x => x.ResolveAsync(
                DestinationType.Local,
                null,
                null,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(
                new ResolvedBookingDestination(
                    BookingDestination.Local(),
                    surcharge));
    }

    private void SetupDateTimeProvider()
    {
        _dateTimeProvider
            .Setup(x => x.ToUtc(
                It.IsAny<DateOnly>(),
                It.IsAny<TimeOnly?>()))
            .Returns<DateOnly, TimeOnly?>((date, time) =>
                DateTime.SpecifyKind(
                    date.ToDateTime(time ?? new TimeOnly(12, 0)),
                    DateTimeKind.Utc));
    }

    private void SetupPricingService(RentalPrice? rentalPrice = null)
    {
        rentalPrice ??= CreateRentalPrice();

        _pricingService
            .Setup(x => x.Calculate(
                It.IsAny<Asset>(),
                It.IsAny<RentalPeriod>(),
                It.IsAny<IReadOnlyCollection<SelectedAdditionalService>>(),
                It.IsAny<decimal>()))
            .Returns((
                Asset asset,
                RentalPeriod period,
                IReadOnlyCollection<SelectedAdditionalService> services,
                decimal routeSurcharge) =>
                new BookingCalculationResult(
                    new BookingPrice(
                        rentalPrice: rentalPrice,
                        deposit: asset.Deposit ?? 5000m,
                        servicesPrice: 0m,
                        routeSurcharge: routeSurcharge),
                    null));
    }

    private void SetupValidBooking(
        Asset asset,
        Customer customer)
    {
        _assetRepo
            .Setup(x => x.GetByIdAsync(
                asset.Id,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(asset);

        _customerRepo
            .Setup(x => x.GetByIdAsync(
                customer.Id,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(customer);

        _bookingRepo
            .Setup(x => x.GetOverlappingAsync(
                asset.Id,
                It.IsAny<RentalPeriod>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Booking>());

        _additionalServiceRepo
            .Setup(x => x.GetByIdsAsync(
                It.IsAny<IEnumerable<Guid>>(),
                It.IsAny<CancellationToken>()))
            .Returns(Task.FromResult(new List<AdditionalService>()));

        SetupDateTimeProvider();
        SetupLocalDestination();
        SetupPricingService();
    }

    [Fact]
    public async Task Should_Create_Booking_If_Data_Is_Valid()
    {
        // Arrange
        Asset asset = new AssetBuilder().Build();
        Customer customer = CreateCustomer();

        SetupValidBooking(asset, customer);

        var command = CreateCommand(asset.Id, customer.Id);

        // Act
        Booking result = await _handler.Handle(
            command,
            CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.AssetId.Should().Be(asset.Id);
        result.CustomerId.Should().Be(customer.Id);
        result.Destination.Type.Should().Be(DestinationType.Local);

        _bookingRepo.Verify(
            x => x.AddAsync(
                It.IsAny<Booking>(),
                It.IsAny<CancellationToken>()),
            Times.Once);

        _uow.Verify(
            x => x.SaveChangesAsync(It.IsAny<CancellationToken>()),
            Times.Once);

        _uow.Verify(
            x => x.CommitAsync(It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task Should_Pass_Route_Surcharge_To_Pricing_Service()
    {
        // Arrange
        Asset asset = new AssetBuilder().Build();
        Customer customer = CreateCustomer();

        const decimal surcharge = 2500m;

        SetupValidBooking(asset, customer);
        SetupLocalDestination(surcharge);
        SetupPricingService();

        var command = CreateCommand(asset.Id, customer.Id);

        // Act
        Booking result = await _handler.Handle(
            command,
            CancellationToken.None);

        // Assert
        _pricingService.Verify(
            x => x.Calculate(
                asset,
                It.IsAny<RentalPeriod>(),
                It.IsAny<IReadOnlyCollection<SelectedAdditionalService>>(),
                surcharge),
            Times.Once);

        result.Price.RouteSurcharge.Should().Be(surcharge);
    }

    [Fact]
    public async Task Should_Throw_When_Asset_Not_Found()
    {
        // Arrange
        _assetRepo
            .Setup(x => x.GetByIdAsync(
                It.IsAny<Guid>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync((Asset?)null);

        var command = CreateCommand(
            Guid.NewGuid(),
            Guid.NewGuid());

        // Act
        Func<Task> act = () => _handler.Handle(
            command,
            CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<AssetNotFoundException>();

        _bookingRepo.Verify(
            x => x.AddAsync(
                It.IsAny<Booking>(),
                It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [Fact]
    public async Task Should_Throw_When_Customer_Not_Found()
    {
        // Arrange
        Asset asset = new AssetBuilder().Build();

        _assetRepo
            .Setup(x => x.GetByIdAsync(
                asset.Id,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(asset);

        _customerRepo
            .Setup(x => x.GetByIdAsync(
                It.IsAny<Guid>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync((Customer?)null);

        var command = CreateCommand(
            asset.Id,
            Guid.NewGuid());

        // Act
        Func<Task> act = () => _handler.Handle(
            command,
            CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<CustomerNotFoundException>();

        _bookingRepo.Verify(
            x => x.AddAsync(
                It.IsAny<Booking>(),
                It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [Fact]
    public async Task Should_Throw_When_Overlapping_Exists()
    {
        // Arrange
        Asset asset = new AssetBuilder().Build();
        Customer customer = CreateCustomer();

        SetupValidBooking(asset, customer);

        RentalPrice rentalPrice = CreateRentalPrice(days: 3);

        Booking existingBooking = Booking.Create(
            asset,
            customer,
            null,
            new RentalPeriod(
                DateTime.UtcNow,
                DateTime.UtcNow.AddDays(3)),
            new BookingPrice(
                rentalPrice: rentalPrice,
                deposit: asset.Deposit ?? 0m,
                servicesPrice: 0m),
            BookingDestination.Local());

        _bookingRepo
            .Setup(x => x.GetOverlappingAsync(
                asset.Id,
                It.IsAny<RentalPeriod>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Booking> { existingBooking });

        var command = CreateCommand(
            asset.Id,
            customer.Id,
            DateOnly.FromDateTime(DateTime.UtcNow.AddDays(1)),
            DateOnly.FromDateTime(DateTime.UtcNow.AddDays(3)));

        // Act
        Func<Task> act = () => _handler.Handle(
            command,
            CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<BookingConflictException>();

        _bookingRepo.Verify(
            x => x.AddAsync(
                It.IsAny<Booking>(),
                It.IsAny<CancellationToken>()),
            Times.Never);

        _uow.Verify(
            x => x.SaveChangesAsync(It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [Fact]
    public async Task Should_Throw_When_RentalPeriod_Is_Invalid()
    {
        // Arrange
        Asset asset = new AssetBuilder().Build();
        Customer customer = CreateCustomer();

        _assetRepo
            .Setup(x => x.GetByIdAsync(
                asset.Id,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(asset);

        _customerRepo
            .Setup(x => x.GetByIdAsync(
                customer.Id,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(customer);

        SetupDateTimeProvider();
        SetupLocalDestination();

        var date = DateOnly.FromDateTime(DateTime.UtcNow);

        var command = CreateCommand(
            asset.Id,
            customer.Id,
            date,
            date);

        // Act
        Func<Task> act = () => _handler.Handle(
            command,
            CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<InvalidRentalPeriodException>();

        _bookingRepo.Verify(
            x => x.AddAsync(
                It.IsAny<Booking>(),
                It.IsAny<CancellationToken>()),
            Times.Never);

        _uow.Verify(
            x => x.SaveChangesAsync(It.IsAny<CancellationToken>()),
            Times.Never);

        _uow.Verify(
            x => x.CommitAsync(It.IsAny<CancellationToken>()),
            Times.Never);
    }
}
