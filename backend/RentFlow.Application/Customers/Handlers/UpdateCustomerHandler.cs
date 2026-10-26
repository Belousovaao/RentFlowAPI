using MediatR;
using RentFlow.Application.Common.Exceptions;
using RentFlow.Application.Customers.Commands;
using RentFlow.Application.Customers.Dtos;
using RentFlow.Application.Customers.Mappers;
using RentFlow.Application.Interfaces;
using RentFlow.Domain.Customers;
using RentFlow.Domain.Customers.ValueObjects;

namespace RentFlow.Application.Customers.Handlers;

public sealed class UpdateCustomerHandler : IRequestHandler<UpdateCustomerCommand, CustomerDetailsDto>
{
    private readonly ICustomerRepository _customers;
    private readonly IUnitOfWork _uow;
    private readonly IAppUserRepository _users;

    public UpdateCustomerHandler(
        ICustomerRepository customers, 
        IUnitOfWork unitOf,
        IAppUserRepository appUserRepository)
    {
        _customers = customers;
        _uow = unitOf;
        _users = appUserRepository;
    }

    public async Task<CustomerDetailsDto> Handle(UpdateCustomerCommand request, CancellationToken ct)
    {
        var user = await _users.GetByIdAsync(request.UserId, ct);

        if (user is null)
            throw new NotFoundException("User not found", request.UserId);

        if (user.CustomerId is null)
            throw new NotFoundException("Customer profile not found", request.UserId);

        var customer = await _customers.GetByIdAsync(user.CustomerId.Value, ct);

        if (customer is null)
            throw new NotFoundException("Customer not found", user.CustomerId.Value);

        customer.UpdateContacts(request.Data.Email, request.Data.Phone);

        switch(customer)
        {
            case Individual individual:
                individual.UpdatePersonalData(
                    new PersonName(request.Data.FirstName!, request.Data.LastName!, request.Data.MiddleName),
                    request.Data.DateOfBirth,
                    CustomerMapper.ToDomain(request.Data.Passport),
                    CustomerMapper.ToDomain(request.Data.DriverLicense),
                    CustomerMapper.ToDomain(request.Data.BankInfo));
                break;
            
            case Organization organization:
                organization.UpdateOrganizationData(
                    request.Data.ShortName!,
                    request.Data.FullName!,
                    OrganizationLegalForm.FromShortName(request.Data.LegalForm!),
                    request.Data.Inn,
                    request.Data.Ogrn,
                    request.Data.Kpp,
                    request.Data.OrganizationAddress,
                    request.Data.FactAddress,
                    CustomerMapper.ToDomain(request.Data.BankInfo)
                );
                break;
            
            case IndividualEntrepreneur entrepreneur:
                entrepreneur.UpdatePersonalData(
                    new PersonName(
                        request.Data.FirstName!,
                        request.Data.LastName!,
                        request.Data.MiddleName),
                    CustomerMapper.ToDomain(request.Data.Passport),
                    CustomerMapper.ToDomain(request.Data.DriverLicense)
                );

                entrepreneur.UpdateBusinessData(
                    request.Data.Inn!,
                    request.Data.Ogrnip!,
                    request.Data.OrganizationAddress!,
                    request.Data.FactAddress!,
                    CustomerMapper.ToDomain(request.Data.BankInfo)
                );
                break;
        }

        await _uow.SaveChangesAsync(ct);
        return CustomerMapper.ToDetailsDto(customer);
    }
}