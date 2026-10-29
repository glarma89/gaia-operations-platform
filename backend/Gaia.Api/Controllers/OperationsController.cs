
using Gaia.Api.Contracts.Requests;
using Gaia.Application.Services;
using Gaia.Domain.Models;
using Microsoft.AspNetCore.Mvc;

namespace Gaia.Api.Controllers;

[ApiController]
[Route("api/operations")]
public sealed class OperationsController : ControllerBase
{
    private readonly OperationService _service;

    public OperationsController(OperationService service)
    {
        _service = service;
    }

    [HttpGet]
    public ActionResult<IReadOnlyCollection<OperationDefinition>> GetOperations()
    {
        var operations = _service.GetAvailableOperations();

        return Ok(operations);
    }

    [HttpPost("execute")]
    public ActionResult<OperationResult> Execute(
        [FromBody] ExecuteOperationRequest request)
    {
        var result = _service.Execute(
            request.OperationId!,
            request.InputA!,
            request.InputB!
        );

        return Ok(result);
    }
}
