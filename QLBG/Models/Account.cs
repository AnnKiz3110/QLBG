using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace QLBG.Models
{
    public class Account
    {
        public string Username { get; set; }
        public string Password { get; set; }
        public List<string> Roles { get; set; } // Danh sách quyền của người dùng

    }
}