using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RegnskabsSystem.Models
{
    public class LoginModel
    {
        public string user { get; set; }
        public string password { get; set; }
        public string GetUnEscapedPassword => Uri.UnescapeDataString(password ?? "");
    }
}