using System.Collections.Generic;
using System.Threading.Tasks;
using HousingRepairsOnlineApi.Gateways;
using Moq;
using Notify.Interfaces;
using Xunit;

namespace HousingRepairsOnlineApi.Tests.GatewaysTests
{
    public class GovNotifyGatewayTests
    {
        private readonly NotifyGateway systemUnderTest;
        private readonly Mock<INotificationClient> notifyClinet;

        public GovNotifyGatewayTests()
        {
            notifyClinet = new Mock<INotificationClient>();
            systemUnderTest = new NotifyGateway(notifyClinet.Object);
        }

        [Fact]
        public void GivenNoException_WhenSendSms_ThenSendSmsIsCalledOnClient()
        {
            //Arrange
            var personalisation = new Dictionary<string, dynamic>
            {
                {"repair_ref", "XXXX"},
                {"appointment_time", "10.00am"}

            };

            //Act
            systemUnderTest.SendSms("07415678534", "templateId", personalisation);

            //Assert
            notifyClinet.Verify(x => x.SendSms("07415678534", "templateId", personalisation, It.IsAny<string>(), It.IsAny<string>()), Times.Once());

        }

        [Fact]
        public void GivenNoException_WhenSendSms_ThenSendSmsResponseIsReturned()
        {
            //Arrange
            var personalisation = new Dictionary<string, dynamic>
            {
                {"repair_ref", "XXXX"},
                {"appointment_time", "10.00am"}

            };
            notifyClinet.Setup(x =>
                x.SendSms(It.IsAny<string>(), It.IsAny<string>(), personalisation, It.IsAny<string>(),
                    It.IsAny<string>()));

            //Act
            systemUnderTest.SendSms("07415678534", "templateId", personalisation);

            //Assert
            notifyClinet.Verify(x => x.SendSms("07415678534", "templateId", personalisation, It.IsAny<string>(), It.IsAny<string>()), Times.Once());
        }

        [Fact]
        public void GivenNoException_WhenSendEmail_ThenSendEmailIsCalledOnClient()
        {
            //Arrange
            var personalisation = new Dictionary<string, dynamic>
            {
                {"repair_ref", "XXXX"},
                {"appointment_time", "10.00am"}

            };
            notifyClinet.Setup(x =>
                x.SendEmail(It.IsAny<string>(), It.IsAny<string>(), personalisation, It.IsAny<string>(),
                    It.IsAny<string>()));

            //Act
            systemUnderTest.SendEmail("dr.who@tardis.com", "templateId", personalisation);

            //Assert
            notifyClinet.Verify(x => x.SendEmail("dr.who@tardis.com", "templateId", personalisation, It.IsAny<string>(), It.IsAny<string>()), Times.Once());

        }

        [Fact]
        public void GivenNoException_WhenSendEmail_ThenSendEmailResponseIsReturned()
        {
            //Arrange
            var personalisation = new Dictionary<string, dynamic>
            {
                {"repair_ref", "XXXX"},
                {"appointment_time", "10.00am"}

            };
            notifyClinet.Setup(x =>
                x.SendEmail(It.IsAny<string>(), It.IsAny<string>(), personalisation, It.IsAny<string>(),
                    It.IsAny<string>()));

            //Act
            systemUnderTest.SendEmail("dr.who@tardis.com", "templateId", personalisation);

            //Assert
            notifyClinet.Verify(x => x.SendEmail("dr.who@tardis.com", "templateId", personalisation, It.IsAny<string>(), It.IsAny<string>()), Times.Once());
        }
    }
}
