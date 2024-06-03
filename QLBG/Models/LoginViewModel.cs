using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace QLBG.Models
{
    public class LoginViewModel
    {
        [Required]
        [Display(Name = "Tài khoản")]
        public string TaiKhoan { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Mật khẩu")]
        public string MatKhau { get; set; }
    }
}