using System;
using RentFlow.Domain.Bookings;

namespace RentFlow.Application.Interfaces;

public interface IBookingRepository
{
    Task<IEnumerable<Booking>> GetAllAsync();
    Task<Booking?> GetByIdAsync(Guid id);
    Task AddAsync(Booking booking);
}
