using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace TimeEntryApi.Models
{
    public class User : IPrincipal, IIdentity
    {
        private bool _isAuthenticated;

        public int Id { get; set; }

        public bool IsAuthenticated => _isAuthenticated;

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

        [JsonIgnore]
        public IIdentity Identity => this;

        public void Authenticate()
        {
            _isAuthenticated = true;
        }

    }
}
