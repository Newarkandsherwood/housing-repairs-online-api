using System;
using System.Collections.Generic;
using FluentAssertions;
using HousingRepairsOnlineApi.Domain;
using HousingRepairsOnlineApi.Helpers;
using HousingRepairsOnlineApi.Helpers.NotificationConfiguration;
using HousingRepairsOnlineApi.UseCases;
using Moq;
using Xunit;

namespace HousingRepairsOnlineApi.Tests.HelpersTests
{
    public class CommunalNotificationConfigurationProviderTests
    {
        private readonly Mock<IRetrieveImageLinkUseCase> retrieveImageLinkUseCase;
        private readonly CommunalNotificationConfigurationProvider systemUnderTest;

        private readonly string confirmationSmsTemplateId = "confirmationSmsTemplateId";
        private readonly string confirmationEmailTemplateId = "confirmationEmailTemplateId";
        private readonly string internalEmailTemplateId = "internalEmailTemplateId";
        private readonly string imageLink = "imagelink";

        private Repair repair = new()
        {
            Id = "id",
            RepairType = null,
            Postcode = null,
            SOR = "SOR",
            Priority = null,
            Address = new RepairAddress
            {
                LocationId = "12345",
                Display = "display"
            },
            Location = null,
            Problem = null,
            Issue = null,
            ContactPersonNumber = null,
            Description = new RepairDescription
            {
                Text = "description",
                PhotoUrl = "photourl/photo"
            },
            ContactDetails = new RepairContactDetails
            {
                Type = AppointmentConfirmationSendingTypes.Email,
                Value = "abc@defg.hij"
            },
            Time = new RepairAvailability()
            {
                Display = "some time"
            }
        };

        public CommunalNotificationConfigurationProviderTests()
        {
            retrieveImageLinkUseCase = new Mock<IRetrieveImageLinkUseCase>();
            retrieveImageLinkUseCase.Setup(x => x.Execute(It.IsAny<string>())).Returns(imageLink);

            systemUnderTest = new CommunalNotificationConfigurationProvider(confirmationSmsTemplateId, confirmationEmailTemplateId, internalEmailTemplateId);
        }

        [Fact]
        public void GivenTemplateIds_WhenGet_ThenCorrectTemplateIdsReturned()
        {
            //Assert
            Assert.True(systemUnderTest.ConfirmationEmailTemplateId == confirmationEmailTemplateId);
            Assert.True(systemUnderTest.ConfirmationSmsTemplateId == confirmationSmsTemplateId);
            Assert.True(systemUnderTest.InternalEmailTemplateId == internalEmailTemplateId);
        }

        //Arrange
        public static IEnumerable<object[]> InvalidStringArgumentTestData()
        {
            yield return new object[] { new ArgumentNullException(), null };
            yield return new object[] { new ArgumentException(), "" };
            yield return new object[] { new ArgumentException(), " " };
        }

        [Theory]
        [MemberData(nameof(InvalidStringArgumentTestData))]
#pragma warning disable xUnit1026
        public void GivenAnInvalidBookingRef_WhenGetPersonalisationForEmailTemplate_ThenExceptionIsThrown<T>(T exception, string invalidStr) where T : Exception
#pragma warning restore xUnit1026
        {
            this.repair.Id = invalidStr;
            //Act
            Action act = () => systemUnderTest.GetPersonalisationForEmailTemplate(repair);

            //Assert
            act.Should().ThrowExactly<T>();
        }

        [Theory]
        [MemberData(nameof(InvalidStringArgumentTestData))]
#pragma warning disable xUnit1026
        public void GivenAnAddress_WhenGetPersonalisationForEmailTemplate_ThenExceptionIsThrown<T>(T exception, string invalidStr) where T : Exception
#pragma warning restore xUnit1026
        {
            repair.Address.Display = invalidStr;
            //Act
            Action act = () => systemUnderTest.GetPersonalisationForEmailTemplate(repair);

            //Assert
            act.Should().ThrowExactly<T>();
        }

        [Fact]
        public void GivenValidRepairValues_WhenGetPersonalisationForEmailTemplate_ThenCorrectDictionaryIsReturned()
        {
            //Arrange
            Dictionary<string, dynamic> personalisation = new()
            {
                {"repair_ref", repair.Id},
                {"address", repair.Address.Display}
            };
            //Act
            var act = systemUnderTest.GetPersonalisationForEmailTemplate(repair);
            //Assert
            act.Should().BeEquivalentTo(personalisation);
        }
        [Theory]
        [MemberData(nameof(InvalidStringArgumentTestData))]
#pragma warning disable xUnit1026
        public void GivenAnInvalidBookingRef_WhenGetPersonalisationForSMSTemplate_ThenExceptionIsThrown<T>(T exception, string invalidStr) where T : Exception
#pragma warning restore xUnit1026
        {
            this.repair.Id = invalidStr;
            //Act
            Action act = () => systemUnderTest.GetPersonalisationForSmsTemplate(repair);

            //Assert
            act.Should().ThrowExactly<T>();
        }

        [Theory]
        [MemberData(nameof(InvalidStringArgumentTestData))]
#pragma warning disable xUnit1026
        public void GivenAnInvalidAddress_WhenGetPersonalisationForSMSTemplate_ThenExceptionIsThrown<T>(T exception, string invalidStr) where T : Exception
#pragma warning restore xUnit1026
        {
            repair.Address.Display = invalidStr;
            //Act
            Action act = () => systemUnderTest.GetPersonalisationForSmsTemplate(repair);

            //Assert
            act.Should().ThrowExactly<T>();
        }

        [Fact]
        public void GivenValidRepairValues_WhenGetPersonalisationForSMSTemplate_ThenCorrectDictionaryIsReturned()
        {
            //Arrange
            Dictionary<string, dynamic> personalisation = new()
            {
                {"repair_ref", repair.Id},
                {"address", repair.Address.Display}
            };
            //Act
            var act = systemUnderTest.GetPersonalisationForSmsTemplate(repair);
            //Assert
            act.Should().BeEquivalentTo(personalisation);
        }

        [Fact]
        public void GivenNullPhotoURL_WhenGetPersonalisationForInternalEmailTemplate_ThenDefaultIsReturned()
        {
            //Arrange
            repair.Description.PhotoUrl = string.Empty;
            //Act
            var act = systemUnderTest.GetPersonalisationForInternalEmailTemplate(repair, retrieveImageLinkUseCase.Object);
            //Assert
            Assert.Equal(act.Result["image_1"], "None");
        }

        [Fact]
        public void GivenPhotoURL_WhenGetPersonalisationForInternalEmailTemplate_ThenImageLinkIsReturned()
        {
            //Act
            var act = systemUnderTest.GetPersonalisationForInternalEmailTemplate(repair, retrieveImageLinkUseCase.Object);
            //Assert
            Assert.Equal(act.Result["image_1"], imageLink);
        }

        [Theory]
        [MemberData(nameof(InvalidStringArgumentTestData))]
#pragma warning disable xUnit1026
        public async void GivenAnInvalidId_WhenGetPersonalisationForInternalEmailTemplate_ThenExceptionIsThrown<T>(T exception, string invalidStr) where T : Exception
#pragma warning restore xUnit1026
        {
            // Arrange
            repair.Id = invalidStr;

            // Act
            var act = async () => await systemUnderTest.GetPersonalisationForInternalEmailTemplate(repair, retrieveImageLinkUseCase.Object);

            // Assert
            await act.Should().ThrowExactlyAsync<T>();
        }

        [Theory]
        [MemberData(nameof(InvalidStringArgumentTestData))]
#pragma warning disable xUnit1026
        public async void GivenAnInvalidDisplayTime_WhenGetPersonalisationForInternalEmailTemplate_ThenExceptionIsThrown<T>(T exception, string invalidStr) where T : Exception
#pragma warning restore xUnit1026
        {
            // Arrange
            repair.Time.Display = invalidStr;

            // Act
            var act = async () => await systemUnderTest.GetPersonalisationForInternalEmailTemplate(repair, retrieveImageLinkUseCase.Object);

            // Assert
            await act.Should().ThrowExactlyAsync<T>();
        }

        [Theory]
        [MemberData(nameof(InvalidStringArgumentTestData))]
#pragma warning disable xUnit1026
        public async void GivenAnInvalidAddressLocationId_WhenGetPersonalisationForInternalEmailTemplate_ThenExceptionIsThrown<T>(T exception, string invalidStr) where T : Exception
#pragma warning restore xUnit1026
        {
            // Arrange
            repair.Address.LocationId = invalidStr;
            // Act
            var act = async () => await systemUnderTest.GetPersonalisationForInternalEmailTemplate(repair, retrieveImageLinkUseCase.Object);

            // Assert
            await act.Should().ThrowExactlyAsync<T>();
        }

        [Theory]
        [MemberData(nameof(InvalidStringArgumentTestData))]
#pragma warning disable xUnit1026
        public async void GivenAnInvalidDisplayAddress_WhenGetPersonalisationForInternalEmailTemplate_ThenExceptionIsThrown<T>(T exception, string invalidStr) where T : Exception
#pragma warning restore xUnit1026
        {
            // Arrange
            repair.Address.Display = invalidStr;

            // Act
            var act = async () => await systemUnderTest.GetPersonalisationForInternalEmailTemplate(repair, retrieveImageLinkUseCase.Object);

            // Assert
            await act.Should().ThrowExactlyAsync<T>();
        }

        [Theory]
        [MemberData(nameof(InvalidStringArgumentTestData))]
#pragma warning disable xUnit1026
        public async void GivenAnInvalidSor_WhenGetPersonalisationForInternalEmailTemplate_ThenExceptionIsThrown<T>(T exception, string invalidStr) where T : Exception
#pragma warning restore xUnit1026
        {
            // Arrange
            repair.SOR = invalidStr;

            // Act
            var act = async () => await systemUnderTest.GetPersonalisationForInternalEmailTemplate(repair, retrieveImageLinkUseCase.Object);

            // Assert
            await act.Should().ThrowExactlyAsync<T>();
        }

        [Theory]
        [MemberData(nameof(InvalidStringArgumentTestData))]
#pragma warning disable xUnit1026
        public async void GivenAnInvalidDescriptionText_WhenGetPersonalisationForInternalEmailTemplate_ThenExceptionIsThrown<T>(T exception, string invalidStr) where T : Exception
#pragma warning restore xUnit1026
        {
            // Arrange
            repair.Description.Text = invalidStr;

            // Act
            var act = async () => await systemUnderTest.GetPersonalisationForInternalEmailTemplate(repair, retrieveImageLinkUseCase.Object);

            // Assert
            await act.Should().ThrowExactlyAsync<T>();
        }

        [Theory]
        [MemberData(nameof(InvalidStringArgumentTestData))]
#pragma warning disable xUnit1026
        public async void GivenAnInvalidContactDetailsValue_WhenGetPersonalisationForInternalEmailTemplate_ThenExceptionIsThrown<T>(T exception, string invalidStr) where T : Exception
#pragma warning restore xUnit1026
        {
            // Arrange
            repair.ContactDetails.Value = invalidStr;

            // Act
            var act = async () => await systemUnderTest.GetPersonalisationForInternalEmailTemplate(repair, retrieveImageLinkUseCase.Object);

            // Assert
            await act.Should().ThrowExactlyAsync<T>();
        }
    }
}
