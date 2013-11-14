using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Moq;
using MyBillingProduct;
using NUnit.Framework;

namespace SimpleMocks.tests
{
    [TestFixture]
    public class LoginManager2Tests
    {
        [Test]
        public void IsLoginOK_LoginOk_CallsLogger()
        {
            var mockLogger = new Mock<ILogger>();
            var stubWebService = new Mock<IWebService>();

            var loginManager = GetLoginManager2(mockLogger, stubWebService);

            loginManager.AddUser("somename", "password");  
            loginManager.IsLoginOK("somename", "password");

            mockLogger.Verify(_ => _.Write("login ok: user: [somename]"));  
        }
        
        [Test]
        public void IsLoginOK_OnLoginSuccessfulLoggingFails_CallsWebService()
        {
            var stubLogger = new Mock<ILogger>();
            var mockWebService = new Mock<IWebService>();

            stubLogger.Setup(_ => _.Write("login ok: user: [somename]")).Throws(new Exception("Some Message"));
            var loginManager = GetLoginManager2(stubLogger, mockWebService);

            loginManager.AddUser("somename", "password");
            loginManager.IsLoginOK("somename", "password");

            mockWebService.Verify(_ => _.Write("Some Message"));
        }

        private static LoginManager2 GetLoginManager2(Mock<ILogger> logger, Mock<IWebService> webservice)
        {
            return new LoginManager2(logger.Object, webservice.Object);
        }

    }
}
