using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using TimeEntryApi.Helpers;
using TimeEntryApi.Models;

namespace TimeEntryApi.Controllers
{
    [Route("[controller]")]
    [ApiController]
    [Authorize]
    public class IdentityController : ControllerBase
    {
        [HttpGet("getusers")]
        public IActionResult GetUsers()
        {
            //return new JsonResult(from c in User.Claims select new { c.Type, c.Value });

            using (var context = new TimeEntryContext())
            {
                var users = context.TimeEntryUsers.Select(u => new { u.Id, u.UserName }).ToList();
                return new JsonResult(users);
            }
        }

        [HttpPost("validateuser")]
        public IActionResult ValidateUser(UserRequest userRequest)
        {
            using (var context = new TimeEntryContext())
            {
                GenerateTimeEntryData.GenerateDataIfRequired();

                var timeEntryUser = context.TimeEntryUsers.FirstOrDefault(u => u.UserName == userRequest.UserName);

                if (timeEntryUser == null)
                {
                    return Ok(false);
                }

                var security = context.Securities.FirstOrDefault(s => s.UserId == timeEntryUser.Id);
                if (security == null)
                {
                    // for this sample, we will create a security entry for the user using the supplied password
                    security = new Security()
                    {
                        UserId = timeEntryUser.Id,
                        Salt = GenerateSalt()
                    };

                    security.Password = HashPassword(userRequest.Password, security.Salt);
                    context.Securities.Add(security);
                    context.SaveChanges();

                    // In the real world, we would, of course:
                    //return Ok(false);
                }

                return Ok(DoPasswordsMatch(userRequest.Password, security.Salt, security.Password));
            }

        }

        [HttpPost("getauthenticateduser")]
        public IActionResult GetAuthenticatedUser(UserRequest userRequest)
        {
            using (var context = new TimeEntryContext())
            {
                var timeEntryUser = context.TimeEntryUsers.Include("Role").FirstOrDefault(u => u.UserName == userRequest.UserName);

                if (timeEntryUser == null)
                {
                    return NotFound();
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
                user.Authenticate();


                return Ok(user);
            }
        }

        private static string GenerateSalt()
        {
            var bytes = new byte[128 / 8];
            var rng = new RNGCryptoServiceProvider();
            rng.GetBytes(bytes);
            return Convert.ToBase64String(bytes);
        }

        private static bool DoPasswordsMatch(string incomingPassword, string salt, string storedHash)
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
