using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FluentAssertions;
using HousingRepairsOnlineApi.Gateways;
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
        private readonly BookAvailableAppointmentUseCase systemUnderTest;

        public BookAvailableAppointmentUseCaseTests()
        {
            appointmentsGatewayMock = new Mock<IAppointmentsGateway>();
            systemUnderTest = new BookAvailableAppointmentUseCase(appointmentsGatewayMock.Object);
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
    }
}
