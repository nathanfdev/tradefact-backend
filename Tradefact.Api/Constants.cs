using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Tradefact.Api
{
    public static class Constants
    {
        public static class AuthenticationPropertiesKeys
        {
            public const string DomainHint = "domain_hint";

            public const string Nonce = "nonce";

            public const string UserProfileRegistrationMode = "user_profile_registration_mode";

            public const string PolicyTokenLifetime = "policy_token_lifetime";

            public const string SkipCorrelation = "skip_correlation";

            public const string UILocales = "ui_locales";

            public const string VerifiedEmail = "verified_email";
        }

        public static class PolicyIds
        {
            public const string Activation = "b2c_1a_activation";

            public const string Invitation = "b2c_1a_invitation";

            public const string InvitationLink = "b2c_1a_invitation_link";

            public const string InvitationRedirect = "b2c_1a_invitation_redirect";

            public const string Link = "b2c_1a_link";

            public const string PasswordReset = "b2c_1a_password_reset";

            public const string ProfileUpdate = "b2c_1a_profile_update_games";

            public const string SignInUsingAppCode = "b2c_1a_sign_in_games_app_code";

            public const string SignInUsingAuthyCode = "b2c_1a_sign_in_games_authy_code";

            public const string SignInUsingEmailCode = "b2c_1a_sign_in_games_email_code";

            public const string SignInUsingPhoneCode = "b2c_1a_sign_in_games_phone_code";

            public const string SignUpOrSignIn = "b2c_1a_sign_up_sign_in_games";

            public const string SignUpUsingAppCode = "b2c_1a_sign_up_games_app_code";

            public const string SignUpUsingAuthyCode = "b2c_1a_sign_up_games_authy_code";

            public const string StepUp = "b2c_1a_step_up";
        }


    }
}
