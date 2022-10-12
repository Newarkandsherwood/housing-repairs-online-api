using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;
using HACT.Dtos;
using HousingRepairsOnline.Authentication.Helpers;
using HousingRepairsOnlineApi.Domain;
using Newtonsoft.Json;

namespace HousingRepairsOnlineApi.Gateways
{
    public class AppointmentsGateway : IAppointmentsGateway
    {
        private readonly HttpClient httpClient;
        private readonly string authenticationIdentifier;

        public AppointmentsGateway(HttpClient httpClient, string authenticationIdentifier)
        {
            this.httpClient = httpClient;
            this.authenticationIdentifier = authenticationIdentifier;
        }

        public async Task<IEnumerable<Appointment>> GetAvailableAppointments(string sorCode, string locationId, DateTime? fromDate = null, IEnumerable<AppointmentSlotTimeSpan> allowedAppointmentSlots = default)
        {
            var request = new HttpRequestMessage(HttpMethod.Get,
                $"/Appointments/AvailableAppointments?sorCode={sorCode}&locationId={locationId}&fromDate={fromDate}")
            {
                Content = JsonContent.Create(allowedAppointmentSlots),
            };

            request.SetupJwtAuthentication(httpClient, authenticationIdentifier);

            var response = await httpClient.SendAsync(request);

            var result = Enumerable.Empty<Appointment>();
            if (response.StatusCode == HttpStatusCode.OK)
            {
                result = await response.Content.ReadFromJsonAsync<List<Appointment>>();
            }

            return result;
        }

        public async Task BookAppointment(string bookingReference, string sorCode, string locationId, DateTime startDateTime,
            DateTime endDateTime, string repairDescriptionText)
        {
            var request = new HttpRequestMessage(HttpMethod.Post,
                $"/Appointments/BookAppointment?bookingReference={bookingReference}&sorCode={sorCode}&locationId={locationId}&startDateTime={startDateTime}&endDateTime={endDateTime}");

            var json = JsonConvert.SerializeObject(new { Text = repairDescriptionText });

            var stringContent = new StringContent(json, Encoding.UTF8, "application/json");

            request.Content = stringContent;

            request.SetupJwtAuthentication(httpClient, authenticationIdentifier);

            await httpClient.SendAsync(request);

        }
    }
}
