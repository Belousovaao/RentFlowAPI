using FluentValidation;
using RentFlow.Application.Bookings.Commands;

namespace RentFlow.Application.Bookings.Validators;

public class CreateBookingCommandValidator : AbstractValidator<CreateBookingCommand>
{
    public CreateBookingCommandValidator()
    {
        RuleFor(x => x.AssetId).NotEmpty();

        RuleFor(x => x.CustomerId).NotEmpty();

        RuleFor(x => x.PickupDate)
            .Must(date =>
            {
                var today = DateOnly.FromDateTime(DateTime.Today);

                Console.WriteLine($"Request pickup = {date}");
                Console.WriteLine($"Today          = {today}");
                Console.WriteLine($"Compare        = {date >= today}");

                return date >= today;
            })
            .WithMessage("Pickup date cannot be in the past.");

        RuleForEach(x => x.Services)
            .ChildRules(service =>
            {
                service.RuleFor(x => x.AdditionalServiceId)
                    .NotEmpty();
            });
    }
}