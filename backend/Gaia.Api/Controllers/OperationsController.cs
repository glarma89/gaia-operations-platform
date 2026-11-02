
using System.Text.Json;
using Gaia.Api.Contracts.Requests;
using Gaia.Application.Services;
using Gaia.Domain.Models;
using Gaia.Infrastructure.Persistence;
using Microsoft.AspNetCore.Mvc;
using Gaia.Api.Contracts.Responses;

namespace Gaia.Api.Controllers;

[ApiController]
[Route("api/operations")]
public sealed class OperationsController : ControllerBase
{
    private readonly OperationService _service;
    private readonly OperationHistoryRepository _historyRepository;

    public OperationsController(
        OperationService service,
        OperationHistoryRepository historyRepository)
    {
        _service = service;
        _historyRepository = historyRepository;
    }

    [HttpGet]
    public ActionResult<IReadOnlyCollection<OperationDefinition>> GetOperations()
    {
        var operations = _service.GetAvailableOperations();

        return Ok(operations);
    }

    [HttpPost("execute")]
    public async Task<ActionResult<OperationResult>> Execute(
        [FromBody] ExecuteOperationRequest request,
        CancellationToken cancellationToken)
    {
        var result = _service.Execute(
            request.OperationId!,
            request.InputA!,
            request.InputB!
        );

        var history = new OperationHistory
        {
            OperationId = result.OperationId,
            InputA = request.InputA!,
            InputB = request.InputB!,
            ResultJson = JsonSerializer.Serialize(result.Value),
            ExecutedAtUtc = DateTime.UtcNow
        };

        await _historyRepository.AddAsync(history, cancellationToken);

        return Ok(result);
    }


    [HttpGet("{operationId}/history")]
    public async Task<ActionResult<OperationHistoryResponse>> GetHistory(
        string operationId,
        CancellationToken cancellationToken)
    {
        var operation = _service.GetAvailableOperations()
            .FirstOrDefault(x =>
                string.Equals(
                    x.Id,
                    operationId,
                    StringComparison.OrdinalIgnoreCase));

        if (operation is null)
        {
            return NotFound(new
            {
                message = $"Operation '{operationId}' was not found."
            });
        }

        var now = DateTime.UtcNow;

        var monthStartUtc = new DateTime(
            now.Year,
            now.Month,
            1,
            0,
            0,
            0,
            DateTimeKind.Utc);

        var nextMonthStartUtc = monthStartUtc.AddMonths(1);

        var history = await _historyRepository.GetLatestAsync(
            operation.Id,
            3,
            cancellationToken);

        var monthlyCount = await _historyRepository.GetMonthlyCountAsync(
            operation.Id,
            monthStartUtc,
            nextMonthStartUtc,
            cancellationToken);

        var latestExecutions = history
            .Select(x => new OperationHistoryItemResponse(
                x.InputA,
                x.InputB,
                JsonSerializer.Deserialize<JsonElement>(x.ResultJson),
                x.ExecutedAtUtc))
            .ToList();

        return Ok(new OperationHistoryResponse(
            operation.Id,
            latestExecutions,
            monthlyCount));
    }

}
