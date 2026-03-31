using System;
using System.ComponentModel.DataAnnotations;
using RentFlow.Domain.Customers;

namespace RentFlow.Application.Bookings.Dtos;

public sealed record DriverDto(
    string FirstName,
    string LastName,
    string MiddleName,
    string Phone,
    string LisenceNumber,
    List<char> Categories,
    DateOnly IssuedDate,
    DateOnly ExpirationDate
);