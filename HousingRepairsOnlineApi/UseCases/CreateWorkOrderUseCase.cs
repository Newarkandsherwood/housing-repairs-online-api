﻿using System.Threading.Tasks;
using Ardalis.GuardClauses;
using HousingRepairsOnlineApi.Domain;
using HousingRepairsOnlineApi.Gateways;
using HousingRepairsOnlineApi.Helpers;

namespace HousingRepairsOnlineApi.UseCases;

public class CreateWorkOrderUseCase : ICreateWorkOrderUseCase
{
    private readonly IWorkOrderGateway workOrderGateway;

    public CreateWorkOrderUseCase(IWorkOrderGateway workOrderGateway)
    {
        this.workOrderGateway = workOrderGateway;
        // this.sorEngineResolver = sorEngineResolver;
    }

    public Task<string> Execute(string locationId, string sorCode, string description)
    {
        Guard.Against.NullOrWhiteSpace(locationId, nameof(locationId));
        Guard.Against.NullOrWhiteSpace(sorCode, nameof(sorCode));
        Guard.Against.NullOrWhiteSpace(description, nameof(description));

        return workOrderGateway.CreateWorkOrder(locationId, sorCode, description);
    }
    public Task<string> Execute(string repairType, RepairRequest repairRequest)
    {
        Guard.Against.NullOrWhiteSpace(repairType, nameof(repairType));
        Guard.Against.InvalidInput(repairType, nameof(repairType), RepairType.IsValidValue);
        Guard.Against.NullOrWhiteSpace(repairRequest.Address.LocationId, nameof(repairRequest.Address.LocationId));
        Guard.Against.NullOrWhiteSpace(repairRequest.Location.Value, nameof(repairRequest.Location.Value));
        Guard.Against.NullOrWhiteSpace(repairRequest.Problem.Value, nameof(repairRequest.Problem.Value));
        Guard.Against.NullOrWhiteSpace(repairRequest.Description.Text, nameof(repairRequest.Description.Text));

        return workOrderGateway.CreateWorkOrder(repairRequest.Address.LocationId, "sorCode", repairRequest.Description.Text);


        return null;
    }

}
