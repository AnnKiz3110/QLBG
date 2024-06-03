using QLBG.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace QLBG.Controllers
{
    public class DangNhapController : Controller
    {
        // GET: DangNhap
        private OracleDbManager dbManager = new OracleDbManager();

        // GET: Account/Login
        public ActionResult Login()
        {
            return View();
        }

        // POST: Account/Login
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Login(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                User user = dbManager.GetUser(model.TaiKhoan, model.MatKhau);
                if (user != null)
                {
                    // Lưu thông tin người dùng vào Session
                    Session["UserID"] = user.ID_KhachHang;
                    Session["UserName"] = user.HoTen;
                    Session["TaiKhoan"] = user.TaiKhoan;

                    // Chuyển hướng đến trang chính
                    // Chuyển hướng đến trang chính hoặc trang admin
                    if (user.Role == 1)
                    {
                        return RedirectToAction("Index", "Admin");
                    }
                    else
                    {
                        return RedirectToAction("Index", "Home");
                    }
                }

                else
                {
                    ModelState.AddModelError("", "Tài khoản hoặc mật khẩu không đúng.");
                }
            }

            return View(model);
        }

        // GET: Account/Logout
        public ActionResult Logout()
        {
            // Xóa thông tin người dùng khỏi Session
            Session.Clear();

            // Chuyển hướng đến trang đăng nhập
            return RedirectToAction("Login");
        }

        public ActionResult Register()
        {
            return View();
        }

        // POST: Account/Register
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = new User
                {
                    HoTen = model.HoTen,
                    NgaySinh = model.NgaySinh,
                    DiaChi = model.DiaChi,
                    GioiTinh = model.GioiTinh,
                    Email = model.Email,
                    SoDienThoai = model.SoDienThoai,
                    TaiKhoan = model.TaiKhoan,
                    MatKhau = model.MatKhau,
                    Role = 0 // Assuming default role is user
                };

                string result = dbManager.RegisterUser(user);
                if (result == "Thành công")
                {
                    ViewBag.SuccessMessage = "Đăng kí tài khoản thành công";
                    return RedirectToAction("Login");
                }
                else
                {
                    ModelState.AddModelError("", result);
                }
            }
            return View(model);
        }
    }
}