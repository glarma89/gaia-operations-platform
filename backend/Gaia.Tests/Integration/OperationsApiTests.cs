
using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc.Testing;

namespace Gaia.Tests.Integration;

[Collection("DatabaseIntegration")]
public sealed class OperationsApiTests
{
    private readonly HttpClient _client;

    public OperationsApiTests(
    GaiaApiFactory factory)
    {
        _client = factory.CreateClient();
    }


    [Fact]
    public async Task GetHistory_MonthlyCount_IncludesAllExecutions()
    {
        // Arrange
        for (int i = 1; i <= 5; i++)
        {
            var request = new
            {
                operationId = "divide",
                inputA = (i * 10).ToString(),
                inputB = "2"
            };

            var response = await _client.PostAsJsonAsync(
                "/api/operations/execute",
                request);

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        // Act
        var historyResponse = await _client.GetAsync(
            "/api/operations/divide/history");

        // Assert
        Assert.Equal(HttpStatusCode.OK, historyResponse.StatusCode);

        using var document = JsonDocument.Parse(
            await historyResponse.Content.ReadAsStringAsync());

        var root = document.RootElement;

        Assert.Equal(
            "divide",
            root.GetProperty("operationId").GetString());

        Assert.Equal(
            3,
            root.GetProperty("latestExecutions").GetArrayLength());

        Assert.Equal(
            5,
            root.GetProperty("monthlyCount").GetInt32());
    }


    [Fact]
    public async Task GetHistory_FiltersByOperationType()
    {
        // Arrange
        var concatRequest = new
        {
            operationId = "concat",
            inputA = "Filter",
            inputB = "Test"
        };

        var equalsRequest = new
        {
            operationId = "equals",
            inputA = "10",
            inputB = "10"
        };

        var concatResponse = await _client.PostAsJsonAsync(
            "/api/operations/execute",
            concatRequest);

        var equalsResponse = await _client.PostAsJsonAsync(
            "/api/operations/execute",
            equalsRequest);

        Assert.Equal(HttpStatusCode.OK, concatResponse.StatusCode);
        Assert.Equal(HttpStatusCode.OK, equalsResponse.StatusCode);

        // Act
        var historyResponse = await _client.GetAsync(
            "/api/operations/concat/history");

        // Assert
        Assert.Equal(HttpStatusCode.OK, historyResponse.StatusCode);

        using var document = JsonDocument.Parse(
            await historyResponse.Content.ReadAsStringAsync());

        var root = document.RootElement;

        Assert.Equal(
            "concat",
            root.GetProperty("operationId").GetString());

        var executions = root.GetProperty("latestExecutions");

        Assert.Contains(
            executions.EnumerateArray(),
            x => x.GetProperty("inputA").GetString() == "Filter"
                && x.GetProperty("inputB").GetString() == "Test");

        Assert.DoesNotContain(
            executions.EnumerateArray(),
            x => x.GetProperty("inputA").GetString() == "10"
                && x.GetProperty("inputB").GetString() == "10");
    }


    [Fact]
    public async Task GetHistory_MoreThanThreeExecutions_ReturnsOnlyThree()
    {
        // Arrange
        for (int i = 1; i <= 5; i++)
        {
            var request = new
            {
                operationId = "subtract",
                inputA = i.ToString(),
                inputB = "1"
            };

            var response = await _client.PostAsJsonAsync(
                "/api/operations/execute",
                request);

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        // Act
        var historyResponse = await _client.GetAsync(
            "/api/operations/subtract/history");

        // Assert
        Assert.Equal(HttpStatusCode.OK, historyResponse.StatusCode);

        using var document = JsonDocument.Parse(
            await historyResponse.Content.ReadAsStringAsync());

        var root = document.RootElement;
        var executions = root.GetProperty("latestExecutions");

        Assert.Equal(3, executions.GetArrayLength());

        Assert.Equal(
            "5",
            executions[0].GetProperty("inputA").GetString());

        Assert.Equal(
            "4",
            executions[1].GetProperty("inputA").GetString());

        Assert.Equal(
            "3",
            executions[2].GetProperty("inputA").GetString());
    }


    [Fact]
    public async Task GetHistory_UnknownOperation_ReturnsNotFound()
    {
        // Act
        var response = await _client.GetAsync(
            "/api/operations/unknown/history");

        // Assert
        Assert.Equal(
            HttpStatusCode.NotFound,
            response.StatusCode);
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


    [Fact]
    public async Task GetHistory_AfterExecute_ReturnsExecutionAndMonthlyCount()
    {
        // Arrange
        var request = new
        {
            operationId = "multiply",
            inputA = "8",
            inputB = "9"
        };

        // Act
        var executeResponse = await _client.PostAsJsonAsync(
            "/api/operations/execute",
            request);

        // Assert
        Assert.Equal(HttpStatusCode.OK, executeResponse.StatusCode);

        var historyResponse = await _client.GetAsync(
            "/api/operations/multiply/history");

        Assert.Equal(HttpStatusCode.OK, historyResponse.StatusCode);

        using var document = JsonDocument.Parse(
            await historyResponse.Content.ReadAsStringAsync());

        var root = document.RootElement;

        Assert.Equal(
            "multiply",
            root.GetProperty("operationId").GetString());

        var executions = root.GetProperty("latestExecutions");

        Assert.True(executions.GetArrayLength() >= 1);
        Assert.True(executions.GetArrayLength() <= 3);

        var latest = executions[0];

        Assert.Equal(
            "8",
            latest.GetProperty("inputA").GetString());

        Assert.Equal(
            "9",
            latest.GetProperty("inputB").GetString());

        Assert.Equal(
            72,
            latest.GetProperty("result").GetInt32());

        Assert.True(
            root.GetProperty("monthlyCount").GetInt32() >= 1);
    }


    private sealed class OperationDto
    {
        public string Id { get; set; } = string.Empty;
    }
}
