using System;
using RentFlow.Domain.Customers;

namespace RentFlow.Test.IntegrationTests.Builders;

public static class TestData
{
    public static Passport ValidPassport() => 
         new Passport("1111", "222222", "Otdelom UFMS Rossii", new DateOnly(2000,1,1), "Moscow, Tsvetochanaya st., 555 apt.");

    public static DriverLicense ValidDriverLicense() =>
        new("11 22 333333", new List<char>{'A', 'B'}, new DateOnly(2001, 2, 2), new DateOnly(2030, 12, 31));

    public static BankAccount ValidBankAccount() =>
        new("11111111111111111", "222222222222222222222222", "09454950", "VTB-Bank");

}
