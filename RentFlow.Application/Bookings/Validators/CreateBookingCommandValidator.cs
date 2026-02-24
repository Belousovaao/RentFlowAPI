using System;
using System.Data;
using FluentValidation;
using RentFlow.Application.Bookings.Commands;

namespace RentFlow.Application.Bookings.Validators;

public class CreateBookingCommandValidator : AbstractValidator<CreateBookingCommand>
{
    public CreateBookingCommandValidator()
    {
        RuleFor(x => x.AssetId).NotEmpty();
        RuleFor(x => x.CustomerId).NotEmpty();
        RuleFor(x => x.DriverPersonId).NotEmpty();
        RuleFor(x => x.StartDate).GreaterThanOrEqualTo(DateTime.UtcNow.Date).WithMessage("Start date cannot be in the past");
        RuleFor(x => x.EndDate).GreaterThan(x => x.StartDate).WithMessage("End date cannot be earlier or equal to startdate");
    }
}
