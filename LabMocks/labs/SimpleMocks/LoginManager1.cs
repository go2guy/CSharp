using System;
using System.Collections;

namespace MyBillingProduct
{
    public class LoginManager1
    {
       
        private readonly ILogger _logger;
        private Hashtable m_Users = new Hashtable();

        public LoginManager1(ILogger logger)
        {
            _logger = logger;
        }

        public bool IsLoginOK(string user, string password)
        {
            if (m_Users[user] != null &&
                m_Users[user] == password)
            {
                _logger.Write(string.Format("login ok: user: [{0}]", user));
                return true;
            }

            _logger.Write(string.Format("bad login: [{0}],[{1}]", user, password));
            return false;
        }

        public void AddUser(string user, string password)
        {
            m_Users[user] = password;
            _logger.Write(string.Format("user added: [{0}],[{1}]", user, password));
        }

        public void ChangePass(string user, string oldPass, string newPassword)
        {
            m_Users[user] = newPassword;
            _logger.Write(string.Format("pass changed: [{0}],[{1}],[{2}]", user, newPassword, oldPass));
        }
    }
}
