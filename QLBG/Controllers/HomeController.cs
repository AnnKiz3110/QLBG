using Oracle.ManagedDataAccess.Client;
using QLBG.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlTypes;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace QLBG.Controllers
{
    public class HomeController : Controller
    {
        private OracleDbManager dbManager = new OracleDbManager();

        public ActionResult Index()
        {
            return View();
        }
        public ActionResult Index1()
        {
            return View();
        }
        public ActionResult about()
        {
            return View();
        }
        public ActionResult contact()
        {
            return View();
        }
        public ActionResult shopsingle()
        {

            return View();
        }
        public ActionResult shop(int page = 1, int pageSize = 9)
        {
            var sanPhams = GetSanPhams();

            // Tính toán số trang
            var totalProducts = sanPhams.Count();
            var totalPages = (int)Math.Ceiling((double)totalProducts / pageSize);

            // Phân trang sản phẩm
            var pagedSanPhams = sanPhams.Skip((page - 1) * pageSize).Take(pageSize).ToList();

            // Truyền thông tin phân trang và danh sách sản phẩm đến view
            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = totalPages;

            return View(pagedSanPhams);
        }
        private List<SanPham> GetSanPhams()
        {
            var sanPhams = new List<SanPham>();

            string query = @"
        SELECT sp.ID_SanPham, sp.TenSanPham, sp.ID_ThuongHieu, sp.ID_DanhMuc, sp.ID_Anh, sp.ID_KichThuoc, sp.ID_Mau, sp.Mota, sp.DonViGia, sp.SoLuongTon,
               ha.AnhChinh, ha.Anh1, ha.Anh2
        FROM SANPHAM sp
        JOIN HINHANH ha ON sp.ID_Anh = ha.ID_Anh";

            DataTable dataTable = dbManager.ExecuteQuery(query);

            foreach (DataRow row in dataTable.Rows)
            {
                var sanPham = new SanPham
                {
                    ID_SanPham = Convert.ToInt32(row["ID_SanPham"]),
                    TenSanPham = row["TenSanPham"].ToString(),
                    ID_ThuongHieu = Convert.ToInt32(row["ID_ThuongHieu"]),
                    ID_DanhMuc = Convert.ToInt32(row["ID_DanhMuc"]),
                    ID_Anh = Convert.ToInt32(row["ID_Anh"]),
                    ID_KichThuoc = Convert.ToInt32(row["ID_KichThuoc"]),
                    ID_Mau = Convert.ToInt32(row["ID_Mau"]),
                    Mota = row["Mota"].ToString(),
                    DonViGia = Convert.ToDecimal(row["DonViGia"]),
                    SoLuongTon = Convert.ToInt32(row["SoLuongTon"]),
                    AnhChinh = row["AnhChinh"].ToString(),
                    Anh1 = row["Anh1"].ToString(),
                    Anh2 = row["Anh2"].ToString()
                };
                sanPhams.Add(sanPham);
            }

            return sanPhams;
        }

        public ActionResult ViewProfile()
        {
            // Lấy thông tin người dùng từ Session
            string taiKhoan = Session["TaiKhoan"] as string;
            if (taiKhoan == null)
            {
                return RedirectToAction("Login"); // Chuyển hướng đến trang đăng nhập nếu chưa đăng nhập
            }

            // Lấy thông tin người dùng từ cơ sở dữ liệu
            var user = dbManager.GetUserByTaiKhoan(taiKhoan);

            // Truyền thông tin người dùng tới view
            return View(user);
        }

        // Phương thức chỉnh sửa hồ sơ người dùng (GET)
        public ActionResult Edit()
        {
            // Lấy thông tin người dùng từ Session
            string taiKhoan = Session["TaiKhoan"] as string;
            if (taiKhoan == null)
            {
                return RedirectToAction("Login"); // Chuyển hướng đến trang đăng nhập nếu chưa đăng nhập
            }

            // Lấy thông tin người dùng từ cơ sở dữ liệu
            var user = dbManager.GetUserByTaiKhoan(taiKhoan);

            // Truyền thông tin người dùng tới view
            return View(user);
        }

        // Phương thức chỉnh sửa hồ sơ người dùng (POST)
        [HttpPost]
        public ActionResult Edit(User updatedProfile)
        {
            if (ModelState.IsValid)
            {
                string taiKhoan = Session["TaiKhoan"] as string;
                var user = dbManager.GetUserByTaiKhoan(taiKhoan);

                if (user == null)
                {
                    return HttpNotFound();
                }

                if (IsValidDate(updatedProfile.NgaySinh))
                {
                    user.HoTen = updatedProfile.HoTen;
                    user.Email = updatedProfile.Email;
                    user.SoDienThoai = updatedProfile.SoDienThoai;

                    if (updatedProfile.NgaySinh.HasValue && updatedProfile.NgaySinh.Value != DateTime.MinValue)
                    {
                        user.NgaySinh = updatedProfile.NgaySinh.Value.Date;
                    }
                    else
                    {
                        user.NgaySinh = null;
                    }

                    user.DiaChi = updatedProfile.DiaChi;
                    user.GioiTinh = updatedProfile.GioiTinh;
                    user.Role = updatedProfile.Role;

                    // Save changes to the database
                    try
                    {
                        if (dbManager.UpdateUser(user))
                        {
                            return RedirectToAction("ViewProfile");
                        }
                        else
                        {
                            ModelState.AddModelError("", "Cập nhật không thành công. Không có dòng nào được cập nhật.");
                        }
                    }
                    catch (OracleException ex)
                    {
                        if (ex.Number == -20001) // Custom error code from trigger
                        {
                            ModelState.AddModelError("Email", ex.Message);
                        }
                        else
                        {
                            ModelState.AddModelError("", $"Lỗi cơ sở dữ liệu: {ex.Message}");
                        }
                    }
                    catch (Exception ex)
                    {
                        ModelState.AddModelError("", $"Lỗi hệ thống: {ex.Message}");
                    }
                }
                else
                {
                    ModelState.AddModelError("NgaySinh", "Ngày sinh không hợp lệ.");
                }
            }
            else
            {
                ModelState.AddModelError("", "Dữ liệu không hợp lệ.");
            }

            // If ModelState is not valid, return to the edit view
            return View(updatedProfile);
        }

        // Hàm kiểm tra giá trị ngày tháng hợp lệ
        private bool IsValidDate(DateTime? date)
        {
            return date.HasValue && date >= (DateTime)System.Data.SqlTypes.SqlDateTime.MinValue && date <= (DateTime)System.Data.SqlTypes.SqlDateTime.MaxValue;
        }


    }
}