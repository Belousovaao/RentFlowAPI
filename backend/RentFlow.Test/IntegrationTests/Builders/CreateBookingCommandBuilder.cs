using System;
using System.Collections.Generic;
using RentFlow.Application.Bookings.Commands;
using RentFlow.Application.Bookings.Dtos;
using RentFlow.Domain.Bookings.ValueObjects;

namespace RentFlow.Test.IntegrationTests.Builders;

public class CreateBookingCommandBuilder
{
    private Guid _customerId = Guid.NewGuid();
    private Guid _assetId = Guid.NewGuid();
    private DriverBuilder? _driverBuilder;

    private DateOnly _pickupDate;
    private TimeOnly? _pickupTime;
    private DateOnly _returnDate;
    private TimeOnly? _returnTime;

    private DestinationType _destinationType = DestinationType.Local;
    private Guid? _destinationId;
    private string? _customRouteDescription;

    public CreateBookingCommandBuilder WithCustomer(Guid id)
    {
        _customerId = id;
        return this;
    }

    public CreateBookingCommandBuilder WithAsset(Guid id)
    {
        _assetId = id;
        return this;
    }

    public CreateBookingCommandBuilder WithDriver()
    {
        _driverBuilder = new DriverBuilder();
        return this;
    }

    public CreateBookingCommandBuilder WithDriver(Action<DriverBuilder> configure)
    {
        _driverBuilder = new DriverBuilder();
        configure(_driverBuilder);
        return this;
    }

    public CreateBookingCommandBuilder WithPeriod(
        DateOnly pickupDate,
        TimeOnly? pickupTime,
        DateOnly returnDate,
        TimeOnly? returnTime)
    {
        _pickupDate = pickupDate;
        _pickupTime = pickupTime;
        _returnDate = returnDate;
        _returnTime = returnTime;

        return this;
    }

    public CreateBookingCommandBuilder WithLocalDestination()
    {
        _destinationType = DestinationType.Local;
        _destinationId = null;
        _customRouteDescription = null;

        return this;
    }

    public CreateBookingCommandBuilder WithCatalogDestination(
        Guid destinationId,
        DestinationType type)
    {
        if (type is not DestinationType.Regional
            and not DestinationType.LongDistance)
        {
            throw new ArgumentException(
                "Catalog destination must be Regional or LongDistance.",
                nameof(type));
        }

        _destinationType = type;
        _destinationId = destinationId;
        _customRouteDescription = null;

        return this;
    }

    public CreateBookingCommandBuilder WithCustomRoute(string description)
    {
        _destinationType = DestinationType.Other;
        _destinationId = null;
        _customRouteDescription = description;

        return this;
    }

    public CreateBookingCommand Build()
    {
        return new CreateBookingCommand(
            _assetId,
            _customerId,
            _pickupDate,
            _pickupTime,
            _returnDate,
            _returnTime,
            _driverBuilder?.Build(),
            new List<BookingServiceRequestDto>(),
            _destinationType,
            _destinationId,
            _customRouteDescription);
    }
}