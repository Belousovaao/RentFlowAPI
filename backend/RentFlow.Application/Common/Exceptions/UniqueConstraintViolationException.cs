namespace RentFlow.Application.Common.Exceptions;

public sealed class UniqueConstraintViolationException : Exception
{
    public string? ConstraintName { get; }

    public UniqueConstraintViolationException(string? constraintName) : base("A unique constraint was violated.")
    {
        ConstraintName = constraintName;
    }
}