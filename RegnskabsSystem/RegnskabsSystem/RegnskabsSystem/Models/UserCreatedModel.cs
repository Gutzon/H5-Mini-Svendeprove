using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RegnskabsSystem.Models
{
    public class UserCreatedModel
    {
        public UserCreatedModel(bool userCreated = false, string userPassword = "")
        {
            this.UserCreated = userCreated;
            this.UserPassword = userPassword;
        }

        public bool UserCreated { get; set; }

        // Note: For live environment this value should not be returned,
        // however a hosting with e-mail should be registered to send out
        // the e-mail generated to the user directly instead.
        // -- Likely with a confirmation link in the e-mail for added security.
        public string UserPassword { get; set; }
    }
}
