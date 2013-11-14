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
    public class LoginManager1Tests
    {
        [Test]
        public void IsLoginOK_UserOK_CallLogger()
        {
            var mockLogger = new FakeLogger();

            var loginManager = new LoginManager1(mockLogger);

            loginManager.AddUser("somename", "password");           
            loginManager.IsLoginOK("somename", "password");

            StringAssert.Contains("login ok: user: [somename]", mockLogger.WriteMessage);
        }

        [Test]
        public void IsLoginOK_InvalidLogin_CallLogger()
        {
            var mockLogger = new FakeLogger();

            var loginManager = new LoginManager1(mockLogger);

            loginManager.AddUser("somename", "password");
            loginManager.IsLoginOK("somename2", "password");

            StringAssert.Contains("bad login: [somename2],[password]", mockLogger.WriteMessage);
        }

        [Test]
        public void AddUser_Always_CallLogger()
        {
            var mockLogger = new FakeLogger();

            var loginManager = new LoginManager1(mockLogger);

            loginManager.AddUser("somename", "password");
  
            StringAssert.Contains("user added: [somename],[password]", mockLogger.WriteMessage);
        }

        [Test]
        public void ChangeUser_Always_CallLogger()
        {
            var mockLogger = new Mock<ILogger>();
            
            var loginManager = new LoginManager1(mockLogger.Object);
            
            loginManager.AddUser("somename", "password");
            loginManager.ChangePass("somename", "password", "password2");

            mockLogger.Verify(_ => _.Write("pass changed: [somename],[password2],[password]"));            
        }
    }

    public class FakeLogger : ILogger
    {
        public void Write(string text)
        {
            WriteMessage = text;
        }

        public string WriteMessage { get; set; }
    }
}
