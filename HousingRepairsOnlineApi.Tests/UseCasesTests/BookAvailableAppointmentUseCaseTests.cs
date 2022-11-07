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

        [Theory]
        [InlineData("", SorCode, Priority, LocationId, "description")]
        [InlineData(BookingReference, "", Priority, LocationId, "description")]
        [InlineData(BookingReference, SorCode, "", LocationId, "description")]
        [InlineData(BookingReference, SorCode, Priority, "", "description")]
        [InlineData(BookingReference, SorCode, Priority, LocationId, "")]
        public async void GivenAnEmptyParameter_WhenExecuting_ThenExceptionIsThrow(string bookingReference, string sorCode, string priority, string locationId, string repairDescriptionText)
        {

            // Act
            Func<Task> act = async () =>
            {
                await systemUnderTest.Execute(
                    bookingReference, sorCode, priority, locationId, repairDescriptionText
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

        [Fact]
        public async void GivenMultipleAppointments_WhenExecuting_ThenBookAppointmentIsCalledWithFirstAppointment()
        {
            // Arrange
            var firstAppointmentEarliestArrivalTime = new DateTime(2022, 01, 01, 8, 0, 0);
            var firstAppointmentLatestArrivalTime = new DateTime(2022, 01, 01, 12, 0, 0);

            appointmentsGatewayMock.Setup(x => x.GetAvailableAppointments(SorCode, Priority, LocationId, null,
                    It.IsAny<IEnumerable<AppointmentSlotTimeSpan>>()))
                .ReturnsAsync(new[]
                {
                    new Appointment
                    {
                        TimeOfDay = new TimeOfDay
                        {
                            EarliestArrivalTime = firstAppointmentEarliestArrivalTime,
                            LatestArrivalTime = firstAppointmentLatestArrivalTime
                        }
                    },
                    new Appointment
                    {
                        TimeOfDay = new TimeOfDay
                        {
                            EarliestArrivalTime = new DateTime(2023, 01, 01, 8, 0, 0),
                            LatestArrivalTime = new DateTime(2023, 01, 01, 12, 0, 0)
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
            appointmentsGatewayMock.Verify(x => x.BookAppointment(BookingReference, SorCode, Priority, LocationId, firstAppointmentEarliestArrivalTime, firstAppointmentLatestArrivalTime,  "description"), Times.Once);
        }
    }
}
