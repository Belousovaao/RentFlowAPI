using RentFlow.Domain.Bookings;
using FluentAssertions;
using RentFlow.Domain.Common;

namespace RentFlow.Test;

public class RentalPeriodTests
{

    [Fact]
    public async Task Should_Throw_When_End_Is_Equal_To_Start()
    {  
        DateTime now = DateTime.UtcNow; 
        Action act = () => new RentalPeriod(now, now);

        act.Should().Throw<InvalidRentalPeriodException>();
    }

    [Fact]
    public async Task Should_Frow_When_End_Is_Before_Start()
    {
        Action act = () => new RentalPeriod(DateTime.UtcNow, DateTime.UtcNow.AddHours(-1));

        act.Should().Throw<InvalidRentalPeriodException>();
    }

}
