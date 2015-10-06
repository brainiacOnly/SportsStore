﻿using System.Security.Principal;

namespace SportsStore.WebUI.Infrastructure.Abstract
{
    public interface IAuthProvider
    {
        bool Authenticate(string username, string password);

        void Logout();

        bool IsAuthenticated(IPrincipal user);
    }
}
