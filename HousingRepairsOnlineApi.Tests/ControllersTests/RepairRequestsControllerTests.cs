﻿using System;
using System.Threading.Tasks;
using FluentAssertions;
using HousingRepairsOnlineApi.Controllers;
using HousingRepairsOnlineApi.Domain;
using HousingRepairsOnlineApi.Helpers;
using HousingRepairsOnlineApi.UseCases;
using Moq;
using Xunit;

namespace HousingRepairsOnlineApi.Tests
{
    public class RepairRequestsControllerTests : ControllerTests
    {
        private RepairController systemUnderTest;
        private Mock<ISaveRepairRequestUseCase> saveRepairRequestUseCaseMock;
        private Mock<IRetrieveRepairsUseCase> retrieveRepairsUseCaseMock;
        private Mock<IBookAppointmentUseCase> bookAppointmentUseCaseMock;
        private Mock<IInternalEmailSender> internalEmailSenderMock;
        private Mock<IAppointmentConfirmationSender> appointmentConfirmationSender;
        private Mock<IRetrieveAvailableCommunalAppointmentUseCase> retrieveAvailableCommunalAppointmentUseCaseMock;

        private Mock<INotificationConfigurationResolver> sendNotificationResolver;
        private readonly string repairTypeArgument = RepairType.Tenant;

        private readonly RepairAvailability repairAvailability = new()
        {
            Display = "Displayed Time",
            StartDateTime = new DateTime(2022, 01, 01, 8, 0, 0),
            EndDateTime = new DateTime(2022, 01, 01, 12, 0, 0),
        };

        private readonly RepairAddress repairAddress = new()
        {
            LocationId = "Location Id",
        };

        public RepairRequestsControllerTests()
        {
            saveRepairRequestUseCaseMock = new Mock<ISaveRepairRequestUseCase>();
            bookAppointmentUseCaseMock = new Mock<IBookAppointmentUseCase>();
            appointmentConfirmationSender = new Mock<IAppointmentConfirmationSender>();
            internalEmailSenderMock = new Mock<IInternalEmailSender>();
            retrieveRepairsUseCaseMock = new Mock<IRetrieveRepairsUseCase>();
            retrieveAvailableCommunalAppointmentUseCaseMock = new Mock<IRetrieveAvailableCommunalAppointmentUseCase>();
            sendNotificationResolver = new Mock<INotificationConfigurationResolver>();
            systemUnderTest = new RepairController(saveRepairRequestUseCaseMock.Object, internalEmailSenderMock.Object, appointmentConfirmationSender.Object, bookAppointmentUseCaseMock.Object, retrieveRepairsUseCaseMock.Object, retrieveAvailableCommunalAppointmentUseCaseMock.Object);
        }

        [Fact]
        public async Task TestEndpoint()
        {
            var (repairRequest, repair) = CreateRepairRequestAndRepair();

            saveRepairRequestUseCaseMock.Setup(x => x.Execute(It.IsAny<string>(), It.IsAny<RepairRequest>())).ReturnsAsync(repair);

            var result = await systemUnderTest.SaveRepair(repairTypeArgument, repairRequest);

            GetStatusCode(result).Should().Be(200);

            saveRepairRequestUseCaseMock.Verify(x => x.Execute(It.IsAny<string>(), repairRequest), Times.Once);

            internalEmailSenderMock.Verify(x => x.Execute(repair), Times.Once);
        }

        [Fact]
        public async Task TestTenantEndpoint()
        {
            // Arrange
            var (repairRequest, repair) = CreateRepairRequestAndRepair();

            saveRepairRequestUseCaseMock.Setup(x => x.Execute(It.IsAny<string>(), It.IsAny<RepairRequest>())).ReturnsAsync(repair);

            // Act
            var result = await systemUnderTest.TenantRepair(repairRequest);

            // Assert
            GetStatusCode(result).Should().Be(200);
            saveRepairRequestUseCaseMock.Verify(x => x.Execute(RepairType.Tenant, repairRequest), Times.Once);
        }


        [Fact]
        public async Task TestLeaseholdEndpoint()
        {
            // Arrange
            var (repairRequest, repair) = CreateRepairRequestAndRepair();

            saveRepairRequestUseCaseMock.Setup(x => x.Execute(It.IsAny<string>(), It.IsAny<RepairRequest>())).ReturnsAsync(repair);

            // Act
            var result = await systemUnderTest.LeaseholdRepair(repairRequest);

            // Assert
            GetStatusCode(result).Should().Be(200);
            saveRepairRequestUseCaseMock.Verify(x => x.Execute(RepairType.Leasehold, repairRequest), Times.Once);
        }

        [Fact]
        public async Task TestCommunalEndpoint()
        {
            // Arrange
            var (repairRequest, repair) = CreateRepairRequestAndRepair();

            retrieveAvailableCommunalAppointmentUseCaseMock.Setup(x => x.Execute(repairRequest.Location.Value,
                    repairRequest.Problem.Value, repairRequest.Issue.Value, repairRequest.Address.LocationId))
                .ReturnsAsync(new AppointmentTime());
            saveRepairRequestUseCaseMock.Setup(x => x.Execute(It.IsAny<string>(), It.IsAny<RepairRequest>()))
                .ReturnsAsync(repair);

            // Act
            var result = await systemUnderTest.CommunalRepair(repairRequest);

            // Assert
            GetStatusCode(result).Should().Be(200);
            saveRepairRequestUseCaseMock.Verify(x => x.Execute(RepairType.Communal, repairRequest), Times.Once);
            retrieveAvailableCommunalAppointmentUseCaseMock.Verify(x => x.Execute(repairRequest.Location.Value,
                repairRequest.Problem.Value, repairRequest.Issue.Value, repairRequest.Address.LocationId));
        }

        private (RepairRequest, Repair) CreateRepairRequestAndRepair()
        {
            var repairRequest = new RepairRequest
            {
                ContactDetails = new RepairContactDetails { Value = "07465087654" },
                Address = new RepairAddress { Display = "address", LocationId = "uprn" },
                Description = new RepairDescriptionRequest { Text = "repair description", Base64Img = "image" },
                Location = new RepairLocation { Value = "location" },
                Problem = new RepairProblem { Value = "problem" },
                Issue = new RepairIssue { Value = "issue" }
            };

            var repair = new Repair
            {
                Id = "1AB2C3D4",
                ContactDetails = new RepairContactDetails { Value = "07465087654" },
                Address = new RepairAddress { Display = "address", LocationId = "uprn" },
                Description = new RepairDescription { Text = "repair description", Base64Image = "image", PhotoUrl = "x/Url.png" },
                Location = new RepairLocation { Value = "location" },
                Problem = new RepairProblem { Value = "problem" },
                Issue = new RepairIssue { Value = "issue" },
                SOR = "sor",
                Time = repairAvailability,
                Priority = "1"
            };

            return (repairRequest, repair);
        }

        [Fact]
        public async Task ReturnsErrorWhenFailsToSave()
        {
            RepairRequest repairRequest = new RepairRequest();

            saveRepairRequestUseCaseMock.Setup(x => x.Execute(It.IsAny<string>(), It.IsAny<RepairRequest>())).Throws<System.Exception>();

            var result = await systemUnderTest.SaveRepair(repairTypeArgument, repairRequest);

            GetStatusCode(result).Should().Be(500);
            saveRepairRequestUseCaseMock.Verify(x => x.Execute(It.IsAny<string>(), repairRequest), Times.Once);
        }

        [Fact]
        public async Task GivenEmailContact_WhenRepair_ThenSendAppointmentConfirmationEmailUseCaseIsCalled()
        {
            //Arrange
            RepairRequest repairRequest = new RepairRequest
            {
                ContactDetails = new RepairContactDetails
                {
                    Type = "email",
                    Value = "dr.who@tardis.com"
                },
                Time = new RepairAvailability
                {
                    Display = "Displayed Time"
                }
            };
            var repair = new Repair()
            {
                Id = "1AB2C3D4",
                ContactDetails = new RepairContactDetails
                {
                    Type = "email",
                    Value = "dr.who@tardis.com"
                },
                Time = repairAvailability,
                Address = repairAddress,
                Description = new RepairDescription()
                {
                    Text = "some textual description"
                }
            };
            saveRepairRequestUseCaseMock.Setup(x => x.Execute(It.IsAny<string>(), repairRequest)).ReturnsAsync(repair);

            //Assert
            await systemUnderTest.SaveRepair(repairTypeArgument, repairRequest);

            //Act
            appointmentConfirmationSender.Verify(x => x.Execute(repair), Times.Once);
        }

        [Fact]
        public async Task GivenSmsContact_WhenRepair_ThenSendAppointmentConfirmationSmsUseCaseIsCalled()
        {
            //Arrange
            var repairRequest = new RepairRequest()
            {
                ContactDetails = new RepairContactDetails
                {
                    Type = "text",
                    Value = "0765374057"
                },
                Time = new RepairAvailability
                {
                    Display = "Displayed Time"
                }
            };
            var repair = new Repair
            {
                Id = "1AB2C3D4",
                ContactDetails = new RepairContactDetails
                {
                    Type = "text",
                    Value = "0765374057"
                },
                Time = repairAvailability,
                Address = repairAddress,
                Description = new RepairDescription()
                {
                    Text = "some textual description"
                }
            };

            saveRepairRequestUseCaseMock.Setup(x => x.Execute(It.IsAny<string>(), repairRequest)).ReturnsAsync(repair);

            //Act
            await systemUnderTest.SaveRepair(repairTypeArgument, repairRequest);

            //Assert
            appointmentConfirmationSender.Verify(x => x.Execute(repair), Times.Once);
        }

        [Fact]
        public async Task GivenCommunalAppointmentIsNotFound_WhenRequestingCommunalRepair_ThenStatusCodeIs500()
        {
            // Arrange
            var (repairRequest, repair) = CreateRepairRequestAndRepair();
            retrieveAvailableCommunalAppointmentUseCaseMock.Setup(x => x.Execute(repairRequest.Location.Value,
                    repairRequest.Problem.Value, repairRequest.Issue.Value, repairRequest.Address.LocationId))
                .ReturnsAsync(null as AppointmentTime);

            // Act
            var actual = await systemUnderTest.CommunalRepair(repairRequest);

            // Assert
            GetStatusCode(actual).Should().Be(500);
        }

        [Fact]
        public async Task GivenCommunalAppointmentIsNotFound_WhenRequestingCommunalRepair_ThenRepairIsNotSaved()
        {
            // Arrange
            var (repairRequest, repair) = CreateRepairRequestAndRepair();
            retrieveAvailableCommunalAppointmentUseCaseMock.Setup(x => x.Execute(repairRequest.Location.Value,
                    repairRequest.Problem.Value, repairRequest.Issue.Value, repairRequest.Address.LocationId))
                .ReturnsAsync(null as AppointmentTime);

            // Act
            _ = await systemUnderTest.CommunalRepair(repairRequest);

            // Assert
            saveRepairRequestUseCaseMock.Verify(x => x.Execute(It.IsAny<string>(), It.IsAny<RepairRequest>()), Times.Never);
        }
    }
}
