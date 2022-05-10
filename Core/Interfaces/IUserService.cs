using Core.Models;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Core.Interfaces
{
    public interface IUserService
    {
        Task<(Models.User, string error)> CreateUser(CreateUser newUser);

        Task<(Models.User, string error)> GetUserById(string userId);

        Task<(IEnumerable<Models.User>, string error)> GetUsers();
    }

    public class UsersResult
    {
        public IEnumerable<Models.User> Value { get; set; }
    }

    public class CreateUser
    {
        public CreateUser(string email, string password, string displayName)
        {
            SignInNames = new[] { new SignInName { Type = "emailAddress", Value = email } };
            PasswordProfile = new PasswordProfile { Password = password };
            DisplayName = displayName;
        }

        public bool AccountEnabled { get; } = true;

        public string City { get; set; }

        public string Country { get; set; }

        public string CreationType { get; } = "LocalAccount";

        public string DisplayName { get; set; }

        public string GivenName { get; set; }

        public string PasswordPolicies { get; } = "DisablePasswordExpiration";

        public PasswordProfile PasswordProfile { get; }

        public string PostalCode { get; set; }

        public IEnumerable<SignInName> SignInNames { get; }

        public string State { get; set; }

        public string StreetAddress { get; set; }

        public string Surname { get; set; }
    }

    public class PasswordProfile
    {
        public bool ForceChangePasswordNextLogin { get; set; } = false;

        public string Password { get; set; }
    }

    public class SignInName
    {
        public string Type { get; set; }

        public string Value { get; set; }
    }

    public class BadRequestResponse
    {
        [JsonProperty(PropertyName = "odata.error")]
        public Error Error { get; set; }

        public string ErrorMessage => Error.Message.Value;
    }

    public class Error
    {
        public string Code { get; set; }

        public ErrorMessage Message { get; set; }
    }

    public class ErrorMessage
    {
        public string Value { get; set; }
    }
}