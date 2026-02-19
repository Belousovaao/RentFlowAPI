using System;

namespace RentFlow.Domain.Customers;

public abstract class Customer
{
    public Guid Id { get; set; }
    public CustomerType Type {get; set; }
    public Guid? IndividualId { get; set; }
    public Guid? OrganizationId { get; set; }
    public Guid? IndividualEntrepreneurId { get; set; }
    public string Email {get; set; }
    public string Phone { get; set;}
}

public enum CustomerType
{
    Individual,
    IndividualEntrepreneur,
    Organization
}
