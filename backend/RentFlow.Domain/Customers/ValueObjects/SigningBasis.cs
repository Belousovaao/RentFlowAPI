using System;

namespace RentFlow.Domain.Customers;

public class SigningBasis
{
    SigningBasisType Type { get; set; }
    public string? AttorneyNumber { get; set; }
    public DateOnly? AttorneyDate { get; set; }
    private SigningBasis() {}
    public static SigningBasis ByCharter()
    {
        return new SigningBasis
        {
            Type = SigningBasisType.Charter
        };
    }

    public static SigningBasis ByAttorney(string number, DateOnly date)
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
