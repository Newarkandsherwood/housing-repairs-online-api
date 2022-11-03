using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FluentAssertions;
using HACT.Dtos;
using HousingRepairsOnlineApi.Domain;
using HousingRepairsOnlineApi.Gateways;
using HousingRepairsOnlineApi.Helpers;
using HousingRepairsOnlineApi.UseCases;
using Moq;
using Xunit;

namespace HousingRepairsOnlineApi.Tests.UseCasesTests
{
    public class BookAvailableAppointmentUseCaseTests
    {
        private const string BookingReference = "bookingReference";
        private const string SorCode = "sorCode";
        private const string Priority = "priority";
        private const string LocationId = "locationId";

        private Mock<IAppointmentsGateway> appointmentsGatewayMock;
        private Mock<IAppointmentSlotsFilter> appointmentSlotsFilterMock;
        private readonly BookAvailableAppointmentUseCase systemUnderTest;

        public BookAvailableAppointmentUseCaseTests()
        {
            appointmentsGatewayMock = new Mock<IAppointmentsGateway>();
            appointmentSlotsFilterMock = new Mock<IAppointmentSlotsFilter>();
            systemUnderTest = new BookAvailableAppointmentUseCase(appointmentsGatewayMock.Object, appointmentSlotsFilterMock.Object);
        }

        [Fact]
        public async void GivenAnEmptyBookingReference_WhenExecuting_ThenExceptionIsThrow()
        {
            // Arrange
            var bookingRef = "";

            // Act
            Func<Task> act = async () =>
            {
                await systemUnderTest.Execute(
                    bookingRef, SorCode, Priority, LocationId, "description"
                );
            };

            // Assert
            await act.Should().ThrowExactlyAsync<ArgumentException>();
        }

        [Fact]
        public async void GivenAnAppointmentSlotsFilter_WhenExecuting_ThenAppointmentSlotsFilterIsCalled()
        {
            // Arrange
            appointmentsGatewayMock.Setup(x => x.GetAvailableAppointments(SorCode, Priority, LocationId, null,
                    It.IsAny<IEnumerable<AppointmentSlotTimeSpan>>()))
                .ReturnsAsync(new[]
                {
                    new Appointment
                    {
                        TimeOfDay = new TimeOfDay
                        {
                            EarliestArrivalTime = new DateTime(2022, 01, 01, 8, 0, 0),
                            LatestArrivalTime = new DateTime(2022, 01, 01, 12, 0, 0)
                        }
                    }
                });

            // Act
            await systemUnderTest.Execute(BookingReference, SorCode, Priority, LocationId, "description");

            // Assert
            appointmentSlotsFilterMock.Verify(x => x.Filter(), Times.Once);
        }


        [Fact]
        public async void GivenAnAppointmentSlotsFilter_WhenExecuting_ThenFilteredAppointmentSlotsAreUsed()
        {
            // Arrange
            appointmentsGatewayMock.Setup(x => x.GetAvailableAppointments(SorCode, Priority, LocationId, null,
                    It.IsAny<IEnumerable<AppointmentSlotTimeSpan>>()))
                .ReturnsAsync(new[]
                {
                    new Appointment
                    {
                        TimeOfDay = new TimeOfDay
                        {
                            EarliestArrivalTime = new DateTime(2022, 01, 01, 8, 0, 0),
                            LatestArrivalTime = new DateTime(2022, 01, 01, 12, 0, 0)
                        }
                    }
                });
            var appointmentSlotTimeSpans = new[]
            {
                new AppointmentSlotTimeSpan { StartTime = new TimeSpan(8, 0, 0), EndTime = new TimeSpan(12, 0, 0), }
            };
            appointmentSlotsFilterMock.Setup(x => x.Filter()).Returns(appointmentSlotTimeSpans);

            // Act
            await systemUnderTest.Execute(BookingReference, SorCode, Priority, LocationId, "description");

            // Assert
            appointmentsGatewayMock.Verify(x => x.GetAvailableAppointments(SorCode, Priority, LocationId, null, appointmentSlotTimeSpans), Times.Once);
        }
    }
}
