using System;

namespace RentFlow.Domain.Bookings;

public class BookingRole
{
    public Guid Id { get; set; }
    public Guid BoolingId { get; set; }
    public Guid PersonId { get; set; }
    public BookingRoleType RoleType { get; set; }
    private BookingRole() {}
    public BookingRole(Guid personId, BookingRoleType roleType)
    {
        Id = Guid.NewGuid();
        PersonId = personId;
        RoleType = roleType;
    }

}

public enum BookingRoleType
{
    Driver,
    Signatory,
    ContactPerson
}
