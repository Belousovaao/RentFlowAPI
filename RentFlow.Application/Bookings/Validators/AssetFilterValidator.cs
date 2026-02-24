using System;
using System.IO.Compression;
using System.Security.Cryptography.X509Certificates;
using FluentValidation;
using RentFlow.Application.Assets.Queries;

namespace RentFlow.Application.Bookings.Validators;

public class AssetFilterValidator : AbstractValidator<AssetFilter>
{
    public AssetFilterValidator()
    {
        RuleFor(x => x.Type).NotEmpty();
        RuleFor(x => x.PriceFrom).GreaterThan(0).When(x => x.PriceFrom.HasValue);
        RuleFor(x => x).Must(x => !x.PriceFrom.HasValue || !x.PriceTo.HasValue || x.PriceFrom >= x.PriceTo).WithMessage("Maxumim price must be greater or equal to minimum price");
        RuleFor(x => x.Status).NotEmpty();
    }
}
