using System;

namespace RentFlow.Domain.Customers;

public abstract class Customer
{
    public Guid Id { get; set; }
    public CustomerType Type {get; set; }
    public string Email {get; set; }
    public string Phone { get; set;}
}

public enum CustomerType
{
    Individual,
    IndividualEntrepreneur,
    Organization
}
