using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RegnskabsSystem.Models
{
    public class LoginModel
    {
        public string User { get; set; }
        public string Password { get; set; }
        public string GetUnEscapedPassword => Uri.UnescapeDataString(Password ?? "");
    }
}