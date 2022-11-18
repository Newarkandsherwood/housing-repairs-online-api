using HousingRepairsOnlineApi.Helpers.NotificationConfiguration;
using Xunit;

namespace HousingRepairsOnlineApi.Tests.HelpersTests
{
    public class BaseNotificationConfigurationProviderTests
    {
        private readonly CommunalNotificationConfigurationProvider systemUnderTest;

        private readonly string confirmationSmsTemplateId = "confirmationSmsTemplateId";
        private readonly string confirmationEmailTemplateId = "confirmationEmailTemplateId";
        private readonly string internalEmailTemplateId = "internalEmailTemplateId";

        public BaseNotificationConfigurationProviderTests()
        {
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
    }
}
