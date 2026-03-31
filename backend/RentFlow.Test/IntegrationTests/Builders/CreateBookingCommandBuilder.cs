using System;
using RentFlow.Application.Bookings.Commands;

namespace RentFlow.Test.IntegrationTests.Builders;

public class CreateBookingCommandBuilder
{
    private Guid _customerId = Guid.NewGuid();
    private Guid _assetId = Guid.NewGuid();
    private DriverBuilder? _driverBuilder;
    private DateTime _start = DateTime.UtcNow.AddDays(1);
    private DateTime _end = DateTime.UtcNow.AddDays(3);

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

    public CreateBookingCommandBuilder WithPeriod(DateTime start, DateTime end)
    {
        _start = start;
        _end = end;
        return this;
    }

    public CreateBookingCommand Build()
    {
        return new CreateBookingCommand
        (
            _assetId,
            _customerId,
            _start,
            _end,
            _driverBuilder?.Build()
        );
    }
}
