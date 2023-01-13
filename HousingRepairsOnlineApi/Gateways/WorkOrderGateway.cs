using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;
using Ardalis.GuardClauses;
using HACT.Dtos;
using HousingRepairsOnline.Authentication.Helpers;
using Newtonsoft.Json;

namespace HousingRepairsOnlineApi.Gateways;

public class WorkOrderGateway : IWorkOrderGateway
{
    private readonly HttpClient httpClient;
    private readonly string authenticationIdentifier;

    public WorkOrderGateway(HttpClient httpClient, string authenticationIdentifier)
    {
        this.httpClient = httpClient;
        this.authenticationIdentifier = authenticationIdentifier;
    }
    public async Task<string> CreateWorkOrder(string locationId, string sorCode, string description)
    {

        var request = new HttpRequestMessage(HttpMethod.Post,
            $"/WorkOrder/CreateWorkOrder?locationId={locationId}&sorCode={sorCode}");

        var stringContent = new StringContent($"\"{description}\"", Encoding.UTF8, "application/json");

        request.Content = stringContent;

        request.SetupJwtAuthentication(httpClient, authenticationIdentifier);

        var response = await httpClient.SendAsync(request);

        var data = response.Content.ReadAsStringAsync();

        return await data;
    }

}
