using System.Net;
using System.Net.Http.Json;
using FinTech.Application.Accounts.Commands.OpenAccount;
using FinTech.Application.Accounts.Queries.GetAccountById;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
namespace FinTech.IntegrationTests;

public class TransferEndpointTests : IClassFixture<FinTechApiFactory>
{
    private readonly HttpClient _client;

    public TransferEndpointTests(FinTechApiFactory factory)
        => _client = factory.CreateClient();

    [Fact]
    public async Task Transfer_ShouldMoveFundsBetweenAccounts_EndToEnd()
    {
        // Arrange
        var originResponse = await _client.PostAsJsonAsync("api/accounts",
            new OpenAccountCommand("Origin Account", "95671266076", "CPF", "BRL")); // CPF generated using 4devs.com.br

        originResponse.StatusCode.Should().Be(HttpStatusCode.Created);
        var originId = (await originResponse.Content.ReadFromJsonAsync<CreatedResponse>())!.Id;

        var destResponse = await _client.PostAsJsonAsync("api/accounts",
            new OpenAccountCommand("Destination Account", "42882379048", "CPF", "BRL"));

        destResponse.StatusCode.Should().Be(HttpStatusCode.Created);
        var destId = (await destResponse.Content.ReadFromJsonAsync<CreatedResponse>())!.Id;

        // Fund the origin account
        var depositRequest = new HttpRequestMessage(HttpMethod.Post, $"api/accounts/{originId}/deposit")
        {
            Content = JsonContent.Create(new { Amount = 500m, Currency = "BRL" })
        };
        depositRequest.Headers.Add("Idempotency-Key", Guid.NewGuid().ToString());
        var depositResponse = await _client.SendAsync(depositRequest);
        depositResponse.StatusCode.Should().Be(HttpStatusCode.NoContent);

        // Act
        var transferRequest = new HttpRequestMessage(HttpMethod.Post, "api/transactions/transfer")
        {
            Content = JsonContent.Create(new
            {
                OriginAccountId = originId,
                DestinationAccountId = destId,
                Amount = 200m,
                Currency = "BRL",
                Reference = "Integration test transfer"
            })
        };
        transferRequest.Headers.Add("Idempotency-Key", Guid.NewGuid().ToString());
        var transferResponse = await _client.SendAsync(transferRequest);
        transferResponse.StatusCode.Should().Be(HttpStatusCode.Created);

        // Assert
        var origin = await _client.GetFromJsonAsync<AccountDto>("api/accounts/{originId}");
        var destination = await _client.GetFromJsonAsync<AccountDto>("api/accounts/{destId}");

        origin!.Balance.Should().Be(300m);
        destination!.Balance.Should().Be(200m);
    }

    [Fact]
    public async Task Transfer_ShouldReturnBadRequest_WhenIdempotencyKeyIsMissing()
    {
        var response = await _client.PostAsJsonAsync("api/transactions/transfer", new
        {
            OriginAccountId = Guid.NewGuid(),
            DestinationAccountId = Guid.NewGuid(),
            Amount = 100m,
            Currency = "BRL",
            Reference = "No Idempotency Key"
        });

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }


    private record CreatedResponse(Guid Id);
}


