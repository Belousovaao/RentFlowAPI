using System;

namespace RentFlow.Domain.Customers.ValueObjects;

public class OrganizationLegalForm
{
    public string ShortName { get; }
    public string FullName { get; }

    private OrganizationLegalForm() {}

    private OrganizationLegalForm(string shortName, string fullName)
    {
        ShortName = shortName;
        FullName = fullName;
    }

    public static readonly OrganizationLegalForm OOO = new("ООО", "Общество с ограниченной ответственностью");
    public static readonly OrganizationLegalForm AO = new("АО", "Акицонерное общество");
    public static readonly OrganizationLegalForm KFH = new("КФХ", "Крестьянское (фермерское) хозяйство");
    public static readonly OrganizationLegalForm IP = new("ИП", "Индивидуальный предприниматель");

    public static OrganizationLegalForm FromShortName(string shortName)
    {
        return shortName switch
        {
            "ООО" => OOO,
            "АО" => AO,
            "КФХ" => KFH,
            "ИП" => IP,
            _ => throw new ArgumentException("Unknown legal form")
        };
    }
}
