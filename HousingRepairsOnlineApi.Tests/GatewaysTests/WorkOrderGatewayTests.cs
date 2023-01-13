using System;
using System.Net;
using System.Threading.Tasks;
using FluentAssertions;
using HousingRepairsOnlineApi.Gateways;
using Newtonsoft.Json;
using RichardSzalay.MockHttp;
using Xunit;

namespace HousingRepairsOnlineApi.Tests.GatewaysTests;

public class WorkOrderGatewayTests : IDisposable
{
    private readonly WorkOrderGateway systemUnderTest;
    private readonly MockHttpMessageHandler mockHttp;
    private const string authenticationIdentifier = "super secret";
    private const string SchedulingApiEndpoint = "https://our-proxy-scheduling.api";
    private const string Priority = "priority";

    public WorkOrderGatewayTests()
    {
        mockHttp = new MockHttpMessageHandler();
        var httpClient = mockHttp.ToHttpClient();
        httpClient.BaseAddress = new Uri(SchedulingApiEndpoint);

        systemUnderTest = new WorkOrderGateway(httpClient, authenticationIdentifier);
    }

    [Fact]
    public async Task GivenApiReturnsEmptyResponse_WhenCreatingWorkOrder_EmptyResponseIsReturned()
    {
        // Arrange
        const string SorCode = "SOR Code";
        const string LocationId = "Location ID";
        const string Description = "Description text";

        mockHttp.Expect($"/WorkOrder/CreateWorkOrder?locationId={LocationId}&sorCode={SorCode}")
            .Respond("application/json", JsonConvert.SerializeObject(""));

        // Act
        var data = await systemUnderTest.CreateWorkOrder(LocationId, SorCode, Description);

        // Assert
        Assert.Empty(data);
        mockHttp.VerifyNoOutstandingExpectation();
    }

    [Fact]
    public async Task GivenApiReturnsNonEmptyResponse_WhenCreatingWorkOrder_ThenNonEmptyResponseIsReturned()
    {
        // Arrange
        const string SorCode = "SOR Code";
        const string LocationId = "Location ID";
        const string Description = "Description text";

        var expected = "XXX";

        mockHttp.Expect($"/WorkOrder/CreateWorkOrder?locationId={LocationId}&sorCode={SorCode}")
            .Respond($"application/json", JsonConvert.SerializeObject(expected));

        // Act
        var data = await systemUnderTest.CreateWorkOrder(LocationId, SorCode, Description);

        // Assert
        data.Should().BeEquivalentTo(expected);
        mockHttp.VerifyNoOutstandingExpectation();
    }

    [Fact]
    public async Task GivenValidParameters_WhenBookingAppointment_NoExceptionIsThrown()
    {
        // Arrange
        const string SorCode = "SOR Code";
        const string LocationId = "Location ID";
        const string Description = "Description text";
        mockHttp.Expect(
                $"/WorkOrder/CreateWorkOrder?locationId={LocationId}&sorCode={SorCode}").WithContent("{\"description\":\"Description text\"}")
            .Respond(HttpStatusCode.OK);

        // Act
        var act = async () =>
        {
            await systemUnderTest.CreateWorkOrder(LocationId, SorCode, Description);
        };

        // Assert
        await act.Should().NotThrowAsync<Exception>();
        mockHttp.VerifyNoOutstandingExpectation();
    }

    public void Dispose()
    {
        mockHttp.Dispose();
    }
}

