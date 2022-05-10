using Core.Interfaces;

namespace cnsActivateAccount.Services
{
    public class UserResolverService : IUserResolverService
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
