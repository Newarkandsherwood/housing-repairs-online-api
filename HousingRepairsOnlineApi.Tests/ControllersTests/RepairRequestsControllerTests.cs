using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using HousingRepairsOnlineApi.Controllers;
using HousingRepairsOnlineApi.Domain;
using HousingRepairsOnlineApi.Helpers;
using HousingRepairsOnlineApi.Tests.Helpers;
using HousingRepairsOnlineApi.UseCases;
using Microsoft.AspNetCore.Mvc;
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
        private Mock<IRepairToRepairBookingResponseMapper> repairBookingResponseHelper;
        private Mock<IAppointmentTimeToRepairAvailabilityMapper> appointmentTimeToRepairAvailabilityMapperMock;
        private Mock<IRepairToFindRepairResponseMapper> repairToFindRepairResponseMapperMock;
        private Mock<ICancelAppointmentUseCase> cancelAppointmentUseCaseMock;
        private Mock<ICancelRepairRequestUseCase> cancelRepairRequestUseCaseMock;
        private Mock<IChangeAppointmentUseCase> changeAppointmentUseCaseMock;
        private Mock<ISaveChangedRepairRequestUseCase> saveChangedRepairRequestUseCaseMock;
        private readonly Mock<ISendRepairCancelledInternalEmailUseCase> sendRepairCancelledInternalEmailUseCaseMock;
        private readonly Mock<ISendRepairAppointmentChangedNotificationUseCase> sendRepairAppointmentChangedNotificationsUseCaseMock;
        private readonly Mock<ICreateWorkOrderUseCase> createWorkOrderUseCaseMock;

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
            repairBookingResponseHelper = new Mock<IRepairToRepairBookingResponseMapper>();
            appointmentTimeToRepairAvailabilityMapperMock = new Mock<IAppointmentTimeToRepairAvailabilityMapper>();
            repairToFindRepairResponseMapperMock = new Mock<IRepairToFindRepairResponseMapper>();
            cancelAppointmentUseCaseMock = new Mock<ICancelAppointmentUseCase>();
            cancelRepairRequestUseCaseMock = new Mock<ICancelRepairRequestUseCase>();
            changeAppointmentUseCaseMock = new Mock<IChangeAppointmentUseCase>();
            saveChangedRepairRequestUseCaseMock = new Mock<ISaveChangedRepairRequestUseCase>();
            sendRepairCancelledInternalEmailUseCaseMock = new Mock<ISendRepairCancelledInternalEmailUseCase>();
            sendRepairAppointmentChangedNotificationsUseCaseMock = new Mock<ISendRepairAppointmentChangedNotificationUseCase>();
            createWorkOrderUseCaseMock = new();

            systemUnderTest = new RepairController(saveRepairRequestUseCaseMock.Object, internalEmailSenderMock.Object,
                appointmentConfirmationSender.Object, bookAppointmentUseCaseMock.Object,
                retrieveRepairsUseCaseMock.Object, retrieveAvailableCommunalAppointmentUseCaseMock.Object,
                repairBookingResponseHelper.Object,
                appointmentTimeToRepairAvailabilityMapperMock.Object, repairToFindRepairResponseMapperMock.Object,
                cancelAppointmentUseCaseMock.Object, cancelRepairRequestUseCaseMock.Object, sendRepairCancelledInternalEmailUseCaseMock.Object,
                changeAppointmentUseCaseMock.Object, saveChangedRepairRequestUseCaseMock.Object, sendRepairAppointmentChangedNotificationsUseCaseMock.Object,
                createWorkOrderUseCaseMock.Object);
        }

        [Fact]
        public async Task TestEndpoint()
        {
            var (repairRequest, repair) = CreateRepairRequestAndRepair();

            saveRepairRequestUseCaseMock.Setup(x => x.Execute(It.IsAny<string>(), It.IsAny<RepairRequest>(), It.IsAny<string>())).ReturnsAsync(repair);

            var result = await systemUnderTest.SaveRepair(repairTypeArgument, repairRequest);

            GetStatusCode(result).Should().Be(200);

            saveRepairRequestUseCaseMock.Verify(x => x.Execute(It.IsAny<string>(), repairRequest, It.IsAny<string>()), Times.Once);

            internalEmailSenderMock.Verify(x => x.Execute(repair), Times.Once);
        }

        [Fact]
        public async Task TestTenantEndpoint()
        {
            // Arrange
            var (repairRequest, repair) = CreateRepairRequestAndRepair();

            saveRepairRequestUseCaseMock.Setup(x => x.Execute(It.IsAny<string>(), It.IsAny<RepairRequest>(), It.IsAny<string>())).ReturnsAsync(repair);

            // Act
            var result = await systemUnderTest.TenantRepair(repairRequest);

            // Assert
            GetStatusCode(result).Should().Be(200);
            saveRepairRequestUseCaseMock.Verify(x => x.Execute(RepairType.Tenant, repairRequest, It.IsAny<string>()), Times.Once);
        }


        [Fact]
        public async Task TestLeaseholdEndpoint()
        {
            // Arrange
            var (repairRequest, repair) = CreateRepairRequestAndRepair();

            saveRepairRequestUseCaseMock.Setup(x => x.Execute(It.IsAny<string>(), It.IsAny<RepairRequest>(), It.IsAny<string>())).ReturnsAsync(repair);

            // Act
            var result = await systemUnderTest.LeaseholdRepair(repairRequest);

            // Assert
            GetStatusCode(result).Should().Be(200);
            saveRepairRequestUseCaseMock.Verify(x => x.Execute(RepairType.Leasehold, repairRequest, It.IsAny<string>()), Times.Once);
        }

        [Fact]
        public async Task TestCommunalEndpoint()
        {
            // Arrange
            var (repairRequest, repair) = CreateRepairRequestAndRepair();

            retrieveAvailableCommunalAppointmentUseCaseMock.Setup(x => x.Execute(repairRequest.Location.Value,
                    repairRequest.Problem.Value, repairRequest.Issue.Value, repairRequest.Address.LocationId))
                .ReturnsAsync(new AppointmentTime());
            saveRepairRequestUseCaseMock.Setup(x => x.Execute(It.IsAny<string>(), It.IsAny<RepairRequest>(), It.IsAny<string>()))
                .ReturnsAsync(repair);

            // Act
            var result = await systemUnderTest.CommunalRepair(repairRequest);

            // Assert
            GetStatusCode(result).Should().Be(200);
            saveRepairRequestUseCaseMock.Verify(x => x.Execute(RepairType.Communal, repairRequest, It.IsAny<string>()), Times.Once);
            retrieveAvailableCommunalAppointmentUseCaseMock.Verify(x => x.Execute(repairRequest.Location.Value,
                repairRequest.Problem.Value, repairRequest.Issue.Value, repairRequest.Address.LocationId));
        }

        [Fact]
        public async Task TestTenantOrLeaseholdPropertyRepairEndpoint()
        {
            // Arrange
            var repairId = "repairId";
            var postcode = "postcode";

            string[] repairTypesUsed = null;

            retrieveRepairsUseCaseMock.Setup(x => x.Execute(It.IsAny<IEnumerable<string>>(), postcode, repairId, false))
                .Callback<IEnumerable<string>, string, string, bool>((repairTypes, _, _, _) =>
                    repairTypesUsed = repairTypes.ToArray())
                .ReturnsAsync(new Repair());

            // Act
            var result = await systemUnderTest.TenantOrLeaseholdPropertyRepair(postcode, repairId);

            // Assert
            GetStatusCode(result).Should().Be(200);
            repairTypesUsed.Should().NotBeNull();
            repairTypesUsed.Should().BeEquivalentTo(new[] { RepairType.Leasehold, RepairType.Tenant });
            retrieveRepairsUseCaseMock.Verify(x => x.Execute(repairTypesUsed, postcode, repairId, false));
        }

        [Fact]
        public async Task GivenNoRepairMatched_WhenCallingTenantOrLeaseholdPropertyRepair_ThenStatusIs404()
        {
            // Arrange
            var repairId = "repairId";
            var postcode = "postcode";

            retrieveRepairsUseCaseMock.Setup(x => x.Execute(It.IsAny<IEnumerable<string>>(), postcode, repairId, false));

            // Act
            var result = await systemUnderTest.TenantOrLeaseholdPropertyRepair(postcode, repairId);

            // Assert
            GetStatusCode(result).Should().Be(404);
        }

        [Fact]
        public async Task GivenACancelledRepair_WhenTenantOrLeaseholdPropertyRepairCancelCalled_ReturnOK()
        {
            // Arrange
            var repairId = "repairId";
            var postcode = "postcode";

            string[] repairTypesUsed = null;

            retrieveRepairsUseCaseMock.Setup(x => x.Execute(It.IsAny<IEnumerable<string>>(), postcode, repairId, true))
                .Callback<IEnumerable<string>, string, string, bool>((repairTypes, _, _, _) =>
                    repairTypesUsed = repairTypes.ToArray())
                .ReturnsAsync(new Repair() { Status = RepairStatus.Cancelled });

            // Act
            var result = await systemUnderTest.TenantOrLeaseholdPropertyRepairCancel(postcode, repairId);

            // Assert
            GetStatusCode(result).Should().Be(200);
            (result as OkObjectResult)?.Value.Should().Be("The repair has already been cancelled in Housing Repairs Online");
            repairTypesUsed.Should().NotBeNull();
            repairTypesUsed.Should().BeEquivalentTo(new[] { RepairType.Leasehold, RepairType.Tenant });
            retrieveRepairsUseCaseMock.Verify(x => x.Execute(repairTypesUsed, postcode, repairId, true));
        }

        [Fact]
        public async Task GivenNoRepairMatched_WhenCallingTenantOrLeaseholdPropertyRepairCancel_ThenStatusIs404()
        {
            // Arrange
            var repairId = "repairId";
            var postcode = "postcode";

            retrieveRepairsUseCaseMock.Setup(x => x.Execute(It.IsAny<IEnumerable<string>>(), postcode, repairId, false));

            // Act
            var result = await systemUnderTest.TenantOrLeaseholdPropertyRepairCancel(postcode, repairId);

            // Assert
            GetStatusCode(result).Should().Be(404);
            (result as NotFoundObjectResult)?.Value.Should()
                .Be("Repair request not found for postcode and repairId provided");
        }

        [Fact]
        public async Task GivenAScheduledRepairWhenRepairNotFoundInHMS_WhenCallingTenantOrLeaseholdPropertyRepairCancel_ThenErrorReturned()
        {
            // Arrange
            var repairId = "repairId";
            var postcode = "postcode";
            string[] repairTypesUsed = null;

            retrieveRepairsUseCaseMock.Setup(x => x.Execute(It.IsAny<IEnumerable<string>>(), postcode, repairId, true))
                .Callback<IEnumerable<string>, string, string, bool>((repairTypes, _, _, _) =>
                    repairTypesUsed = repairTypes.ToArray())
                .ReturnsAsync(new Repair() { Status = RepairStatus.Scheduled });

            cancelAppointmentUseCaseMock.Setup(x => x.Execute(It.IsAny<string>()))
                .ReturnsAsync(UpdateOrCancelAppointmentStatus.NotFound);
            // Act
            var result = await systemUnderTest.TenantOrLeaseholdPropertyRepairCancel(postcode, repairId);

            // Assert
            GetStatusCode(result).Should().Be(500);
            (result as ObjectResult)?.Value.Should()
                .Be("Error updating the appointment");
        }

        [Fact]
        public async Task GivenAScheduledRepairWhenErrorReturnedFromScheduler_WhenCallingTenantOrLeaseholdPropertyRepairCancel_ThenErrorReturned()
        {
            // Arrange
            var repairId = "repairId";
            var postcode = "postcode";
            string[] repairTypesUsed = null;

            retrieveRepairsUseCaseMock.Setup(x => x.Execute(It.IsAny<IEnumerable<string>>(), postcode, repairId, true))
                .Callback<IEnumerable<string>, string, string, bool>((repairTypes, _, _, _) =>
                    repairTypesUsed = repairTypes.ToArray())
                .ReturnsAsync(new Repair() { Status = RepairStatus.Scheduled });

            cancelAppointmentUseCaseMock.Setup(x => x.Execute(It.IsAny<string>()))
                .ReturnsAsync(UpdateOrCancelAppointmentStatus.Error);
            // Act
            var result = await systemUnderTest.TenantOrLeaseholdPropertyRepairCancel(postcode, repairId);

            // Assert
            GetStatusCode(result).Should().Be(500);
            (result as ObjectResult)?.Value.Should()
                .Be("Error updating the appointment");
        }

        [Fact]
        public async Task GivenAScheduledRepair_WhenCallingTenantOrLeaseholdPropertyRepairCancel_ThenCancelRepairRequestUseCaseCalled()
        {
            // Arrange
            var repairId = "repairId";
            var postcode = "postcode";

            retrieveRepairsUseCaseMock.Setup(x => x.Execute(It.IsAny<IEnumerable<string>>(), postcode, repairId, true))
                .Callback<IEnumerable<string>, string, string, bool>((repairTypes, _, _, _) => { })
                .ReturnsAsync(new Repair() { Status = RepairStatus.Scheduled });

            cancelAppointmentUseCaseMock.Setup(x => x.Execute(It.IsAny<string>()))
                .ReturnsAsync(UpdateOrCancelAppointmentStatus.AppointmentCancelled);

            // Act
            var result = await systemUnderTest.TenantOrLeaseholdPropertyRepairCancel(postcode, repairId);

            // Assert
            GetStatusCode(result).Should().Be(200);
            cancelRepairRequestUseCaseMock.Verify(x => x.Execute(It.IsAny<Repair>()), Times.Once);
        }

        [Fact]
        public async Task GivenAScheduledRepair_WhenCallingTenantOrLeaseholdPropertyRepairCancel_ThenSendRepairCancelledInternalEmailUseCaseCalled()
        {
            // Arrange
            var repairId = "repairId";
            var postcode = "postcode";

            retrieveRepairsUseCaseMock.Setup(x => x.Execute(It.IsAny<IEnumerable<string>>(), postcode, repairId, true))
                .Callback<IEnumerable<string>, string, string, bool>((repairTypes, _, _, _) =>
                {
                })
                .ReturnsAsync(new Repair { Status = RepairStatus.Scheduled });

            cancelAppointmentUseCaseMock.Setup(x => x.Execute(It.IsAny<string>()))
                .ReturnsAsync(UpdateOrCancelAppointmentStatus.AppointmentCancelled);
            sendRepairCancelledInternalEmailUseCaseMock.Setup(x => x.Execute(It.IsAny<Repair>()));

            // Act
            var result = await systemUnderTest.TenantOrLeaseholdPropertyRepairCancel(postcode, repairId);

            // Assert
            GetStatusCode(result).Should().Be(200);
            sendRepairCancelledInternalEmailUseCaseMock.Verify(x => x.Execute(It.IsAny<Repair>()), Times.Once);
        }

        [Fact]
        public async Task GivenNoRepairMatched_WhenCallingTenantOrLeaseholdPropertyRepairChangeAppointmentSlot_ThenStatusIs404()
        {
            // Arrange
            var repairId = "repairId";
            var postcode = "postcode";

            retrieveRepairsUseCaseMock.Setup(x => x.Execute(It.IsAny<IEnumerable<string>>(), postcode, repairId, false));

            // Act
            var result = await systemUnderTest.TenantOrLeaseholdPropertyRepairChangeAppointmentSlot(postcode, repairId, new RepairAvailability());

            // Assert
            GetStatusCode(result).Should().Be(404);
            (result as NotFoundObjectResult)?.Value.Should()
                .Be("Repair request not found for postcode and repairId provided");
        }

        [Fact]
        public async Task GivenRepairMatchedButHasSameDates_WhenCallingTenantOrLeaseholdPropertyRepairChange_ThenStatusIsOK()
        {
            // Arrange
            var repairId = "repairId";
            var postcode = "postcode";
            var sameRepairAvailability = new RepairAvailability()
            {
                StartDateTime = new DateTime(2022, 1, 1),
                EndDateTime = new DateTime(2022, 1, 2),
            };
            var repair = new Repair() { Time = sameRepairAvailability };

            retrieveRepairsUseCaseMock.Setup(x => x.Execute(It.IsAny<IEnumerable<string>>(), postcode, repairId, false)).ReturnsAsync(repair);

            // Act
            var result = await systemUnderTest.TenantOrLeaseholdPropertyRepairChangeAppointmentSlot(postcode, repairId, sameRepairAvailability);

            // Assert
            GetStatusCode(result).Should().Be(200);
            (result as OkObjectResult)?.Value.Should().Be("The repair already has the same start and end times as those provided");
        }

        [Fact]
        public async Task GivenRepairMatched_AndSchedulerReturnsError_WhenCallingTenantOrLeaseholdPropertyRepairChange_ThenStatusIsError()
        {
            // Arrange
            var repairId = "repairId";
            var postcode = "postcode";
            var altRepairAvailability = new RepairAvailability()
            {
                StartDateTime = new DateTime(2022, 1, 1),
                EndDateTime = new DateTime(2022, 1, 2),
            };
            var repair = new Repair { Time = repairAvailability };

            retrieveRepairsUseCaseMock.Setup(x => x.Execute(It.IsAny<IEnumerable<string>>(), postcode, repairId, false)).ReturnsAsync(repair);
            changeAppointmentUseCaseMock
                .Setup(x => x.Execute(It.IsAny<string>(), It.IsAny<DateTime>(), It.IsAny<DateTime>()))
                .ReturnsAsync(UpdateOrCancelAppointmentStatus.Error);

            // Act
            var result = await systemUnderTest.TenantOrLeaseholdPropertyRepairChangeAppointmentSlot(postcode, repairId, altRepairAvailability);

            // Assert
            GetStatusCode(result).Should().Be(500);
            (result as ObjectResult)?.Value.Should()
                .Be("Error changing the appointment");
        }

        [Fact]
        public async Task GivenRepairMatched_AndSchedulerReturnsNotFound_WhenCallingTenantOrLeaseholdPropertyRepairChange_ThenStatusIsError()
        {
            // Arrange
            var repairId = "repairId";
            var postcode = "postcode";
            var altRepairAvailability = new RepairAvailability()
            {
                StartDateTime = new DateTime(2022, 1, 1),
                EndDateTime = new DateTime(2022, 1, 2),
            };
            var repair = new Repair { Time = repairAvailability };

            retrieveRepairsUseCaseMock.Setup(x => x.Execute(It.IsAny<IEnumerable<string>>(), postcode, repairId, false)).ReturnsAsync(repair);
            changeAppointmentUseCaseMock
                .Setup(x => x.Execute(It.IsAny<string>(), It.IsAny<DateTime>(), It.IsAny<DateTime>()))
                .ReturnsAsync(UpdateOrCancelAppointmentStatus.NotFound);

            // Act
            var result = await systemUnderTest.TenantOrLeaseholdPropertyRepairChangeAppointmentSlot(postcode, repairId, altRepairAvailability);

            // Assert
            GetStatusCode(result).Should().Be(500);
            (result as ObjectResult)?.Value.Should()
                .Be("Error changing the appointment");
        }

        [Fact]
        public async Task GivenRepairMatched_WhenCallingTenantOrLeaseholdPropertyRepairChangeAppointmentSlot_ThenSendRepairAppointmentChangedNotificationsUseCaseCalled()
        {
            // Arrange
            var repairId = "repairId";
            var postcode = "postcode";
            var repair = new Repair { Time = repairAvailability };
            var newRepairAvailability = new RepairAvailability()
            {
                StartDateTime = repairAvailability.StartDateTime.AddDays(1),
                EndDateTime = repairAvailability.EndDateTime.AddDays(1),
            };

            retrieveRepairsUseCaseMock.Setup(x => x.Execute(It.IsAny<IEnumerable<string>>(), postcode, repairId, false)).ReturnsAsync(repair);
            changeAppointmentUseCaseMock.Setup(x => x.Execute(It.IsAny<string>(), It.IsAny<DateTime>(), It.IsAny<DateTime>()))
                .ReturnsAsync(UpdateOrCancelAppointmentStatus.AppointmentUpdated);
            sendRepairAppointmentChangedNotificationsUseCaseMock.Setup(x => x.Execute(repair));

            // Act
            var result = await systemUnderTest.TenantOrLeaseholdPropertyRepairChangeAppointmentSlot(postcode, repairId, newRepairAvailability);

            // Assert
            GetStatusCode(result).Should().Be(200);
            sendRepairAppointmentChangedNotificationsUseCaseMock.Verify(x => x.Execute(repair), Times.Once);
        }

        [Fact]
        public async Task GivenRepairMatched_AndSchedulerReturnsSuccess_WhenCallingTenantOrLeaseholdPropertyRepairChange_ThenCallsChangeRepairRequestUseCaseExecute()
        {
            // Arrange
            var repairId = "repairId";
            var postcode = "postcode";
            var altRepairAvailability = new RepairAvailability()
            {
                StartDateTime = new DateTime(2022, 1, 1),
                EndDateTime = new DateTime(2022, 1, 2),
            };
            var repair = new Repair { Time = repairAvailability };

            retrieveRepairsUseCaseMock.Setup(x => x.Execute(It.IsAny<IEnumerable<string>>(), postcode, repairId, false)).ReturnsAsync(repair);
            changeAppointmentUseCaseMock
                .Setup(x => x.Execute(It.IsAny<string>(), It.IsAny<DateTime>(), It.IsAny<DateTime>()))
                .ReturnsAsync(UpdateOrCancelAppointmentStatus.AppointmentUpdated);
            saveChangedRepairRequestUseCaseMock.Setup(x =>
                x.Execute(It.IsAny<Repair>()));

            // Act
            var result = await systemUnderTest.TenantOrLeaseholdPropertyRepairChangeAppointmentSlot(postcode, repairId, altRepairAvailability);

            // Assert
            saveChangedRepairRequestUseCaseMock.Verify(x => x.Execute(repair), Times.Once);
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

            saveRepairRequestUseCaseMock.Setup(x => x.Execute(It.IsAny<string>(), It.IsAny<RepairRequest>(), It.IsAny<string>())).Throws<System.Exception>();

            var result = await systemUnderTest.SaveRepair(repairTypeArgument, repairRequest);

            GetStatusCode(result).Should().Be(500);
            saveRepairRequestUseCaseMock.Verify(x => x.Execute(It.IsAny<string>(), repairRequest, It.IsAny<string>()), Times.Once);
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
            saveRepairRequestUseCaseMock.Setup(x => x.Execute(It.IsAny<string>(), repairRequest, It.IsAny<string>())).ReturnsAsync(repair);

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

            saveRepairRequestUseCaseMock.Setup(x => x.Execute(It.IsAny<string>(), repairRequest, It.IsAny<string>())).ReturnsAsync(repair);

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
            saveRepairRequestUseCaseMock.Verify(x => x.Execute(It.IsAny<string>(), It.IsAny<RepairRequest>(), It.IsAny<string>()), Times.Never);
        }

        [Fact]
        public async Task GivenWorkOrderIsCreated_WhenRequestingRepair_ThenRepairIsSavedWithWorkOrderId()
        {
            // Arrange
            var (repairRequest, repair) = CreateRepairRequestAndRepair();

            const string WorkOrderId = "WorkOrderID";
            createWorkOrderUseCaseMock.Setup(x => x.Execute(It.IsAny<string>(), It.IsAny<RepairRequest>()))
                .ReturnsAsync(WorkOrderId);

            saveRepairRequestUseCaseMock.Setup(x => x.Execute(It.IsAny<string>(), It.IsAny<RepairRequest>(), It.IsAny<string>())).ReturnsAsync(repair);

            // Act
            var result = await systemUnderTest.SaveRepair(repairTypeArgument, repairRequest);

            // Assert
            GetStatusCode(result).Should().Be(200);
            saveRepairRequestUseCaseMock.Verify(x => x.Execute(It.IsAny<string>(), repairRequest, WorkOrderId), Times.Once);
        }

        [Theory]
        [MemberData(nameof(RepairTypeTestData.ValidRepairTypeArgumentTestData), MemberType = typeof(RepairTypeTestData))]
        public async Task GivenAnyRepairType_WhenSavingRepairRequest_ThenCreateWorkOrderUseCaseIsExecuted(string repairType)
        {
            // Arrange
            const string WorkOrderId = "WorkOrderID";
            createWorkOrderUseCaseMock.Setup(x => x.Execute(It.IsAny<string>(), It.IsAny<RepairRequest>()))
                .ReturnsAsync(WorkOrderId);
            var repairRequest = new RepairRequest();

            // Act
            _ = await systemUnderTest.SaveRepair(repairType, repairRequest);

            // Assert
            createWorkOrderUseCaseMock.Verify(x => x.Execute(It.IsAny<string>(), It.IsAny<RepairRequest>()), Times.Once);
        }
    }
}
