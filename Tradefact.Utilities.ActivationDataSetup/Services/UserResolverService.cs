using Core.Interfaces;

namespace Tradefact.Utilities.ActivationDataSetup.Services
{
    public class UserResolverService: IUserResolverService
    {
        private string _user { get; set; }
        public UserResolverService(string user)
        {
            this._user = user;
        }

        public string GetUser()
        {
            return _user;
        }
    }
}
