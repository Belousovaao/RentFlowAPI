using System;
using System.Security.Cryptography.X509Certificates;

namespace RentFlow.Application.Common.Exceptions;

public sealed class NotFoundException : Exception
{
    public NotFoundException(string entity, object key) : base($"{entity} with key '{key}' was nor found")
    {
    }
}
