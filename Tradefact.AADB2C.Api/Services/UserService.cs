using Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Tradefact.AADB2C.Api.Services
{
    public class UserService : IUserResolverService
    {

        public string GetUser()
        {
            return "AADB2C.Api";
        }
    }
}
