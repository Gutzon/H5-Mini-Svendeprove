using Microsoft.AspNetCore.Http;
using ServerSideData.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace RegnskabsSystem.Helpers
{
    public static class CookieHelper
    {
        public static Validation GetValidation(HttpRequest request)
        {
            if (!request.Cookies.TryGetValue("accessToken", out var accessTokenValue)
                || string.IsNullOrEmpty(accessTokenValue))
            {
                return null;
            }

            if (!request.Cookies.TryGetValue("userName", out var userName)
                || string.IsNullOrEmpty(userName))
            {
                return null;
            }

            return new Validation(userName, accessTokenValue);
        }
    }
}
