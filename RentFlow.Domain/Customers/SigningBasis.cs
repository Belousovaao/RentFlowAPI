using System;

namespace RentFlow.Domain.Customers;

public class SigningBasis
{
    SigningBasisType Type { get; set; }
    public string? AttorneyNumber { get; set; }
    public DateOnly? AttorneyDate { get; set; }
    private SigningBasis() {}
    public SigningBasis ByCharter()
    {
        return new SigningBasis
        {
            Type = SigningBasisType.Charter
        };
    }

    public SigningBasis ByAttorney(string number, DateOnly date)
    {
        return new SigningBasis
        {
            Type = SigningBasisType.Attorney,
            AttorneyDate = date,
            AttorneyNumber = number
        };
    }

}

public enum SigningBasisType
{
    Charter, //устав
    Attorney //доверка
}
