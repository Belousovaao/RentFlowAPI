using MediatR;
using RentFlow.Application.Customers.Commands;
using RentFlow.Application.Customers.Dtos;
using RentFlow.Application.Customers.Mappers;
using RentFlow.Application.Interfaces;
using RentFlow.Domain.Customers;

namespace RentFlow.Application.Customers.Handlers;
public class CreateCustomerHandler : IRequestHandler<CreateCustomerCommand, CustomerLookupDto>
{
    private readonly ICustomerRepository _customers;
    private readonly IUnitOfWork _uow;

    public CreateCustomerHandler(ICustomerRepository customers, IUnitOfWork uow)
    {
        _customers = customers;
        _uow = uow;
    }

    public async Task<CustomerLookupDto> Handle(CreateCustomerCommand cmd, CancellationToken ct)
    {
        Customer customer = cmd.Type switch
        {
            CustomerType.Individual =>
                CreateIndividual(cmd),

            CustomerType.IndividualEntrepreneur =>
                CreateEntrepreneur(cmd),

            CustomerType.Organization =>
                CreateOrganization(cmd),

            _ => throw new NotSupportedException()
        };

        await _customers.AddAsync(customer, ct);
        await _uow.SaveChangesAsync(ct);

        return CustomerMapper.ToLookup(customer);
    }

    private static Customer CreateIndividual(CreateCustomerCommand cmd)
    {
        return Customer.CreateIndividual(
            cmd.Email,
            cmd.Phone,
            new PersonName(
                cmd.FirstName,
                cmd.LastName,
                cmd.MiddleName),
            cmd.DateOfBirth,
            CreateCustomerMapper.ToPassport(cmd.Passport),
            CreateCustomerMapper.ToDriverLicense(cmd.DriverLicense),
            CreateCustomerMapper.ToBankAccount(cmd.BankInfo));
    }
    private static Customer CreateEntrepreneur(CreateCustomerCommand cmd)
    {
        return Customer.CreateIndividualEntrepreneur(
            cmd.Email,
            cmd.Phone,
            new PersonName(
                cmd.FirstName,
                cmd.LastName,
                cmd.MiddleName),

            CreateCustomerMapper.ToPassport(cmd.Passport)!,

            cmd.INN!,
            cmd.OGRNIP!,

            cmd.OrganizationAddress!,
            cmd.FactAddress!,

            CreateCustomerMapper.ToBankAccount(cmd.BankInfo)!,

            null,

            CreateCustomerMapper.ToDriverLicense(cmd.DriverLicense));
    }

    private static Customer CreateOrganization(CreateCustomerCommand cmd)
    {
        return Customer.CreateOrganization(
            cmd.Email,
            cmd.Phone,

            cmd.FullName!,
            cmd.ShortName!,

            CreateCustomerMapper.ToLegalForm(cmd),

            cmd.INN!,
            cmd.OGRN!,
            cmd.KPP!,

            cmd.OrganizationAddress!,
            cmd.FactAddress!,

            null,
            null,
            CreateCustomerMapper.ToBankAccount(cmd.BankInfo)!);
    }
}