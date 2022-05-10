using Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace Tradefact.Utilities.DataImport.Services
{
    public class ImportUserService : IUserResolverService
    {
        private string _user { get; set; }
        public ImportUserService(string user)
        {
            this._user = user;
        }

        public string GetUser()
        {
            return _user;
        }
    }
}
