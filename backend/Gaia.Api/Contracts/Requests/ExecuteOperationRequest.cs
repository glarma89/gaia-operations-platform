
using System.ComponentModel.DataAnnotations;

namespace Gaia.Api.Contracts.Requests;

public sealed class ExecuteOperationRequest
{
    [Required]
    public string? OperationId { get; init; }

    [Required(AllowEmptyStrings = true)]
    public string? InputA { get; init; }

    [Required(AllowEmptyStrings = true)]
    public string? InputB { get; init; }
}
