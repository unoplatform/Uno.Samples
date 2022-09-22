namespace TimeEntryRia.Web
{
    using System.Security.Authentication;
    using System.ServiceModel.DomainServices.Hosting;
    using System.ServiceModel.DomainServices.Server;
    using System.ServiceModel.DomainServices.Server.ApplicationServices;
    using System.Threading;
    using System.Data;
    using System.Linq;
    using System.Security.Cryptography;
    using System.Text;
    using System;
    using System.Collections.Generic;
    using TimeEntryRia.Web.Models;

    /// <summary>
    /// RIA Services DomainService responsible for authenticating users when
    /// they try to log on to the application.
    ///
    /// Most of the functionality is already provided by the base class
    /// AuthenticationBase
    /// </summary>
    [EnableClientAccess]
    public class AuthenticationService : AuthenticationBase<User> 
    {
        private TimeEntryEntities _context;

        // override for custom implementation
        protected override bool ValidateUser(string userName, string password)
        {
            GenerateTimeEntryData.GenerateDataIfRequired();

            var timeEntryUser = _context.TimeEntryUsers.FirstOrDefault(u => u.UserName == userName);

            if (timeEntryUser == null)
            {
                return false;
            }

            var security = _context.Securities.FirstOrDefault(s => s.UserId == timeEntryUser.Id);
            if (security == null)
            {
                // for this sample, we will create a security entry for the user using the supplied password
                security = new Security()
                {
                    UserId = timeEntryUser.Id,
                    Salt = GenerateSalt()
                };

                security.Password = HashPassword(password, security.Salt);
                _context.Securities.AddObject(security);
                _context.SaveChanges();

                // In the real world, we would, of course:
                //return false;
            }

            return DoPasswordsMatch(password, security.Salt, security.Password);
            //return base.ValidateUser(userName, password);
        }

        protected override User GetAuthenticatedUser(System.Security.Principal.IPrincipal principal)
        {
            var timeEntryUser = _context.TimeEntryUsers.Include("Role").FirstOrDefault(u => u.UserName == principal.Identity.Name);

            if (timeEntryUser == null)
            {
                return base.GetAuthenticatedUser(principal);
            }

            var user = new User()
            {
                Name = timeEntryUser.UserName,
                FriendlyName = timeEntryUser.DisplayName,
                Id = timeEntryUser.Id,
            };

            var roles = new List<string>();
            switch (timeEntryUser.Role.Name)
            {
                case TimeEntryRoles.Admin:
                    roles.Add(TimeEntryRoles.Admin);
                    roles.Add(TimeEntryRoles.Consultant);
                    break;

                case TimeEntryRoles.Consultant:
                    roles.Add(TimeEntryRoles.Consultant);
                    roles.Add(TimeEntryRoles.ReportViewer);
                    break;

                case TimeEntryRoles.ReportViewer:
                    roles.Add(TimeEntryRoles.ReportViewer);
                    break;
                default:
                    break;
            }

            user.Roles = roles;

            return user;
        }

        public override void Initialize(DomainServiceContext context)
        {
            _context = new TimeEntryEntities();
            base.Initialize(context);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _context.Dispose();
            }

            base.Dispose(disposing);
        }


        private static string GenerateSalt()
        {
            var bytes = new byte[128 / 8];
            var rng = new RNGCryptoServiceProvider();
            rng.GetBytes(bytes);
            return Convert.ToBase64String(bytes);
        }

        private static bool DoPasswordsMatch(string incomingPassword,string salt, string storedHash)
        {
            var incomingPasswordHash = HashPassword(incomingPassword, salt);
            return incomingPasswordHash == storedHash;
        }

        private static string HashPassword(string password, string salt)
        {
            return ComputeHash(Encoding.ASCII.GetBytes(password), Encoding.ASCII.GetBytes(salt));
        }

        private static string ComputeHash(byte[] bytesToHash, byte[] salt)
        {
            var byteResult = new Rfc2898DeriveBytes(bytesToHash, salt, 10000);
            return Convert.ToBase64String(byteResult.GetBytes(24));
        }
    }
}
