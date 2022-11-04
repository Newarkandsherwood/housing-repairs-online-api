﻿using System;
using System.Threading.Tasks;
using HousingRepairsOnlineApi.Domain;
using HousingRepairsOnlineApi.Helpers;
using HousingRepairsOnlineApi.UseCases;
using Microsoft.AspNetCore.Mvc;
using Sentry;

namespace HousingRepairsOnlineApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class RepairController : ControllerBase
    {
        private readonly ISaveRepairRequestUseCase saveRepairRequestUseCase;
        private readonly IAppointmentConfirmationSender appointmentConfirmationSender;
        private readonly IBookAppointmentUseCase bookAppointmentUseCase;
        private readonly IInternalEmailSender internalEmailSender;
        private readonly IRetrieveRepairsUseCase retrieveRepairsUseCase;
        private readonly IRetrieveAvailableAppointmentsUseCase retrieveAvailableAppointmentsUseCase;
        private readonly IBookAvailableAppointmentUseCase bookAvailableAppointmentUseCase;

        public RepairController(
            ISaveRepairRequestUseCase saveRepairRequestUseCase,
            IInternalEmailSender internalEmailSender,
            IAppointmentConfirmationSender appointmentConfirmationSender,
            IBookAppointmentUseCase bookAppointmentUseCase,
            IRetrieveRepairsUseCase retrieveRepairsUseCase,
            IRetrieveAvailableAppointmentsUseCase retrieveAvailableAppointmentsUseCase,
            IBookAvailableAppointmentUseCase bookAvailableAppointmentUseCase)
        {
            this.saveRepairRequestUseCase = saveRepairRequestUseCase;
            this.internalEmailSender = internalEmailSender;
            this.appointmentConfirmationSender = appointmentConfirmationSender;
            this.bookAppointmentUseCase = bookAppointmentUseCase;
            this.retrieveRepairsUseCase = retrieveRepairsUseCase;
            this.retrieveAvailableAppointmentsUseCase = retrieveAvailableAppointmentsUseCase;
            this.bookAvailableAppointmentUseCase = bookAvailableAppointmentUseCase;
        }

        [HttpGet]
        [Route("CommunalPropertyRepairs")]
        public async Task<IActionResult> CommunalPropertyRepairs([FromQuery] string propertyReference)
        {
            try
            {
                var result = await retrieveRepairsUseCase.Execute(RepairType.Communal, propertyReference);
                return Ok(result);
            }
            catch (Exception ex)
            {
                SentrySdk.CaptureException(ex);
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPost]
        [Route("TenantRepair")]
        public async Task<IActionResult> TenantRepair([FromBody] RepairRequest repairRequest)
        {
            return await SaveRepair(RepairType.Tenant, repairRequest, bookAppointmentUseCase.Execute);
        }

        [HttpPost]
        [Route("CommunalRepair")]
        public async Task<IActionResult> CommunalRepair([FromBody] RepairRequest repairRequest)
        {
            return await SaveRepair(RepairType.Communal, repairRequest, BookCommunalAppointment);

            Task BookCommunalAppointment(string bookingReference, string sorCode, string priority, string locationId,
                DateTime startDateTime, DateTime endDateTime, string repairDescriptionText) =>
                bookAvailableAppointmentUseCase.Execute(bookingReference, sorCode, priority, locationId, repairDescriptionText);
        }

        internal async Task<IActionResult> SaveRepair(string repairType, RepairRequest repairRequest, Func<string, string, string, string, DateTime, DateTime, string, Task> bookAppointment)
        {
            try
            {
                var result = await saveRepairRequestUseCase.Execute(repairType, repairRequest);

                await bookAppointment(result.Id, result.SOR, result.Priority, result.Address.LocationId,
                    result.Time.StartDateTime, result.Time.EndDateTime, result.Description.Text);

                appointmentConfirmationSender.Execute(result);
                await internalEmailSender.Execute(result);
                return Ok(result.Id);
            }
            catch (Exception ex)
            {
                SentrySdk.CaptureException(ex);
                return StatusCode(500, ex.Message);
            }
        }
    }
}
