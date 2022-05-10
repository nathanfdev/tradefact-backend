using Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Tradefact.External.API.Helpers
{
    public class UserResolverService : IUserResolverService
    {
        public string GetUser()
        {
            return "External API";
        }
    }
}
