using System;
using RentFlow.Domain.Customers.ValueObjects;

namespace RentFlow.Domain.Customers;

public abstract class Customer
{
    public Guid Id { get; protected set; }
    public string? Email {get; protected set; }
    public string Phone { get; protected set;}

    protected Customer() {}

    public abstract Driver ResolveDriver(Driver? provideDriver);
    public static Customer CreateIndividual(
        string? email, 
        string phone, 
        PersonName name, 
        DateOnly? dateOfBirth,
        Passport? passport,
        DriverLicense? driverLicense,
        BankAccount? bankAccount)
    {
        return new Individual(email, phone, name, dateOfBirth, passport, driverLicense, bankAccount);
    }

    public static Customer CreateIndividualEntrepreneur(
        string? email,
        string phone,
        PersonName name,
        Passport passport,
        string inn,
        string ogrnip,
        string organizationAddress,
        string factAdress,
        BankAccount bankAccount,
        Guid? representativeId,
        DriverLicense? driverLicense)
    {
        return new IndividualEntrepreneur(email, phone, name, passport, inn, ogrnip, organizationAddress, factAdress, bankAccount, representativeId, driverLicense);
    }

    public static Customer CreateOrganization(
            string? email,
            string phone,
            string fullName,
            string shortName,
            OrganizationLegalForm legalForm,
            string inn,
            string ogrn,
            string kpp,
            string organizationAddress,
            string factAddress,
            Guid representativeId,
            SigningBasis signingBasis,
            BankAccount bankAccount)
    {
        return new Organization(email, phone, fullName, shortName, legalForm, inn, ogrn, kpp, organizationAddress, factAddress, representativeId, signingBasis, bankAccount);
    }
}

