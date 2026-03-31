using System;

namespace RentFlow.Domain.Customers;

public sealed class BankAccount
{
    public string? CurrentAccount { get; set; }
    public string? CorrespondentAccount { get; set; }
    public string? BIK { get; set; }
    public string? BankName { get; set; }

    private BankAccount() {}
    public BankAccount(string? currentAccount, string? correspondentAccount, string? bik, string? bankName)
    {
        CurrentAccount = currentAccount;
        CorrespondentAccount = correspondentAccount;
        BIK = bik;
        BankName = bankName;
    }

    public string FullAccountData => 
        $"р/с {CurrentAccount} в {BankName}, к/с {CorrespondentAccount}, БИК {BIK}";
}
