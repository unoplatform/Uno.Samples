using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;

namespace TimeEntryUno.Shared.Models
{
    public class User : IPrincipal, IIdentity
    {
        public int Id { get; set; }

        public bool IsAuthenticated { get; set; }

        public string Name { get; set; }

        public IEnumerable<string> Roles { get; set; }

        public bool IsInRole(string role)
        {
            return Roles.Contains(role);
        }
        public string FriendlyName { get; set; }

        public string DisplayName
        {
            get
            {
                if (!string.IsNullOrEmpty(this.FriendlyName))
                {
                    return this.FriendlyName;
                }
                else
                {
                    return this.Name;
                }
            }
        }

        public string AuthenticationType => "Custom";

        public IIdentity Identity => this;
    }
}
