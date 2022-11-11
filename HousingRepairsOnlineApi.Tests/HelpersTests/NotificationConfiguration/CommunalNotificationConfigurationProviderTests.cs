using System;
using System.Collections.Generic;
using FluentAssertions;
using HousingRepairsOnlineApi.Domain;
using HousingRepairsOnlineApi.Helpers;
using HousingRepairsOnlineApi.Helpers.SendNotifications;
using Moq;
using Xunit;

namespace HousingRepairsOnlineApi.Tests.HelpersTests
{
    public class CommunalNotificationConfigurationProviderTests
    {
        private Mock<INotificationConfigurationResolver> notificationConfigurationResolver;
        private Mock<INotificationConfigurationProvider> notificationConfigurationProvider;
        private readonly CommunalNotificationConfigurationProvider systemUnderTest;

        private readonly string confirmationSmsTemplateId = "confirmationSmsTemplateId";
        private readonly string confirmationEmailTemplateId = "confirmationEmailTemplateId";
        private readonly string internalEmailTemplateId = "internalEmailTemplateId";

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
                Text = "description"
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
            notificationConfigurationResolver = new Mock<INotificationConfigurationResolver>();
            notificationConfigurationProvider = new Mock<INotificationConfigurationProvider>();


            notificationConfigurationResolver.Setup(x => x.Resolve(It.IsAny<string>())).Returns(notificationConfigurationProvider.Object);

            systemUnderTest = new CommunalNotificationConfigurationProvider(confirmationSmsTemplateId, confirmationEmailTemplateId, internalEmailTemplateId);
        }

        //Arrange
        public static IEnumerable<object[]> InvalidStringArgumentTestData()
        {
            yield return new object[] { new ArgumentNullException(), null };
            yield return new object[] { new ArgumentException(), "" };
        }

        [Theory]
        [MemberData(nameof(InvalidStringArgumentTestData))]
#pragma warning disable xUnit1026
        public void GivenAnInvalidBookingRef_WhenExecute_ThenExceptionIsThrown<T>(T exception, string invalidStr) where T : Exception
#pragma warning restore xUnit1026
        {
            this.repair.Id = invalidStr;
            //Act
            Action act = () => systemUnderTest.GetPersonalisationForEmailTemplate(repair);

            //Assert
            act.Should().ThrowExactly<T>();
        }

        //Arrange
        public static IEnumerable<object[]> InvalidAppointmentTimeArgumentTestData()
        {
            yield return new object[] { new ArgumentNullException(), null };
            yield return new object[] { new ArgumentException(), "" };
        }


    }
}
