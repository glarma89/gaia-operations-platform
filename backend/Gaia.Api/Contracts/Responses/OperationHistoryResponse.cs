
using System.Text.Json;

namespace Gaia.Api.Contracts.Responses;

public sealed record OperationHistoryResponse(
    string OperationId,
    IReadOnlyList<OperationHistoryItemResponse> LatestExecutions,
    int MonthlyCount);

public sealed record OperationHistoryItemResponse(
    string InputA,
    string InputB,
    JsonElement Result,
    DateTime ExecutedAtUtc);
