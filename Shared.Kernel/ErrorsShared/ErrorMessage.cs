using FluentValidation.Results;

namespace Shared.Kernel.ErrorsShared;

public record ErrorMessage
{
    public ErrorMessage(string code, string message)
    { Code = code; Message = message; }

    private string Code { get; }
    private string Message { get; }

    public static implicit operator ErrorMessage(ValidationFailure error)
    {
        return new ErrorMessage(error.ErrorCode, error.ErrorMessage);
    }
}
