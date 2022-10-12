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
    public class BookAppointmentUseCaseTests
    {
        private const string BookingReference = "bookingReference";
        private const string SorCode = "sorCode";
        private const string LocationId = "locationId";

        private Mock<IAppointmentsGateway> appointmentsGatewayMock;
        private readonly BookAppointmentUseCase systemUnderTest;

        public BookAppointmentUseCaseTests()
        {
            appointmentsGatewayMock = new Mock<IAppointmentsGateway>();
            systemUnderTest = new BookAppointmentUseCase(appointmentsGatewayMock.Object);
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
                bookingRef, It.IsAny<string>(), It.IsAny<string>(), It.IsAny<DateTime>(), It.IsAny<DateTime>(), It.IsAny<string>()
            );

            //Assert
            await act.Should().ThrowExactlyAsync<T>();
        }

        [Theory]
        [MemberData(nameof(InvalidArgumentTestData))]
#pragma warning disable xUnit1026
        public async void GivenAnInvalidSorCode_WhenExecuting_ThenExceptionIsThrown<T>(T exception, string sorCode)
            where T : Exception
#pragma warning restore xUnit1026
        {
            //Act
            Func<Task> act = async () => await systemUnderTest.Execute(
                BookingReference, sorCode, It.IsAny<string>(), It.IsAny<DateTime>(), It.IsAny<DateTime>(), It.IsAny<string>()
            );

            //Assert
            await act.Should().ThrowExactlyAsync<T>();
        }

        [Theory]
        [MemberData(nameof(InvalidArgumentTestData))]
#pragma warning disable xUnit1026
        public async void GivenAnInvalidLocationId_WhenExecuting_ThenExceptionIsThrown<T>(T exception,
            string locationId) where T : Exception
#pragma warning restore xUnit1026
        {
            //Act
            Func<Task> act = async () => await systemUnderTest.Execute(
                BookingReference, SorCode, locationId, It.IsAny<DateTime>(), It.IsAny<DateTime>(), It.IsAny<string>()
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
                    BookingReference, SorCode, LocationId, startDateTime, endDateTime, It.IsAny<string>()
                );
            };

            // Assert
            await act.Should().ThrowExactlyAsync<ArgumentOutOfRangeException>();
        }
    }
}
