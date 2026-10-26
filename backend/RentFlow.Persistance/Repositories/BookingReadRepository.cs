using Microsoft.EntityFrameworkCore;
using RentFlow.Application.Assets.Dtos;
using RentFlow.Application.Bookings.Dtos;
using RentFlow.Application.Customers.Dtos;
using RentFlow.Application.Interfaces;
using RentFlow.Domain.Assets;
using RentFlow.Domain.Bookings;
using RentFlow.Domain.Bookings.ValueObjects;
using RentFlow.Domain.Customers;
using RentFlow.Persistance;

namespace RentFlow.Infrastructure.Persistence.Repositories;

public sealed class BookingReadRepository : IBookingReadRepository
{
    private readonly RentFlowDbContext _db;
    private readonly IFileStorage _fileStorage;

    public BookingReadRepository(RentFlowDbContext db, IFileStorage fileStorage)
    {
        _db = db;
        _fileStorage = fileStorage;
    }

    private static BookingListItemDto MapBookingListItem(Booking booking, string? coverPhotoUrl = null)
    {
        return new BookingListItemDto(
            booking.Id,
            new AssetShortDto(
                booking.AssetId,
                booking.AssetSnapshot.BrandName,
                booking.AssetSnapshot.Model,
                booking.AssetSnapshot.Year,
                coverPhotoUrl),
            booking.RentalPeriod,
            new BookingPriceDto(
                MapRentalPrice(booking.Price.RentalPrice),
                booking.Price.Deposit,
                booking.Price.ServicesPrice,
                booking.Price.RouteSurcharge,
                booking.Price.Total,
                booking.Price.TotalToPay,
                booking.Price.Penalties.Select(x => new BookingPenaltyDto(
                    x.Id,
                    x.Type,
                    x.Amount)).ToList(),
                booking.Price.PenaltiesPrice
                ),
            booking.Status,
            booking.Services.Select(s => new BookingServiceDto(
                s.AdditionalServiceId,
                s.Type,
                s.Name,
                s.Price,
                s.SelectedTime)).ToList()
        );
    }

    private static RentalPriceDto MapRentalPrice(RentalPrice price)
    {
        return new RentalPriceDto(
            price.Segments
                .OrderBy(x => x.AppliedForDays)
                .Select(x => new RentalPriceSegmentDto(
                    x.Days,
                    x.BaseDailyPrice,
                    x.DurationDiscountPercent,
                    x.DiscountedDailyPrice,
                    x.BaseRentalPrice,
                    x.DiscountAmount,
                    x.Total,
                    x.AppliedForDays
                ))
                .ToList(),
            price.BaseRentalPrice,
            price.DiscountAmount,
            price.RentalPriceWithDiscount
        );
    }

    public async Task<IReadOnlyList<AdminBookingListItemDto>> GetAdminBookingsAsync(CancellationToken ct = default)
    {
        var rows = await (
            from b in _db.Bookings.AsNoTracking()
            join c in _db.Customers.AsNoTracking()
                on b.CustomerId equals c.Id
            orderby
                b.Status == BookingStatus.Pending ? 0 : 1,
                b.RentalPeriod.StartDate descending
            select new
            {
                BookingId = b.Id,
                BookingStatus = b.Status,
                b.AssetId,
                AssetBrandName = b.AssetSnapshot.BrandName,
                AssetModel = b.AssetSnapshot.Model,
                AssetYear = b.AssetSnapshot.Year,
                CustomerId = c.Id,
                CustomerType = b.CustomerSnapshot.Type,
                CustomerEmail = c.Email,
                CustomerPhone = c.Phone,
                CustomerVerificationStatus = c.VerificationStatus,
                CustomerName = c is Individual
                    ? ((Individual)c).Name.FullName
                    : c is IndividualEntrepreneur
                        ? ((IndividualEntrepreneur)c).Name.FullName
                        : c is Organization
                            ? ((Organization)c).ShortName
                            : null,

                b.RentalPeriod,
                b.Price.RentalPrice,
                b.Price.Deposit,
                b.Price.ServicesPrice,
                b.Price.RouteSurcharge,
                b.Price.Penalties,
                b.Price.TotalToPay,
                b.Price.Total
            }
        ).ToListAsync(ct);

        return rows.Select(x => new AdminBookingListItemDto(
            x.BookingId,
            new AssetShortDto(
                x.AssetId,
                x.AssetBrandName,
                x.AssetModel,
                x.AssetYear,
                null),
            new CustomerShortDto(
                x.CustomerId,
                x.CustomerType,
                x.CustomerName,
                x.CustomerEmail,
                x.CustomerPhone,
                x.CustomerVerificationStatus),
            x.RentalPeriod,
            new BookingPriceDto(
                MapRentalPrice(x.RentalPrice),
                x.Deposit,
                x.ServicesPrice,
                x.RouteSurcharge,
                x.Total,
                x.TotalToPay,
                x.Penalties.Select(p => new BookingPenaltyDto(
                    p.Id,
                    p.Type,
                    p.Amount
                )).ToList(),
                x.Penalties.Sum(p => p.Amount)),
            x.BookingStatus
        )).ToList();
    }

    public async Task<AdminBookingDetailsDto?> GetAdminBookingDetailsAsync(Guid bookingId, CancellationToken ct = default)
    {
        var booking = await (
            from b in _db.Bookings.AsNoTracking()
            join c in _db.Customers.AsNoTracking()
                on b.CustomerId equals c.Id
            join d in _db.Destinations.AsNoTracking()
                on b.Destination.DestinationId equals (Guid?)d.Id
                into destinations
            from d in destinations.DefaultIfEmpty()
            where b.Id == bookingId
            select new
            {
                BookingId = b.Id,
                BookingStatus = b.Status,
                b.CustomerId,
                Asset = new AssetShortDto(
                    b.AssetId,
                    b.AssetSnapshot.BrandName,
                    b.AssetSnapshot.Model,
                    b.AssetSnapshot.Year,
                    null
                ),
                b.RentalPeriod,
                Price = new BookingPriceDto(
                    MapRentalPrice(b.Price.RentalPrice),
                    b.Price.Deposit,
                    b.Price.ServicesPrice,
                    b.Price.RouteSurcharge,
                    b.Price.Total,
                    b.Price.TotalToPay,
                    b.Price.Penalties.Select(x => new BookingPenaltyDto(
                        x.Id,
                        x.Type,
                        x.Amount
                    )).ToList(),
                    b.Price.PenaltiesPrice
                ),
                Customer = new
                {
                    c.Id,
                    b.CustomerSnapshot.Type,
                    c.Email,
                    c.Phone,
                    c.VerificationStatus,
                    IndividualName = c is Individual
                        ? ((Individual)c).Name.FullName
                        : null,
                    EntrepreneurName = c is IndividualEntrepreneur
                        ? ((IndividualEntrepreneur)c).Name.FullName
                        : null,
                    OrganizationShortName = c is Organization
                        ? ((Organization)c).ShortName
                        : null,
                    OrganizationFullName = c is Organization
                        ? ((Organization)c).FullName
                        : null
                },
                Services = b.Services
                    .Select(s => new BookingServiceDto(
                        s.AdditionalServiceId,
                        s.Type,
                        s.Name,
                        s.Price,
                        s.SelectedTime))
                    .ToList(),
                Destination = new BookingDestinationDto(
                    b.Destination.DestinationId,
                    b.Destination.Type,
                    b.Destination.CustomRouteDescription,
                    b.Destination.ReviewStatus,
                    b.Destination.DestinationNameSnapshot
                ),
                DestinationDetails = d == null
                    ? null
                    : new DestinationDto(
                        d.Id,
                        d.Name,
                        d.Region,
                        d.DistanceKm,
                        d.Type,
                        d.RouteSurcharge
                ),
            }
        ).FirstOrDefaultAsync(ct);

        if (booking is null)
            return null;

        var displayName = booking.Customer.Type switch
        {
            CustomerType.Individual =>
                booking.Customer.IndividualName,
            CustomerType.IndividualEntrepreneur =>
                booking.Customer.EntrepreneurName is { } name
                    ? $"ИП {name}"
                    : null,
            CustomerType.Organization =>
                booking.Customer.OrganizationShortName
                ?? booking.Customer.OrganizationFullName,
            _ => null
        };

        var documents = await _db.CustomerDocuments
            .AsNoTracking()
            .Where(x => x.CustomerId == booking.CustomerId)
            .OrderBy(x => x.Type)
            .ToListAsync(ct);

        var documentDtos = new List<CustomerDocumentDto>(documents.Count);
        foreach (var document in documents)
        {
            var url = await _fileStorage.CreatePresignedDownloadUrlAsync(
                document.ObjectKey,
                900,
                ct);

            documentDtos.Add(
                new CustomerDocumentDto(
                    document.Id,
                    document.Type,
                    url,
                    document.UploadedAt));
        }

        var previousBookings = await GetCustomerBookingsForAdminAsync(booking.CustomerId, booking.BookingId, ct);

        return new AdminBookingDetailsDto(
            booking.BookingId,
            booking.BookingStatus,
            booking.Asset,
            booking.RentalPeriod,
            booking.Price,
            booking.Services,

            new AdminCustomerDetailsDto(
                booking.Customer.Id,
                booking.Customer.Type,
                displayName,
                booking.Customer.Email,
                booking.Customer.Phone,
                booking.Customer.VerificationStatus,
                documentDtos,
                previousBookings),
            booking.Destination,
            booking.DestinationDetails);
    }

    public async Task<IReadOnlyList<BookingListItemDto>> GetCustomerBookingsAsync(Guid customerId, CancellationToken ct = default)
    {
        var bookings = await _db.Bookings
            .AsNoTracking()
            .Include(b => b.Services)
            .Where(b => b.CustomerId == customerId)
            .OrderByDescending(b => b.RentalPeriod.StartDate)
            .ToListAsync(ct);

        var coverPhotos = await GetCoverPhotoUrlsAsync(bookings.Select(b => b.AssetId), ct);

        return bookings
            .Select(booking =>
            {
                coverPhotos.TryGetValue(booking.AssetId, out var coverPhotoUrl);

                return MapBookingListItem(
                    booking,
                    coverPhotoUrl);
            }).ToList();
    }

    private async Task<IReadOnlyList<BookingListItemDto>> GetCustomerBookingsForAdminAsync(Guid customerId, Guid? excludeBookingId, CancellationToken ct)
    {
        var query = _db.Bookings
            .AsNoTracking()
            .Include(b => b.Services)
            .Where(b => b.CustomerId == customerId);

        if (excludeBookingId.HasValue)
            query = query.Where(b => b.Id != excludeBookingId.Value);

        var bookings = await query
            .OrderByDescending(b => b.RentalPeriod.StartDate)
            .ToListAsync(ct);

        var coverPhotos = await GetCoverPhotoUrlsAsync(bookings.Select(b => b.AssetId), ct);

        return bookings
            .Select(booking =>
            {
                coverPhotos.TryGetValue(booking.AssetId, out var coverPhotoUrl);

                return MapBookingListItem(booking, coverPhotoUrl);
            })
            .ToList();
    }

    private async Task<Dictionary<Guid, string>> GetCoverPhotoUrlsAsync(IEnumerable<Guid> assetIds, CancellationToken ct)
    {
        var ids = assetIds.Distinct().ToList();

        if (ids.Count == 0)
            return new Dictionary<Guid, string>();

        return await _db.AssetPhotos
            .AsNoTracking()
            .Where(p =>
                ids.Contains(p.AssetId) &&
                p.Type == PhotoType.Exterior)
            .OrderBy(p => p.Order)
            .GroupBy(p => p.AssetId)
            .Select(g => new
            {
                AssetId = g.Key,
                Url = g.First().Url
            })
            .ToDictionaryAsync(
                x => x.AssetId,
                x => x.Url,
                ct);
    }

    public async Task<BookingDetailsDto?> GetCustomerBookingDetailsAsync(
        Guid customerId,
        Guid bookingId,
        CancellationToken ct = default)
    {
        var booking = await (
            from b in _db.Bookings.AsNoTracking()

            join c in _db.Customers.AsNoTracking()
                on b.CustomerId equals c.Id
            join d in _db.Destinations.AsNoTracking()
                on b.Destination.DestinationId equals (Guid?)d.Id
                into destinations
            from d in destinations.DefaultIfEmpty()

            where
                b.Id == bookingId &&
                b.CustomerId == customerId

            select new
            {
                BookingId = b.Id,
                BookingStatus = b.Status,
                b.CustomerId,
                b.AssetId,
                AssetBrandName = b.AssetSnapshot.BrandName,
                AssetModel = b.AssetSnapshot.Model,
                AssetYear = b.AssetSnapshot.Year,
                b.RentalPeriod,
                Price = new BookingPriceDto(
                    MapRentalPrice(b.Price.RentalPrice),
                    b.Price.Deposit,
                    b.Price.ServicesPrice,
                    b.Price.RouteSurcharge,
                    b.Price.Total,
                    b.Price.TotalToPay,
                    b.Price.Penalties.Select(x => new BookingPenaltyDto(
                        x.Id,
                        x.Type,
                        x.Amount
                    )).ToList(),
                    b.Price.PenaltiesPrice
                ),
                Customer = new CustomerShortDto(
                    c.Id,
                    b.CustomerSnapshot.Type,
                    c is Individual
                        ? ((Individual)c).Name.FullName
                        : c is IndividualEntrepreneur
                            ? ((IndividualEntrepreneur)c).Name.FullName
                            : c is Organization
                                ? ((Organization)c).ShortName
                                : null,

                    c.Email,
                    c.Phone,
                    c.VerificationStatus
                ),
                Services = b.Services
                    .Select(s => new BookingServiceDto(
                        s.AdditionalServiceId,
                        s.Type,
                        s.Name,
                        s.Price,
                        s.SelectedTime))
                    .ToList(),
                Destination = new BookingDestinationDto(
                    b.Destination.DestinationId,
                    b.Destination.Type,
                    b.Destination.CustomRouteDescription,
                    b.Destination.ReviewStatus,
                    b.Destination.DestinationNameSnapshot
                ),
                DestinationDetails = d == null
                    ? null
                    : new DestinationDto(
                        d.Id,
                        d.Name,
                        d.Region,
                        d.DistanceKm,
                        d.Type,
                        d.RouteSurcharge
                ),
            }
        ).FirstOrDefaultAsync(ct);

        if (booking is null)
            return null;

        var coverPhotos = await GetCoverPhotoUrlsAsync(
            new[] { booking.AssetId },
            ct);

        coverPhotos.TryGetValue(
            booking.AssetId,
            out var coverPhotoUrl);

        return new BookingDetailsDto(
            booking.BookingId,
            booking.BookingStatus,
            booking.CustomerId,
            booking.Customer,
            booking.AssetId,
            new AssetShortDto(
                booking.AssetId,
                booking.AssetBrandName,
                booking.AssetModel,
                booking.AssetYear,
                coverPhotoUrl),
            booking.RentalPeriod,
            booking.Price,
            booking.Services,
            booking.Destination,
            booking.DestinationDetails
        );
    } 
}