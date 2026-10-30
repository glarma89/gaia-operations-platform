
using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc.Testing;

namespace Gaia.Tests.Integration;

public sealed class OperationsApiTests
    : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly HttpClient _client;

    public OperationsApiTests(
        WebApplicationFactory<Program> factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task GetOperations_ReturnsSixOperations()
    {
        var response = await _client.GetAsync("/api/operations");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        using var document = JsonDocument.Parse(
            await response.Content.ReadAsStringAsync());

        Assert.Equal(6, document.RootElement.GetArrayLength());
    }

    [Fact]
    public async Task GetOperations_ContainsMultiplyOperation()
    {
        var operations = await _client.GetFromJsonAsync<
            List<OperationDto>>("/api/operations");

        Assert.NotNull(operations);

        Assert.Contains(
            operations,
            operation => operation.Id == "multiply");
    }

    [Fact]
    public async Task Execute_Add_ReturnsCorrectResult()
    {
        var request = new
        {
            operationId = "add",
            inputA = "15",
            inputB = "25"
        };

        var response = await _client.PostAsJsonAsync(
            "/api/operations/execute", request);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        using var document = JsonDocument.Parse(
            await response.Content.ReadAsStringAsync());

        Assert.Equal(
            40,
            document.RootElement.GetProperty("value").GetDecimal());
    }

    [Fact]
    public async Task Execute_Multiply_ReturnsCorrectResult()
    {
        var request = new
        {
            operationId = "multiply",
            inputA = "6",
            inputB = "7"
        };

        var response = await _client.PostAsJsonAsync(
            "/api/operations/execute", request);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        using var document = JsonDocument.Parse(
            await response.Content.ReadAsStringAsync());

        Assert.Equal(
            42,
            document.RootElement.GetProperty("value").GetDecimal());
    }

    [Fact]
    public async Task Execute_Concat_ReturnsCombinedString()
    {
        var request = new
        {
            operationId = "concat",
            inputA = "Hello",
            inputB = "World"
        };

        var response = await _client.PostAsJsonAsync(
            "/api/operations/execute", request);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        using var document = JsonDocument.Parse(
            await response.Content.ReadAsStringAsync());

        Assert.Equal(
            "HelloWorld",
            document.RootElement.GetProperty("value").GetString());
    }

    [Fact]
    public async Task Execute_DivideByZero_ReturnsBadRequest()
    {
        var request = new
        {
            operationId = "divide",
            inputA = "10",
            inputB = "0"
        };

        var response = await _client.PostAsJsonAsync(
            "/api/operations/execute", request);

        Assert.Equal(
            HttpStatusCode.BadRequest,
            response.StatusCode);
    }

    [Fact]
    public async Task Execute_UnknownOperation_ReturnsNotFound()
    {
        var request = new
        {
            operationId = "power",
            inputA = "2",
            inputB = "3"
        };

        var response = await _client.PostAsJsonAsync(
            "/api/operations/execute", request);

        Assert.Equal(
            HttpStatusCode.NotFound,
            response.StatusCode);
    }

    [Fact]
    public async Task Execute_InvalidNumber_ReturnsBadRequest()
    {
        var request = new
        {
            operationId = "multiply",
            inputA = "abc",
            inputB = "5"
        };

        var response = await _client.PostAsJsonAsync(
            "/api/operations/execute", request);

        Assert.Equal(
            HttpStatusCode.BadRequest,
            response.StatusCode);
    }

    [Fact]
    public async Task Execute_MissingOperationId_ReturnsBadRequest()
    {
        var request = new
        {
            inputA = "10",
            inputB = "5"
        };

        var response = await _client.PostAsJsonAsync(
            "/api/operations/execute", request);

        Assert.Equal(
            HttpStatusCode.BadRequest,
            response.StatusCode);
    }

    private sealed class OperationDto
    {
        public string Id { get; set; } = string.Empty;
    }
}
