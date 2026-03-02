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

namespace RentFlow.Test;

public class RentalPeriodTests
{

    [Fact]
    public async Task Should_Throw_When_End_Is_Equal_To_Start()
    {   
        Action act = () => new RentalPeriod(DateTime.UtcNow, DateTime.UtcNow);

        act.Should().Throw<InvalidRentalPeriodException>();
    }

    [Fact]
    public async Task Should_Frow_When_End_Is_Before_Start()
    {
        Action act = () => new RentalPeriod(DateTime.UtcNow, DateTime.UtcNow.AddHours(-1));

        act.Should().Throw<InvalidRentalPeriodException>();
    }

}
