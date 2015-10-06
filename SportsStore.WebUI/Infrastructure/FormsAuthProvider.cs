using System;
using System.Security.Principal;
using System.Web.Security;
using SportsStore.WebUI.Infrastructure.Abstract;

namespace SportsStore.WebUI.Infrastructure
{
    public class FormsAuthProvider : IAuthProvider
    {
        public bool Authenticate(string username, string password)
        {
            bool result = FormsAuthentication.Authenticate(username, password);
            if (result)
            {
                FormsAuthentication.SetAuthCookie(username, false);
            }
            return result;
        }

        public bool IsAuthenticated(IPrincipal user)
        {
            return user.Identity.IsAuthenticated;
        }

        public void Logout()
        {
            FormsAuthentication.SignOut();
        }
    }
}