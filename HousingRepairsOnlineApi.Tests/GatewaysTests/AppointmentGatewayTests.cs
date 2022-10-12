using System;
using System.Net;
using System.Net.Http.Json;
using System.Threading.Tasks;
using FluentAssertions;
using HACT.Dtos;
using HousingRepairsOnlineApi.Gateways;
using Newtonsoft.Json;
using RichardSzalay.MockHttp;
using Xunit;

namespace HousingRepairsOnlineApi.Tests.GatewaysTests
{
    public class AppointmentGatewayTests : IDisposable
    {
        private readonly AppointmentsGateway systemUnderTest;
        private readonly MockHttpMessageHandler mockHttp;
        private const string authenticationIdentifier = "super secret";
        private const string SchedulingApiEndpoint = "https://our-proxy-scheduling.api";

        public AppointmentGatewayTests()
        {
            mockHttp = new MockHttpMessageHandler();
            var httpClient = mockHttp.ToHttpClient();
            httpClient.BaseAddress = new Uri(SchedulingApiEndpoint);

            systemUnderTest = new AppointmentsGateway(httpClient, authenticationIdentifier);
        }

        [Fact]
        public async Task GivenApiReturnsEmptyResponse_WhenGettingAvailableAppointments_EmptyResponseIsReturned()
        {
            // Arrange
            const string SorCode = "SOR Code";
            const string LocationId = "Location ID";

            mockHttp.Expect($"/Appointments/AvailableAppointments?sorCode={SorCode}&locationId={LocationId}")
                .Respond("application/json", "[]");

            // Act
            var data = await systemUnderTest.GetAvailableAppointments(SorCode, LocationId);

            // Assert
            Assert.Empty(data);
            mockHttp.VerifyNoOutstandingExpectation();
        }

        [Fact]
        public async Task GivenApiReturnsNonEmptyResponse_WhenGettingAvailableAppointments_NonEmptyResponseIsReturned()
        {
            // Arrange
            const string SorCode = "SOR Code";
            const string LocationId = "Location ID";
            var year = 2021;
            var month = 01;
            var day = 01;
            var expected = new Appointment
            {
                Date = new DateTime(year, month, day),
                TimeOfDay = new TimeOfDay
                {
                    EarliestArrivalTime = new DateTime(year, month, day, 8, 0, 0),
                    LatestArrivalTime = new DateTime(year, month, day, 12, 0, 0),
                }
            };

            mockHttp.Expect($"/Appointments/AvailableAppointments?sorCode={SorCode}&locationId={LocationId}")
                .Respond($"application/json",
                    "[" + JsonConvert.SerializeObject(expected) + "]");

            // Act
            var data = await systemUnderTest.GetAvailableAppointments(SorCode, LocationId);

            // Assert
            data.Should().BeEquivalentTo(new[] { expected });
            mockHttp.VerifyNoOutstandingExpectation();
        }

        [Theory]
        [InlineData(HttpStatusCode.Unauthorized)]
        [InlineData(HttpStatusCode.ServiceUnavailable)]
        public async Task GivenApiRespondsWithResponseOtherThanOk_WhenGettingAvailableAppointments_EmptyResponseIsReturned(HttpStatusCode httpStatusCode)
        {
            // Arrange
            const string SorCode = "SOR Code";
            const string LocationId = "Location ID";

            mockHttp.Expect($"/Appointments/AvailableAppointments?sorCode={SorCode}&locationId={LocationId}")
                .Respond(httpStatusCode);

            // Act
            var data = await systemUnderTest.GetAvailableAppointments(SorCode, LocationId);

            // Assert
            Assert.Empty(data);
            mockHttp.VerifyNoOutstandingExpectation();
        }

        [Fact]
        public async Task GivenValidParameters_WhenBookingAppointment_NoExceptionIsThrown()
        {
            // Arrange
            const string SorCode = "SOR Code";
            const string LocationId = "Location ID";
            const string BookingReference = "Booking Reference";
            const string RepairDescriptionText = "Repair description text";
            var startDateTime = new DateTime(2022, 01, 01, 8, 0, 0);
            var endDateTime = new DateTime(2022, 01, 01, 12, 0, 0);
            mockHttp.Expect(
                    $"/Appointments/BookAppointment?bookingReference={BookingReference}&sorCode={SorCode}&locationId={LocationId}&startDateTime={startDateTime}&endDateTime={endDateTime}").With()
                .Respond(HttpStatusCode.OK);

            // Act
            Func<Task> act = async () =>
            {
                await systemUnderTest.BookAppointment(BookingReference, SorCode, LocationId, startDateTime, endDateTime, RepairDescriptionText);
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
}
