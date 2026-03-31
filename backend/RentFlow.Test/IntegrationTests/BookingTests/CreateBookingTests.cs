using System.Net.Http.Json;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RentFlow.Application.Bookings.Commands;
using RentFlow.Domain.Assets;
using RentFlow.Domain.Bookings;
using RentFlow.Domain.Customers;
using RentFlow.Test.IntegrationTests.Builders;
using RentFlow.Test.IntegrationTests.Infrastructure;

namespace RentFlow.Test.IntegrationTests;

public class CreateBookingTests : IntegrationBaseTest
{
    public CreateBookingTests(PostgreSqlContainerFixture fixture) : base(fixture)
    {
    }

    [Fact]
    public async Task Should_Create_Booking_Individual_Customer_Without_Driver()
    {
        await ResetDatabaseAsync();

        await using var arrangeDb = await CreateDbContextAsync();

        Customer customer = new CustomerBuilder().Build();
        arrangeDb.Customers.Add(customer);
        await arrangeDb.SaveChangesAsync();

        Asset asset = new AssetBuilder().Build();
        arrangeDb.Assets.Add(asset);
        await arrangeDb.SaveChangesAsync();

        CreateBookingCommand command = new CreateBookingCommandBuilder().WithAsset(asset.Id).WithCustomer(customer.Id).Build();

        var response = await Client.PostAsJsonAsync("/api/booking", command);
        var content = await response.Content.ReadAsStringAsync();

        System.Diagnostics.Debug.WriteLine($"Response: {content}");

        if (!response.IsSuccessStatusCode)
        {
            try
            {
                var problem = System.Text.Json.JsonSerializer.Deserialize<ProblemDetails>(content);
                throw new Exception($"Request failed: {problem?.Title} - {problem?.Detail}");
            }
            catch
            {
                throw new Exception($"Request failed with status {response.StatusCode}: {content}");
            }
        }
        await using var assertDb = await CreateDbContextAsync();
        Booking booking = await assertDb.Bookings.Where(b => b.AssetId == asset.Id && b.CustomerId == customer.Id).FirstAsync();

        booking.CustomerId.Should().Be(customer.Id);
        booking.AssetId.Should().Be(asset.Id); 

        booking.CustomerSnapshot.Should().NotBeNull(); 
        booking.CustomerSnapshot.Type.Should().Be(CustomerType.Individual);

        Individual individualCustomer = (Individual)customer;
        booking.CustomerSnapshot.Name!.FirstName.Should().Be(individualCustomer.Name.FirstName);
        booking.CustomerSnapshot.Name!.LastName.Should().Be(individualCustomer.Name.LastName); 
        booking.CustomerSnapshot.Passport!.Number.Should().Be(individualCustomer.IndividualPassport!.Number);
        booking.CustomerSnapshot.Email.Should().Be(individualCustomer.Email);
        booking.CustomerSnapshot.Phone.Should().Be(individualCustomer.Phone);

        booking.AssetSnapshot.Should().NotBeNull();
        booking.AssetSnapshot.Category.Should().Be(asset.Category);
        booking.AssetSnapshot.DailyPrice.Should().Be(asset.DailyPrice);
        booking.AssetSnapshot.Deposit.Should().Be(asset.Deposit);
        booking.AssetSnapshot.Type.Should().Be(asset.Type);
        booking.AssetSnapshot.DeliveryPrice.Should().Be(asset.DeliveryPrice);
        booking.AssetSnapshot.CanDeliver.Should().Be(asset.CanDeliver);

        booking.BookingDriver.Should().NotBeNull(); 
        booking.BookingDriver.Phone.Should().Be(individualCustomer.Phone);
        booking.BookingDriver.Name.FirstName.Should().Be(individualCustomer.Name.FirstName);  
        booking.BookingDriver.Name.LastName.Should().Be(individualCustomer.Name.LastName); 
    }

    [Fact]
    public async Task Should_Create_Booking_Individual_Customer_With_Driver()
    {
        await ResetDatabaseAsync();

        await using var arrangeDb = await CreateDbContextAsync();

        Customer customer = new CustomerBuilder().Build();
        arrangeDb.Customers.Add(customer);
        await arrangeDb.SaveChangesAsync();

        Asset asset = new AssetBuilder().Build();
        arrangeDb.Assets.Add(asset);
        await arrangeDb.SaveChangesAsync();

        CreateBookingCommand command = new CreateBookingCommandBuilder().WithAsset(asset.Id).WithCustomer(customer.Id).WithDriver().Build();

        var response = await Client.PostAsJsonAsync("/api/booking", command);
        var content = await response.Content.ReadAsStringAsync();
        if (!response.IsSuccessStatusCode)
        {
            try
            {
                var problem = System.Text.Json.JsonSerializer.Deserialize<ProblemDetails>(content);
                throw new Exception($"Request failed: {problem?.Title} - {problem?.Detail}");
            }
            catch
            {
                throw new Exception($"Request failed with status {response.StatusCode}: {content}");
            }
        }
        await using var assertDb = await CreateDbContextAsync();
        Booking booking = await assertDb.Bookings.Where(b => b.AssetId == asset.Id && b.CustomerId == customer.Id).FirstAsync();

        booking.CustomerId.Should().Be(customer.Id);
        booking.AssetId.Should().Be(asset.Id);    

        booking.CustomerSnapshot.Should().NotBeNull(); 
        booking.CustomerSnapshot.Type.Should().Be(CustomerType.Individual);

        booking.BookingDriver.Should().NotBeNull();
        booking.BookingDriver.Id.Should().NotBeEmpty();

        booking.BookingDriver.Name.Should().NotBeNull();
        booking.BookingDriver.Name.FirstName.Should().Be("Ivan");
        booking.BookingDriver.Name.LastName.Should().Be("Petrov");
        booking.BookingDriver.Name.MiddleName.Should().Be("Ivanovich");
        booking.BookingDriver.Phone.Should().Be("+7 (900) 000-22-00");
        booking.BookingDriver.License.Should().NotBeNull();
        booking.BookingDriver.License.Number.Should().Be("11 22 333333");
        booking.BookingDriver.License.IssuedDate.Should().Be(new DateOnly(2020, 1, 1));
        booking.BookingDriver.License.ExpirationDate.Should().Be(new DateOnly(2030, 1, 1));
        booking.BookingDriver.License.Categories.Should().Contain("A");
        booking.BookingDriver.License.Categories.Should().Contain("B");

        booking.AssetSnapshot.Should().NotBeNull();
        booking.AssetSnapshot.Category.Should().Be(asset.Category);
    }

    [Fact]
    public async Task Should_Create_Booking_Entrepreneur_Customer_Without_Driver()
    {
        await ResetDatabaseAsync();

        await using var arrangeDb = await CreateDbContextAsync();

        Customer customer = new CustomerBuilder().AsEntrepreneur().Build();
        arrangeDb.Customers.Add(customer);
        await arrangeDb.SaveChangesAsync();

        Asset asset = new AssetBuilder().Build();
        arrangeDb.Assets.Add(asset);
        await arrangeDb.SaveChangesAsync();

        CreateBookingCommand command = new CreateBookingCommandBuilder().WithAsset(asset.Id).WithCustomer(customer.Id).Build();

        var response = await Client.PostAsJsonAsync("/api/booking", command);
        var content = await response.Content.ReadAsStringAsync();
        if (!response.IsSuccessStatusCode)
        {
            try
            {
                var problem = System.Text.Json.JsonSerializer.Deserialize<ProblemDetails>(content);
                throw new Exception($"Request failed: {problem?.Title} - {problem?.Detail}");
            }
            catch
            {
                throw new Exception($"Request failed with status {response.StatusCode}: {content}");
            }
        }
        await using var assertDb = await CreateDbContextAsync();
        Booking booking = await assertDb.Bookings.Where(b => b.AssetId == asset.Id && b.CustomerId == customer.Id).FirstAsync();
        booking.CustomerId.Should().Be(customer.Id);
        booking.AssetId.Should().Be(asset.Id); 

        booking.CustomerSnapshot.Should().NotBeNull(); 
        booking.CustomerSnapshot.Type.Should().Be(CustomerType.IndividualEntrepreneur);

        IndividualEntrepreneur entrepreneur = (IndividualEntrepreneur)customer;
        booking.CustomerSnapshot.Name!.FirstName.Should().Be(entrepreneur.Name.FirstName);
        booking.CustomerSnapshot.Name!.LastName.Should().Be(entrepreneur.Name.LastName); 
        booking.CustomerSnapshot.OrganizationAddress.Should().Be(entrepreneur.OrganizationAdress);
        booking.CustomerSnapshot.FactAddress.Should().Be(entrepreneur.FactAdress);
        booking.CustomerSnapshot.BankAccount!.CurrentAccount.Should().Be(entrepreneur.IPBankAccount.CurrentAccount);
        booking.CustomerSnapshot.Email.Should().Be(entrepreneur.Email);
        booking.CustomerSnapshot.Phone.Should().Be(entrepreneur.Phone);

        booking.AssetSnapshot.Should().NotBeNull();
        booking.AssetSnapshot.Category.Should().Be(asset.Category);

        booking.BookingDriver.Should().NotBeNull(); 
        booking.BookingDriver.Phone.Should().Be(entrepreneur.Phone);
        booking.BookingDriver.Name.FirstName.Should().Be(entrepreneur.Name.FirstName);  
        booking.BookingDriver.Name.LastName.Should().Be(entrepreneur.Name.LastName);        
    }

     [Fact]
    public async Task Should_Create_Booking_Entrepreneur_Customer_With_Driver()
    {
        await ResetDatabaseAsync();

        await using var arrangeDb = await CreateDbContextAsync();

        Customer customer = new CustomerBuilder().AsEntrepreneur().Build();
        arrangeDb.Customers.Add(customer);
        await arrangeDb.SaveChangesAsync();

        Asset asset = new AssetBuilder().Build();
        arrangeDb.Assets.Add(asset);
        await arrangeDb.SaveChangesAsync();

        CreateBookingCommand command = new CreateBookingCommandBuilder().WithAsset(asset.Id).WithCustomer(customer.Id).WithDriver().Build();

        var response = await Client.PostAsJsonAsync("/api/booking", command);
        var content = await response.Content.ReadAsStringAsync();
        if (!response.IsSuccessStatusCode)
        {
            try
            {
                var problem = System.Text.Json.JsonSerializer.Deserialize<ProblemDetails>(content);
                throw new Exception($"Request failed: {problem?.Title} - {problem?.Detail}");
            }
            catch
            {
                throw new Exception($"Request failed with status {response.StatusCode}: {content}");
            }
        }
        await using var assertDb = await CreateDbContextAsync();
        Booking booking = await assertDb.Bookings.Where(b => b.AssetId == asset.Id && b.CustomerId == customer.Id).FirstAsync();
        booking.CustomerId.Should().Be(customer.Id);
        booking.AssetId.Should().Be(asset.Id);      
        
        booking.CustomerSnapshot.Should().NotBeNull(); 
        booking.CustomerSnapshot.Type.Should().Be(CustomerType.IndividualEntrepreneur);

        booking.BookingDriver.Should().NotBeNull();
        booking.BookingDriver.Id.Should().NotBeEmpty();

        booking.BookingDriver.Name.Should().NotBeNull();
        booking.BookingDriver.Name.FirstName.Should().Be("Ivan");
        booking.BookingDriver.Name.LastName.Should().Be("Petrov");
        booking.BookingDriver.Name.MiddleName.Should().Be("Ivanovich");
        booking.BookingDriver.Phone.Should().Be("+7 (900) 000-22-00");
        booking.BookingDriver.License.Should().NotBeNull();
        booking.BookingDriver.License.Number.Should().Be("11 22 333333");
        booking.BookingDriver.License.IssuedDate.Should().Be(new DateOnly(2020, 1, 1));
        booking.BookingDriver.License.ExpirationDate.Should().Be(new DateOnly(2030, 1, 1));
        booking.BookingDriver.License.Categories.Should().Contain("A");
        booking.BookingDriver.License.Categories.Should().Contain("B");

        booking.AssetSnapshot.Should().NotBeNull();
        booking.AssetSnapshot.Category.Should().Be(asset.Category);
    }

    [Fact]
    public async Task Should_Throw_When_Organization_Customer_Without_Driver()
    {
        await ResetDatabaseAsync();

        await using var arrangeDb = await CreateDbContextAsync();

        Customer customer = new CustomerBuilder().AsOrganization().Build();
        arrangeDb.Customers.Add(customer);
        await arrangeDb.SaveChangesAsync();

        Asset asset = new AssetBuilder().Build();
        arrangeDb.Assets.Add(asset);
        await arrangeDb.SaveChangesAsync();

        CreateBookingCommand command = new CreateBookingCommandBuilder().WithAsset(asset.Id).WithCustomer(customer.Id).Build();

        var response = await Client.PostAsJsonAsync("/api/booking", command);
        response.IsSuccessStatusCode.Should().BeFalse();
        await using var assertDb = await CreateDbContextAsync();
        List<Booking> bookings = await assertDb.Bookings.ToListAsync();
        bookings.Should().BeEmpty();       
    }

    [Fact]
    public async Task Should_Create_Booking_Organization_Customer_With_Driver()
    {
        await ResetDatabaseAsync();

        await using var arrangeDb = await CreateDbContextAsync();

        Customer customer = new CustomerBuilder().AsOrganization().Build();
        arrangeDb.Customers.Add(customer);
        await arrangeDb.SaveChangesAsync();

        Asset asset = new AssetBuilder().Build();
        arrangeDb.Assets.Add(asset);
        await arrangeDb.SaveChangesAsync();

        CreateBookingCommand command = new CreateBookingCommandBuilder().WithAsset(asset.Id).WithCustomer(customer.Id).WithDriver().Build();

        var response = await Client.PostAsJsonAsync("/api/booking", command);
        var content = await response.Content.ReadAsStringAsync();
        if (!response.IsSuccessStatusCode)
        {
            try
            {
                var problem = System.Text.Json.JsonSerializer.Deserialize<ProblemDetails>(content);
                throw new Exception($"Request failed: {problem?.Title} - {problem?.Detail}");
            }
            catch
            {
                throw new Exception($"Request failed with status {response.StatusCode}: {content}");
            }
        }
        await using var assertDb = await CreateDbContextAsync();
        Booking booking = await assertDb.Bookings.Where(b => b.AssetId == asset.Id && b.CustomerId == customer.Id).FirstAsync();
        booking.CustomerId.Should().Be(customer.Id);
        booking.AssetId.Should().Be(asset.Id);    
        booking.BookingDriver.Should().NotBeNull();    
        
        booking.CustomerId.Should().Be(customer.Id);
        booking.AssetId.Should().Be(asset.Id);      
        
        booking.CustomerSnapshot.Should().NotBeNull(); 
        booking.CustomerSnapshot.Type.Should().Be(CustomerType.Organization);

        booking.BookingDriver.Should().NotBeNull();
        booking.BookingDriver.Id.Should().NotBeEmpty();
        booking.BookingDriver.Name.FirstName.Should().Be("Ivan");
        booking.BookingDriver.Name.LastName.Should().Be("Petrov");
        booking.BookingDriver.Name.MiddleName.Should().Be("Ivanovich");
        booking.BookingDriver.Phone.Should().Be("+7 (900) 000-22-00");
        booking.BookingDriver.License.Should().NotBeNull();
        booking.BookingDriver.License.Number.Should().Be("11 22 333333");
        booking.BookingDriver.License.IssuedDate.Should().Be(new DateOnly(2020, 1, 1));
        booking.BookingDriver.License.ExpirationDate.Should().Be(new DateOnly(2030, 1, 1));
        booking.BookingDriver.License.Categories.Should().Contain("A");
        booking.BookingDriver.License.Categories.Should().Contain("B");

        booking.AssetSnapshot.Should().NotBeNull();
        booking.AssetSnapshot.Category.Should().Be(asset.Category);
    }

    [Fact]
    public async Task Should_Throw_When_Customer_Not_Found()
    {
        await ResetDatabaseAsync();

        await using var arrangeDb = await CreateDbContextAsync();

        Asset asset = new AssetBuilder().Build();
        arrangeDb.Assets.Add(asset);
        await arrangeDb.SaveChangesAsync();

        CreateBookingCommand command = new CreateBookingCommandBuilder().WithAsset(asset.Id).Build();
        var response = await Client.PostAsJsonAsync("/api/booking", command);
        response.IsSuccessStatusCode.Should().BeFalse();
    }

    [Fact]
    public async Task  Should_Throw_Wnen_Asset_Not_Found()
    {
        await ResetDatabaseAsync();

        await using var arrangeDb = await CreateDbContextAsync();

        Customer customer = new CustomerBuilder().AsOrganization().Build();
        arrangeDb.Customers.Add(customer);
        await arrangeDb.SaveChangesAsync();

        CreateBookingCommand command = new CreateBookingCommandBuilder().Build();
        var response = await Client.PostAsJsonAsync("/api/booking", command);
        response.IsSuccessStatusCode.Should().BeFalse();
    }
}
