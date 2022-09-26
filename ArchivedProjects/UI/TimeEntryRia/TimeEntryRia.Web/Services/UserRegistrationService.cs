namespace TimeEntryRia.Web
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ServiceModel.DomainServices.Hosting;
    using System.ServiceModel.DomainServices.Server;
    using System.Web.Profile;
    using System.Web.Security;
    using TimeEntryRia.Web.Resources;

    /// <summary>
    ///   RIA Services Domain Service that exposes methods for performing user
    ///   registrations.
    /// </summary>
    [EnableClientAccess]
    public class UserRegistrationService : DomainService
    {
        /// <summary>
        /// Role to which users will be added by default.
        /// </summary>
        public const string DefaultRole = "Registered Users";

        //// NOTE: This is a sample code to get your application started. In the production code you would 
        //// want to provide a mitigation against a denial of service attack by providing CAPTCHA 
        //// control functionality or verifying user's email address.

        /// <summary>
        /// Adds a new user with the supplied <see cref="RegistrationData"/> and <paramref name="password"/>.
        /// </summary>
        /// <param name="user">The registration information for this user.</param>
        /// <param name="password">The password for the new user.</param>
        [Invoke(HasSideEffects = true)]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic")]
        public CreateUserStatus CreateUser(RegistrationData user,
            [Required(ErrorMessageResourceName = "ValidationErrorRequiredField", ErrorMessageResourceType = typeof(ValidationErrorResources))]
            [RegularExpression("^.*[^a-zA-Z0-9].*$", ErrorMessageResourceName = "ValidationErrorBadPasswordStrength", ErrorMessageResourceType = typeof(ValidationErrorResources))]
            [StringLength(50, MinimumLength = 7, ErrorMessageResourceName = "ValidationErrorBadPasswordLength", ErrorMessageResourceType = typeof(ValidationErrorResources))]
            string password)
        {
            if (user == null)
            {
                throw new ArgumentNullException("user");
            }

            // Run this BEFORE creating the user to make sure roles are enabled and the default role
            // will be available
            //
            // If there are a problem with the role manager it is better to fail now than to have it
            // happening after the user is created
            if (!Roles.RoleExists(UserRegistrationService.DefaultRole))
            {
                Roles.CreateRole(UserRegistrationService.DefaultRole);
            }

            // NOTE: ASP.NET by default uses SQL Server Express to create the user database. 
            // CreateUser will fail if you do not have SQL Server Express installed.
            MembershipCreateStatus createStatus;
            Membership.CreateUser(user.UserName, password, user.Email, user.Question, user.Answer, true, null, out createStatus);

            if (createStatus != MembershipCreateStatus.Success)
            {
                return UserRegistrationService.ConvertStatus(createStatus);
            }

            // Assign it to the default role
            // This *can* fail but only if role management is disabled
            Roles.AddUserToRole(user.UserName, UserRegistrationService.DefaultRole);

            // Set its friendly name (profile setting)
            // This *can* fail but only if Web.config is configured incorrectly 
            ProfileBase profile = ProfileBase.Create(user.UserName, true);
            profile.SetPropertyValue("FriendlyName", user.FriendlyName);
            profile.Save();

            return CreateUserStatus.Success;
        }


        /// <summary>
        /// Query method that exposes the <see cref="RegistrationData"/> class to Silverlight client code.
        /// </summary>
        /// <remarks>
        /// This query method is not used and will throw <see cref="NotSupportedException"/> if called.
        /// Its primary job is to indicate the <see cref="RegistrationData"/> class should be made
        /// available to the Silverlight client.
        /// </remarks>
        /// <returns>Not applicable.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic"), System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate")]
        public IEnumerable<RegistrationData> GetUsers()
        {
            throw new NotSupportedException();
        }

        private static CreateUserStatus ConvertStatus(MembershipCreateStatus createStatus)
        {
            switch (createStatus)
            {
                case MembershipCreateStatus.Success: return CreateUserStatus.Success;
                case MembershipCreateStatus.InvalidUserName: return CreateUserStatus.InvalidUserName;
                case MembershipCreateStatus.InvalidPassword: return CreateUserStatus.InvalidPassword;
                case MembershipCreateStatus.InvalidQuestion: return CreateUserStatus.InvalidQuestion;
                case MembershipCreateStatus.InvalidAnswer: return CreateUserStatus.InvalidAnswer;
                case MembershipCreateStatus.InvalidEmail: return CreateUserStatus.InvalidEmail;
                case MembershipCreateStatus.DuplicateUserName: return CreateUserStatus.DuplicateUserName;
                case MembershipCreateStatus.DuplicateEmail: return CreateUserStatus.DuplicateEmail;
                default: return CreateUserStatus.Failure;
            }
        }
    }

    /// <summary>
    /// An enumeration of the values that can be returned from <see cref="UserRegistrationService.CreateUser"/>
    /// </summary>
    public enum CreateUserStatus
    {
        Success = 0,
        InvalidUserName = 1,
        InvalidPassword = 2,
        InvalidQuestion = 3,
        InvalidAnswer = 4,
        InvalidEmail = 5,
        DuplicateUserName = 6,
        DuplicateEmail = 7,
        Failure = 8,
    }
}