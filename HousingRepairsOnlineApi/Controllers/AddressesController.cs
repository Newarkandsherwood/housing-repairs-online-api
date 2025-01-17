﻿using System;
using System.Threading.Tasks;
using HousingRepairsOnlineApi.Helpers;
using HousingRepairsOnlineApi.UseCases;
using Microsoft.AspNetCore.Mvc;
using Sentry;

namespace HousingRepairsOnlineApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AddressesController : ControllerBase
    {
        private readonly IRetrieveAddressesUseCase retrieveAddressesUseCase;

        public AddressesController(IRetrieveAddressesUseCase retrieveAddressesUseCase)
        {
            this.retrieveAddressesUseCase = retrieveAddressesUseCase;
        }

        [HttpGet]
        [Route("TenantAddresses")]
        public async Task<IActionResult> GetTenantAddresses([FromQuery] string postcode)
        {
            try
            {
                var result = await retrieveAddressesUseCase.Execute(postcode, RepairType.Tenant);
                return Ok(result);
            }
            catch (Exception ex)
            {
                SentrySdk.CaptureException(ex);
                return StatusCode(500, ex.Message);
            }
        }

        [HttpGet]
        [Route("CommunalAddresses")]
        public async Task<IActionResult> GetCommunalAddresses([FromQuery] string postcode)
        {
            try
            {
                var result = await retrieveAddressesUseCase.Execute(postcode, RepairType.Communal);
                return Ok(result);
            }
            catch (Exception ex)
            {
                SentrySdk.CaptureException(ex);
                return StatusCode(500, ex.Message);
            }
        }

        [HttpGet]
        [Route("LeaseholdAddresses")]
        public async Task<IActionResult> GetLeaseholdAddresses([FromQuery] string postcode)
        {
            try
            {
                var result = await retrieveAddressesUseCase.Execute(postcode, RepairType.Leasehold);
                return Ok(result);
            }
            catch (Exception ex)
            {
                SentrySdk.CaptureException(ex);
                return StatusCode(500, ex.Message);
            }
        }
    }
}
