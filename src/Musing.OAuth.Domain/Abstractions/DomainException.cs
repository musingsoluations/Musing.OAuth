using System;

namespace Musing.OAuth.Domain.Abstractions;

public class DomainException : Exception
{
    public DomainException(string message) : base(message) { }
}
