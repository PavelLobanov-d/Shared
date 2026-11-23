using Shared.Kernel.ErrorsShared;
using System.Text.Json;

namespace Shared.Kernel.Exceptions;

public class NotFoundException : Exception
{
    //public NotFoundException(IEnumerable<string> messages) : base(string.Join(", ", messages)) { }
    public NotFoundException(Error error) : base(JsonSerializer.Serialize(error.ToErrors())) { }
    public NotFoundException(Errors errors) : base(JsonSerializer.Serialize(errors)) { }
    public NotFoundException(Guid guid, String? record = null) : base($"{record ?? "Record"} with {guid} not found") { }

    public NotFoundException() : base()
    {
    }

    public NotFoundException(string? message, Exception? innerException) : base(message, innerException)
    {
    }
}


