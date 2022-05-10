using Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace Infrastructure.Functions.Helpers
{
    public class UserResolverService : IUserResolverService
    {
        public string GetUser()
        {
            return Environment.GetEnvironmentVariable("ApplicationId", EnvironmentVariableTarget.Process); ;
        }
    }
}
