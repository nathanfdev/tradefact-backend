using FluentValidation;
using Newtonsoft.Json;
using Tradefact.Api.Model;

namespace Tradfact.Api.Requests
{

    public class BaseContactRequest
    {
        public string FullName { get; set; }
        public string Department { get; set; }
        public System.Guid LocationId { get; set; }
        public EmailRequest Email { get; set; }
        public PhoneRequest Phone { get; set; }
    }

    public class FullContactRequest: BaseContactRequest
    {
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public string LastName { get; set; }
        public string Title { get; set; }
        public string Salutation { get; set; }
        public bool IsDefault { get; set; }
        public string Notes { get; set; }
    }


    public class CreateContactRequest : BaseContactRequest
    {
    }

    public class CreateFullContactRequest: FullContactRequest
    {

    }

    public class UpdateContactRequest : BaseContactRequest
    {
    }

    public class UpdateFullContactRequest : BaseContactRequest
    {

    }

    public class BaseValidator<T> : AbstractValidator<T> where T : BaseContactRequest
    {
        public BaseValidator()
        {
            RuleFor(b => b.FullName).NotNull();
            RuleFor(x => x.LocationId).SetValidator(new GuidValidator());
        }
    }


    //public class CreateContactRequestValidator : AbstractValidator<CreateFullContactRequest>
    //{
    //    public CreateContactRequestValidator()
    //    {
    //        RuleFor(x => x.FullName).NotEmpty().Length(3, 150);
    //    }
    //}

    //public class CreateBasicContactRequestValidator : AbstractValidator<CreateContactRequest>
    //{
    //    public CreateBasicContactRequestValidator()
    //    {
    //        RuleFor(x => x.FullName).NotEmpty().Length(3, 150);
    //    }
    //}
}