using System;
using System.Collections.Generic;
using System.Text;

namespace RealEstateApp.Models
{
    public class LoginResult
    {
        public bool Succeded { get; set; }
        public string AccessToken { get; set; }
        public string Refreshtoken { get; set; }
    }
}
