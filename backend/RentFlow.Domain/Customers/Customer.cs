using System;
using RentFlow.Domain.Customers.ValueObjects;

namespace RentFlow.Domain.Customers;

public abstract class Customer
{
    public Guid Id { get; protected set; }
    public string? Email {get; protected set; }
    public string Phone { get; protected set;}
    public VerificationStatus VerificationStatus { get; private set; }
    private readonly List<CustomerDocument> _documents = [];
    public IReadOnlyCollection<CustomerDocument> Documents => _documents;
    private readonly List<PersonalDataConsent> _personalDataConsents = [];

    public IReadOnlyCollection<PersonalDataConsent> PersonalDataConsents => _personalDataConsents.AsReadOnly();

    protected Customer() {}

    public abstract Driver ResolveDriver(Driver? provideDriver);

    //минимальные фабрики:
    public static Customer CreateIndividualEntrepreneur(
        string? email,
        string phone,
        PersonName name,
        string inn)
    {
        return new IndividualEntrepreneur(
            email,
            phone,
            name,
            inn);
    }

    public static Customer CreateOrganization(
        string? email,
        string phone,
        string inn,
        Guid? representativeId)
    {
        return new Organization(
            email,
            phone,
            inn,
            representativeId);
    }

    //полноценные фабрики
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
            Guid? representativeId,
            SigningBasis? signingBasis,
            BankAccount bankAccount)
    {
        return new Organization(email, phone, fullName, shortName, legalForm, inn, ogrn, kpp, organizationAddress, factAddress, representativeId, signingBasis, bankAccount);
    }

    public void SubmitForVerification()
    {
        VerificationStatus = VerificationStatus.Pending;
    }

    public void Approve()
    {
        VerificationStatus = VerificationStatus.Approved;
    }

    public void Reject()
    {
        VerificationStatus = VerificationStatus.Rejected;
    }

    public void AddDocument(CustomerDocument document)
    {
        _documents.Add(document);

        VerificationStatus = VerificationStatus.Pending;
    }

    public void RemoveDocument(CustomerDocument document)
    {
        _documents.Remove(document);
    }

    public void ClearDocuments()
    {
        _documents.Clear();
    }

    public void UpdateContacts(string? email, string phone)
    {
        Email = email;
        Phone = phone;
    }

    public void AddPersonalDataConsent(PersonalDataConsent consent)
    {
        ArgumentNullException.ThrowIfNull(consent);

        if (consent.CustomerId != Id)
            throw new InvalidOperationException("Consent belongs to another customer.");

        if (_personalDataConsents.Any(c => c.Id == consent.Id))
            throw new InvalidOperationException("Consent has already been added.");

        _personalDataConsents.Add(consent);
    }
}

public enum VerificationStatus
{
    NotSubmitted = 0,   // документы не отправлены
    Pending = 1,        // ожидает проверки
    Approved = 2,       // проверен
    Rejected = 3        // отклонен
}



