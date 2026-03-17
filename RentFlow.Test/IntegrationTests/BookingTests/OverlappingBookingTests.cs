using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RentFlow.Application.Bookings.Commands;
using RentFlow.Domain.Assets;
using RentFlow.Domain.Bookings;
using RentFlow.Domain.Customers;
using RentFlow.Test.IntegrationTests.Builders;
using RentFlow.Test.IntegrationTests.Infrastructure;
namespace RentFlow.Test.IntegrationTests.BookingTests;

public class OverlappingBookingTests : IntegrationBaseTest
{
    public OverlappingBookingTests(PostgreSqlContainerFixture fixture) : base(fixture)
    {
    }

    [Fact]
    public async Task Should_Throw_When_Booking_Overlaps_Completely()
    {
        await ResetDatabaseAsync();
        
        await using var arrangeDb = await CreateDbContextAsync();
        Customer customer = new CustomerBuilder().Build();
        Asset asset = new AssetBuilder().Build();
        arrangeDb.Customers.Add(customer);
        arrangeDb.Assets.Add(asset);
        await arrangeDb.SaveChangesAsync();

        CreateBookingCommand command1 = new CreateBookingCommandBuilder()
            .WithAsset(asset.Id)
            .WithCustomer(customer.Id)
            .WithPeriod(DateTime.UtcNow.AddDays(1), DateTime.UtcNow.AddDays(5))
            .Build();

        var response1 = await Client.PostAsJsonAsync("/api/booking", command1);
        response1.IsSuccessStatusCode.Should().BeTrue();

        //полностью перекрывающее бронирование
        CreateBookingCommand command2 = new CreateBookingCommandBuilder()
            .WithAsset(asset.Id)
            .WithCustomer(customer.Id)
            .WithPeriod(DateTime.UtcNow.AddDays(2), DateTime.UtcNow.AddDays(4))
            .Build();

        var response2 = await Client.PostAsJsonAsync("/api/booking", command2);
        
        response2.IsSuccessStatusCode.Should().BeFalse();
        
        // второе бронирование не создалось
        await using var assertDb = await CreateDbContextAsync();
        var bookings = await assertDb.Bookings
            .Where(b => b.AssetId == asset.Id)
            .ToListAsync();
        bookings.Count.Should().Be(1);
    }

    [Fact]
    public async Task Should_Throw_When_Booking_Overlaps_Start()
    {
        await ResetDatabaseAsync();
        
        await using var arrangeDb = await CreateDbContextAsync();
        Customer customer = new CustomerBuilder().Build();
        Asset asset = new AssetBuilder().Build();
        arrangeDb.Customers.Add(customer);
        arrangeDb.Assets.Add(asset);
        await arrangeDb.SaveChangesAsync();

        var start1 = DateTime.UtcNow.AddDays(2);
        var end1 = DateTime.UtcNow.AddDays(5);
        
        var start2 = start1.AddDays(-1);
        var end2 = start1.AddDays(2); // Пересекается с началом первого

        CreateBookingCommand command1 = new CreateBookingCommandBuilder()
            .WithAsset(asset.Id)
            .WithCustomer(customer.Id)
            .WithPeriod(start1, end1)
            .Build();

        var response1 = await Client.PostAsJsonAsync("/api/booking", command1);
        response1.IsSuccessStatusCode.Should().BeTrue();

        CreateBookingCommand command2 = new CreateBookingCommandBuilder()
            .WithAsset(asset.Id)
            .WithCustomer(customer.Id)
            .WithPeriod(start2, end2)
            .Build();

        var response2 = await Client.PostAsJsonAsync("/api/booking", command2);
        
        response2.IsSuccessStatusCode.Should().BeFalse();
    }

    [Fact]
    public async Task Should_Throw_When_Booking_Overlaps_End()
    {
         await ResetDatabaseAsync();
        
        await using var arrangeDb = await CreateDbContextAsync();
        Customer customer = new CustomerBuilder().Build();
        Asset asset = new AssetBuilder().Build();
        arrangeDb.Customers.Add(customer);
        arrangeDb.Assets.Add(asset);
        await arrangeDb.SaveChangesAsync();

        var start1 = DateTime.UtcNow.AddDays(1);
        var end1 = DateTime.UtcNow.AddDays(5);
        
        var start2 = start1.AddDays(4);
        var end2 = start1.AddDays(8); // Пересекается с концом первого

        CreateBookingCommand command1 = new CreateBookingCommandBuilder()
            .WithAsset(asset.Id)
            .WithCustomer(customer.Id)
            .WithPeriod(start1, end1)
            .Build();

        var response1 = await Client.PostAsJsonAsync("/api/booking", command1);
        response1.IsSuccessStatusCode.Should().BeTrue();

        CreateBookingCommand command2 = new CreateBookingCommandBuilder()
            .WithAsset(asset.Id)
            .WithCustomer(customer.Id)
            .WithPeriod(start2, end2)
            .Build();

        var response2 = await Client.PostAsJsonAsync("/api/booking", command2);
        
        response2.IsSuccessStatusCode.Should().BeFalse();
    }

    [Fact]
    public async Task Should_Throw_When_Booking_Exactly_Matches_Existing()
    {
        await ResetDatabaseAsync();
        
        await using var arrangeDb = await CreateDbContextAsync();
        Customer customer = new CustomerBuilder().Build();
        Asset asset = new AssetBuilder().Build();
        arrangeDb.Customers.Add(customer);
        arrangeDb.Assets.Add(asset);
        await arrangeDb.SaveChangesAsync();

        var start = DateTime.UtcNow.AddDays(1);
        var end = DateTime.UtcNow.AddDays(5);

        CreateBookingCommand command1 = new CreateBookingCommandBuilder()
            .WithAsset(asset.Id)
            .WithCustomer(customer.Id)
            .WithPeriod(start, end)
            .Build();

        var response1 = await Client.PostAsJsonAsync("/api/booking", command1);
        response1.IsSuccessStatusCode.Should().BeTrue();

        CreateBookingCommand command2 = new CreateBookingCommandBuilder()
            .WithAsset(asset.Id)
            .WithCustomer(customer.Id)
            .WithPeriod(start, end) // Те же даты
            .Build();

        var response2 = await Client.PostAsJsonAsync("/api/booking", command2);
        
        response2.IsSuccessStatusCode.Should().BeFalse();
    }

    [Fact]
    public async Task Should_Throw_When_New_Booking_Starts_Exactly_When_Existing_Ends()
    {
        await ResetDatabaseAsync();
        
        await using var arrangeDb = await CreateDbContextAsync();
        Customer customer = new CustomerBuilder().Build();
        Asset asset = new AssetBuilder().Build();
        arrangeDb.Customers.Add(customer);
        arrangeDb.Assets.Add(asset);
        await arrangeDb.SaveChangesAsync();

        var start1 = DateTime.UtcNow.AddDays(1);
        var end1 = DateTime.UtcNow.AddDays(5);
        
        var start2 = end1; // Начинается точно в день окончания первого
        var end2 = DateTime.UtcNow.AddDays(9);

        CreateBookingCommand command1 = new CreateBookingCommandBuilder()
            .WithAsset(asset.Id)
            .WithCustomer(customer.Id)
            .WithPeriod(start1, end1)
            .Build();

        var response1 = await Client.PostAsJsonAsync("/api/booking", command1);
        response1.IsSuccessStatusCode.Should().BeTrue();

        CreateBookingCommand command2 = new CreateBookingCommandBuilder()
            .WithAsset(asset.Id)
            .WithCustomer(customer.Id)
            .WithPeriod(start2, end2)
            .Build();

        var response2 = await Client.PostAsJsonAsync("/api/booking", command2);
        
        // ДОЛЖНО БЫТЬ CONFLICT, потому что '[]' включает end date
        response2.IsSuccessStatusCode.Should().BeFalse();
    }

    [Fact]
    public async Task Should_Throw_When_New_Booking_Ends_Exactly_When_Existing_Starts()
    {
        // Arrange
        await ResetDatabaseAsync();
        
        await using var arrangeDb = await CreateDbContextAsync();
        Customer customer = new CustomerBuilder().Build();
        Asset asset = new AssetBuilder().Build();
        arrangeDb.Customers.Add(customer);
        arrangeDb.Assets.Add(asset);
        await arrangeDb.SaveChangesAsync();

        var start1 = DateTime.UtcNow.AddDays(5);
        var end1 = DateTime.UtcNow.AddDays(10);
        
        var start2 = DateTime.UtcNow.AddDays(1);
        var end2 = start1; // Заканчивается точно в день начала первого

        CreateBookingCommand command1 = new CreateBookingCommandBuilder()
            .WithAsset(asset.Id)
            .WithCustomer(customer.Id)
            .WithPeriod(start1, end1)
            .Build();

        var response1 = await Client.PostAsJsonAsync("/api/booking", command1);
        response1.IsSuccessStatusCode.Should().BeTrue();

        CreateBookingCommand command2 = new CreateBookingCommandBuilder()
            .WithAsset(asset.Id)
            .WithCustomer(customer.Id)
            .WithPeriod(start2, end2)
            .Build();

        var response2 = await Client.PostAsJsonAsync("/api/booking", command2);
        
        // ДОЛЖНО БЫТЬ CONFLICT, потому что '[]' включает start date
        response2.IsSuccessStatusCode.Should().BeFalse();
    }

    [Fact]
    public async Task Should_Allow_Booking_When_No_Overlap_With_Gap()
    {
        await ResetDatabaseAsync();
        
        await using var arrangeDb = await CreateDbContextAsync();
        Customer customer = new CustomerBuilder().Build();
        Asset asset = new AssetBuilder().Build();
        arrangeDb.Customers.Add(customer);
        arrangeDb.Assets.Add(asset);
        await arrangeDb.SaveChangesAsync();

        var start1 = DateTime.UtcNow.AddDays(1);
        var end1 = DateTime.UtcNow.AddDays(5);
        
        var start2 = DateTime.UtcNow.AddDays(6); // День после окончания первого
        var end2 = DateTime.UtcNow.AddDays(10);

        CreateBookingCommand command1 = new CreateBookingCommandBuilder()
            .WithAsset(asset.Id)
            .WithCustomer(customer.Id)
            .WithPeriod(start1, end1)
            .Build();

        var response1 = await Client.PostAsJsonAsync("/api/booking", command1);
        response1.IsSuccessStatusCode.Should().BeTrue();

        CreateBookingCommand command2 = new CreateBookingCommandBuilder()
            .WithAsset(asset.Id)
            .WithCustomer(customer.Id)
            .WithPeriod(start2, end2)
            .Build();

        var response2 = await Client.PostAsJsonAsync("/api/booking", command2);
        
        response2.IsSuccessStatusCode.Should().BeTrue();
        
        await using var assertDb = await CreateDbContextAsync();
        var bookings = await assertDb.Bookings
            .Where(b => b.AssetId == asset.Id)
            .ToListAsync();
        bookings.Count.Should().Be(2);
    }

    [Fact]
    public async Task Should_Allow_Overlapping_Bookings_For_Different_Assets()
    {
        await ResetDatabaseAsync();
        
        await using var arrangeDb = await CreateDbContextAsync();
        Customer customer = new CustomerBuilder().Build();
        Asset asset1 = new AssetBuilder().Build();
        Asset asset2 = new AssetBuilder().Build(); // Другой ассет
        arrangeDb.Customers.Add(customer);
        arrangeDb.Assets.AddRange(asset1, asset2);
        await arrangeDb.SaveChangesAsync();

        var start = DateTime.UtcNow.AddDays(1);
        var end = DateTime.UtcNow.AddDays(5);

        // Бронирование для первого ассета
        CreateBookingCommand command1 = new CreateBookingCommandBuilder()
            .WithAsset(asset1.Id)
            .WithCustomer(customer.Id)
            .WithPeriod(start, end)
            .Build();

        var response1 = await Client.PostAsJsonAsync("/api/booking", command1);
        response1.IsSuccessStatusCode.Should().BeTrue();

        // Бронирование для второго ассета с теми же датами
        CreateBookingCommand command2 = new CreateBookingCommandBuilder()
            .WithAsset(asset2.Id)
            .WithCustomer(customer.Id)
            .WithPeriod(start, end)
            .Build();

        var response2 = await Client.PostAsJsonAsync("/api/booking", command2);
        
        // Должно быть успешно, потому что разные ассеты
        response2.IsSuccessStatusCode.Should().BeTrue();
        
        await using var assertDb = await CreateDbContextAsync();
        var bookings = await assertDb.Bookings.ToListAsync();
        bookings.Count.Should().Be(2);
    }

    [Fact]
    public async Task Should_Throw_When_Updating_Period_Causes_Overlap()
    {
        await ResetDatabaseAsync();
        
        await using var arrangeDb = await CreateDbContextAsync();
        Customer customer = new CustomerBuilder().Build();
        Asset asset = new AssetBuilder().Build();
        arrangeDb.Customers.Add(customer);
        arrangeDb.Assets.Add(asset);
        await arrangeDb.SaveChangesAsync();

        // Создаем два непересекающихся бронирования
        CreateBookingCommand command1 = new CreateBookingCommandBuilder()
            .WithAsset(asset.Id)
            .WithCustomer(customer.Id)
            .WithPeriod(DateTime.UtcNow.AddDays(1), DateTime.UtcNow.AddDays(3))
            .Build();

        var response1 = await Client.PostAsJsonAsync("/api/booking", command1);
        response1.IsSuccessStatusCode.Should().BeTrue();
    
        var responseContent = await response1.Content.ReadAsStringAsync();
        using var jsonDoc = JsonDocument.Parse(responseContent);
        Guid booking1Id = jsonDoc.RootElement.GetProperty("id").GetGuid();

        CreateBookingCommand command2 = new CreateBookingCommandBuilder()
            .WithAsset(asset.Id)
            .WithCustomer(customer.Id)
            .WithPeriod(DateTime.UtcNow.AddDays(4), DateTime.UtcNow.AddDays(6))
            .Build();

        var response2 = await Client.PostAsJsonAsync("/api/booking", command2);
        response2.IsSuccessStatusCode.Should().BeTrue();

        //пытаемся обновить второе бронирование, чтобы оно пересеклось с первым
        var updateCommand = new 
        { 
            StartDate = DateTime.UtcNow.AddDays(2), 
            EndDate = DateTime.UtcNow.AddDays(5) 
        };
        
        var updateResponse = await Client.PutAsJsonAsync($"/api/booking/{booking1Id}/period", updateCommand);
        
        updateResponse.IsSuccessStatusCode.Should().BeFalse();
    }

    [Fact]
    public async Task Should_Handle_Concurrent_Bookings_For_Same_Asset()
    {
        await ResetDatabaseAsync();
        
        await using var arrangeDb = await CreateDbContextAsync();
        Customer customer1 = new CustomerBuilder().Build();
        Customer customer2 = new CustomerBuilder().Build();
        Asset asset = new AssetBuilder().Build();
        arrangeDb.Customers.AddRange(customer1, customer2);
        arrangeDb.Assets.Add(asset);
        await arrangeDb.SaveChangesAsync();

        var start = DateTime.UtcNow.AddDays(1);
        var end = DateTime.UtcNow.AddDays(5);

        CreateBookingCommand command1 = new CreateBookingCommandBuilder()
            .WithAsset(asset.Id)
            .WithCustomer(customer1.Id)
            .WithPeriod(start, end)
            .Build();

        CreateBookingCommand command2 = new CreateBookingCommandBuilder()
            .WithAsset(asset.Id)
            .WithCustomer(customer2.Id)
            .WithPeriod(start, end)
            .Build();

        // отправляем два запроса параллельно
        var task1 = Client.PostAsJsonAsync("/api/booking", command1);
        var task2 = Client.PostAsJsonAsync("/api/booking", command2);
        
        await Task.WhenAll(task1, task2);
        
        var response1 = await task1;
        var response2 = await task2;

        // только одно должно быть успешным
        var successCount = new[] { response1, response2 }
            .Count(r => r.IsSuccessStatusCode);
        
        successCount.Should().Be(1);
        
        // В БД должна быть только одна запись
        await using var assertDb = await CreateDbContextAsync();
        var bookings = await assertDb.Bookings
            .Where(b => b.AssetId == asset.Id)
            .ToListAsync();
        bookings.Count.Should().Be(1);
    }
}
