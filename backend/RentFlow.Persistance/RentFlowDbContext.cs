using Microsoft.EntityFrameworkCore;
using RentFlow.Domain.Assets;
using RentFlow.Domain.Locations;
using RentFlow.Domain.Bookings;
using RentFlow.Domain.Customers;
using RentFlow.Domain.Auth;
using RentFlow.Domain.Bookings.Services;
using RentFlow.Domain.Payments;
using RentFlow.Domain.Settings;
namespace RentFlow.Persistance;
public class RentFlowDbContext : DbContext
{
    public DbSet<Asset> Assets => Set<Asset>();
    public DbSet<Location> Locations => Set<Location>();
    public DbSet<AssetPhoto> AssetPhotos => Set<AssetPhoto>();
    public DbSet<Booking> Bookings => Set<Booking>();
    public DbSet<Customer> Customers => Set<Customer>();
    public DbSet<CustomerDocument> CustomerDocuments => Set<CustomerDocument>();
    public DbSet<AppUser> AppUsers => Set<AppUser>();
    public DbSet<AdditionalService> AdditionalServices => Set<AdditionalService>();
    public DbSet<BookingService> BookingServices => Set<BookingService>();
    public DbSet<Destination> Destinations => Set<Destination>();
    public DbSet<ConsentDocument> ConsentDocuments => Set<ConsentDocument>();
    public DbSet<PersonalDataConsent> PersonalDataConsents => Set<PersonalDataConsent>();
    public DbSet<Payment> Payments => Set<Payment>();
    public DbSet<BookingCharge> BookingCharges => Set<BookingCharge>();
    public DbSet<BookingChargeSettings> BookingChargeSettings => Set<BookingChargeSettings>();
    public DbSet<RentalTariff> RentalTariffs => Set<RentalTariff>();
    public DbSet<PaymentAllocation> PaymentAllocations => Set<PaymentAllocation>();
    public RentFlowDbContext(DbContextOptions<RentFlowDbContext> options) : base(options) { }
    protected override void OnModelCreating(ModelBuilder modelBuilder) => modelBuilder.ApplyConfigurationsFromAssembly(typeof(RentFlowDbContext).Assembly);
}