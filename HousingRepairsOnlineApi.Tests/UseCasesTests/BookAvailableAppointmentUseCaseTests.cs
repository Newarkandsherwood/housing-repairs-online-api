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
        private const string Description = "Description";
        private const string Location = "Location";
        private const string LongDescription =
            "Lorem ipsum dolor sit amet, consectetuer adipiscing elit. Aenean commodo ligula eget dolor. Aenean massa. Cum sociis natoque penatibus et magnis dis parturient montes, nascetur ridiculus mus. Donec quam felis, ultricies nec, pellentesque eu, pretium quis, sem. Nulla consequat massa quis enim. Donec pede justo, fringilla vel, aliquet nec, vulputate eget, arcu. In enim justo, rhoncus ut, imperdiet a, venenatis vitae, justo. Nullam dictum felis eu pede mollis pretium. Integer tincidunt. Cras dapibus. Vivamus elementum semper nisi. Aenean vulputate eleifend tellus. Aenean leo ligula, porttitor eu, consequat vitae, eleifend ac, enim. Aliquam lorem ante, dapibus in, viverra quis, feugiat a, tellus. Phasellus viverra nulla ut metus varius laoreet. Quisque rutrum. Aenean imperdiet. Etiam ultricies nisi vel augue. Curabitur ullamcorper ultricies nisi. Nam eget dui. Etiam rhoncus. Maecenas tempus, tellus eget condimentum rhoncus, sem quam semper libero, sit amet adipiscing sem neque sed ipsum. Nam quam nunc, blandit vel, luctus pulvinar, hendrerit id, lorem. Maecenas nec odio et ante tincidunt tempus. Donec vitae sapien ut libero venenatis faucibus. Nullam quis ante. Etiam sit amet orci eget eros faucibus tincidunt. Duis leo. Sed fringilla mauris sit amet nibh. Donec sodales sagittis magna. Sed consequat, leo eget bibendum sodales, augue velit cursus nunc, quis gravida magna mi a libero. Fusce vulputate eleifend sapien. Vestibulum purus quam, scelerisque ut, mollis sed, nonummy id, metus. Nullam accumsan lorem in dui. Cras ultricies mi eu turpis hendrerit fringilla. Vestibulum ante ipsum primis in faucibus orci luctus et ultrices posuere cubilia Curae; In ac dui quis mi consectetuer lacinia. Nam pretium turpis et";

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
        [InlineData("", SorCode, Priority, LocationId, Description, Location)]
        [InlineData(BookingReference, "", Priority, LocationId, Description, Location)]
        [InlineData(BookingReference, SorCode, "", LocationId, Description, Location)]
        [InlineData(BookingReference, SorCode, Priority, "", Description, Location)]
        [InlineData(BookingReference, SorCode, Priority, LocationId, "", Location)]
        [InlineData(BookingReference, SorCode, Priority, LocationId, Description, "")]
        public async void GivenAnEmptyParameter_WhenExecuting_ThenExceptionIsThrow(string bookingReference, string sorCode, string priority, string locationId, string repairDescriptionText, string repairDescriptionLocation)
        {

            // Act
            Func<Task> act = async () =>
            {
                await systemUnderTest.Execute(
                    bookingReference, sorCode, priority, locationId, repairDescriptionText, repairDescriptionLocation
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
            await systemUnderTest.Execute(BookingReference, SorCode, Priority, LocationId, Description, Location);

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
            await systemUnderTest.Execute(BookingReference, SorCode, Priority, LocationId, Description, Location);

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
            await systemUnderTest.Execute(BookingReference, SorCode, Priority, LocationId, "description", "location");

            // Assert
            appointmentsGatewayMock.Verify(x => x.BookAppointment(BookingReference, SorCode, Priority, LocationId, firstAppointmentEarliestArrivalTime, firstAppointmentLatestArrivalTime, "description location"), Times.Once);
        }

        [Fact]
        public async void GivenAnNoAppointments_WhenExecute_ThenExceptionIsThrown()
        {
            appointmentsGatewayMock.Setup(x => x.GetAvailableAppointments(SorCode, Priority, LocationId, null,
                    It.IsAny<IEnumerable<AppointmentSlotTimeSpan>>()))
                .ReturnsAsync(Array.Empty<Appointment>());
            var appointmentSlotTimeSpans = new[]
            {
                new AppointmentSlotTimeSpan { StartTime = new TimeSpan(8, 0, 0), EndTime = new TimeSpan(12, 0, 0), }
            };
            appointmentSlotsFilterMock.Setup(x => x.Filter()).Returns(appointmentSlotTimeSpans);

            // Act
            var act = async () => await systemUnderTest.Execute(BookingReference, SorCode, Priority, LocationId, Description, Location);

            //Assert
            await act.Should().ThrowExactlyAsync<InvalidOperationException>();
        }

        [Fact]
        public async void GivenDescriptionLengthOver255Characters_WhenExecuting_ThenExceptionIsThrown()
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
            var act = async () => await systemUnderTest.Execute(BookingReference, SorCode, Priority, LocationId, LongDescription, Location);

            // Assert
            await act.Should().ThrowExactlyAsync<Exception>();        }

    }
}
