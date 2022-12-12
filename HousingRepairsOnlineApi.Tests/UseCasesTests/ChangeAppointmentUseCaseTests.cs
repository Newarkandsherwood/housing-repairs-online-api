using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FluentAssertions;
using HousingRepairsOnlineApi.Gateways;
using HousingRepairsOnlineApi.Helpers;
using HousingRepairsOnlineApi.UseCases;
using Moq;
using Xunit;

namespace HousingRepairsOnlineApi.Tests.UseCasesTests
{
    public class ChangeAppointmentUseCaseTests
    {
        private const string BookingReference = "bookingReference";

        private readonly Mock<IAppointmentsGateway> appointmentsGatewayMock;
        private readonly ChangeAppointmentUseCase systemUnderTest;

        public ChangeAppointmentUseCaseTests()
        {
            appointmentsGatewayMock = new Mock<IAppointmentsGateway>();
            systemUnderTest = new ChangeAppointmentUseCase(appointmentsGatewayMock.Object);
        }

        public static IEnumerable<object[]> InvalidArgumentTestData()
        {
            yield return new object[] { new ArgumentNullException(), null };
            yield return new object[] { new ArgumentException(), "" };
            yield return new object[] { new ArgumentException(), " " };
        }

        [Theory]
        [MemberData(nameof(InvalidArgumentTestData))]
#pragma warning disable xUnit1026
        public async void GivenAnInvalidBookingRef_WhenExecuting_ThenExceptionIsThrown<T>(T exception,
            string bookingRef) where T : Exception
#pragma warning restore xUnit1026
        {
            //Act
            Func<Task> act = async () => await systemUnderTest.Execute(
                bookingRef, It.IsAny<DateTime>(), It.IsAny<DateTime>()
            );

            //Assert
            await act.Should().ThrowExactlyAsync<T>();
        }

        [Fact]
        public async void GivenAStartDateTimeLaterThanEndDateTime_WhenExecuting_ThenExceptionIsThrow()
        {
            // Arrange
            var startDateTime = new DateTime(2022, 1, 1);
            var endDateTime = startDateTime.AddDays(-1);

            // Act
            Func<Task> act = async () =>
            {
                await systemUnderTest.Execute(
                    BookingReference, startDateTime, endDateTime
                );
            };

            // Assert
            await act.Should().ThrowExactlyAsync<ArgumentOutOfRangeException>();
        }

        [Fact]
        public async void GivenAValidBookingRef_WhenExecuting_ThenChangeAppointmentIsCalled()
        {
            // Arrange
            var startDateTime = new DateTime(2022, 1, 1);
            var endDateTime = startDateTime.AddDays(1);
            appointmentsGatewayMock.Setup(x => x.ChangeAppointment(BookingReference, startDateTime, endDateTime))
                .ReturnsAsync(ChangeAppointmentStatus.Found);
            // Act
            var act = await systemUnderTest.Execute(
                    BookingReference, startDateTime, endDateTime
                );


            // Assert
            appointmentsGatewayMock.Verify(x => x.ChangeAppointment(BookingReference, startDateTime, endDateTime), Times.Once);
        }
    }
}
