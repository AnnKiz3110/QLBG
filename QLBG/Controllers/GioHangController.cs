using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Oracle.ManagedDataAccess.Client;
using QLBG.Models; // Assuming using Oracle Managed Data Access

namespace QLBG.Controllers
{
    public class GioHangController : Controller
    {
        // GET: GioHang
        private OracleDbManager dbManager = new OracleDbManager();

        public ActionResult Index1()
        {
            return View();
        }

        public List<GioHang> LayGioHang()
        {
            string sessionId = Session.SessionID;
            return dbManager.GetGioHang(sessionId);
        }

        public ActionResult ThemGioHang(int ms, string strURL, int quantity = 1)
        {
            string sessionId = Session.SessionID;
            GioHang SanPham = dbManager.GetSanPhamInGioHang(sessionId, ms);

            if (SanPham == null)
            {
                dbManager.AddSanPhamToGioHang(sessionId, ms, quantity);
            }
            else
            {
                dbManager.UpdateSoLuongSanPham(sessionId, ms, SanPham.SoLuong + quantity);
            }

            return RedirectToAction("GioHang");
        }

        public int TongSoLuong()
        {
            string sessionId = Session.SessionID;
            return dbManager.GetTongSoLuong(sessionId);
        }

        private double ThanhTien()
        {
            string sessionId = Session.SessionID;
            return dbManager.GetThanhTien(sessionId);
        }

        public ActionResult GioHang()
        {
            string sessionId = Session.SessionID;
            List<GioHang> lstGioHang = LayGioHang();

            ViewBag.TongSoLuong = TongSoLuong();
            ViewBag.TongThanhTien = ThanhTien();
            return View(lstGioHang);
        }

        public ActionResult GioHangPar()
        {
            ViewBag.TongSoLuong = TongSoLuong();
            return View();
        }

        public ActionResult TangSoLuong(int id)
        {
            string sessionId = Session.SessionID;
            GioHang SanPham = dbManager.GetSanPhamInGioHang(sessionId, id);

            if (SanPham != null)
            {
                dbManager.UpdateSoLuongSanPham(sessionId, id, SanPham.SoLuong + 1);
            }

            return RedirectToAction("GioHang");
        }

        public ActionResult GiamSoLuong(int id)
        {
            string sessionId = Session.SessionID;
            GioHang SanPham = dbManager.GetSanPhamInGioHang(sessionId, id);

            if (SanPham != null && SanPham.SoLuong > 1)
            {
                dbManager.UpdateSoLuongSanPham(sessionId, id, SanPham.SoLuong - 1);
            }

            return RedirectToAction("GioHang");
        }

        public ActionResult XoaGioHang(int id)
        {
            string sessionId = Session.SessionID;
            dbManager.RemoveSanPhamFromGioHang(sessionId, id);

            return RedirectToAction("GioHang");
        }

        public ActionResult DatHang()
        {
            string sessionId = Session.SessionID;
            List<GioHang> lstGioHang = LayGioHang();
            int khachHangId = GetCurrentKhachHangId();

            if (lstGioHang == null || !lstGioHang.Any())
            {
                return RedirectToAction("GioHang");
            }

            double tongThanhTien = ThanhTien();
            int idDatHang = dbManager.AddDatHang(khachHangId, tongThanhTien);

            foreach (var item in lstGioHang)
            {
                dbManager.AddChiTietDatHang(idDatHang, item.ID_SanPham, item.SoLuong, item.Gia);
            }

            dbManager.ClearGioHang(sessionId);

            return View("DatHang");
        }

        private int GetCurrentKhachHangId()
        {
            // Assume this method fetches the currently logged-in user's ID
            return (int)Session["UserId"];
        }

        // Gọi hàm Thêm Đơn Hàng từ OracleDbManager
        public ActionResult ThemDonHang(int idKhachHang, DateTime ngayDat, int soLuong)
        {
            try
            {
                dbManager.ThemDonHang(idKhachHang, ngayDat, soLuong);
                ViewBag.Message = "Đã thêm đơn hàng thành công!";
            }
            catch (Exception ex)
            {
                ViewBag.Message = "Lỗi khi thêm đơn hàng: " + ex.Message;
            }

            return View("Result");
        }

        // Gọi hàm Hiển Thị Đơn Hàng từ OracleDbManager
        public ActionResult HienThiDonHang()
        {
            try
            {
                dbManager.HienThiDonHang();
                ViewBag.Message = "Đã hiển thị đơn hàng thành công!";
            }
            catch (Exception ex)
            {
                ViewBag.Message = "Lỗi khi hiển thị đơn hàng: " + ex.Message;
            }

            return View("Result");
        }

        // Gọi hàm Tính Tổng Số Lượng Sản Phẩm Theo Thương Hiệu từ OracleDbManager
        public ActionResult TinhTongSoLuongSanPhamTheoThuongHieu(int thuong_hieu_id)
        {
            try
            {
                int tongSoLuong = dbManager.TinhTongSoLuongSanPhamTheoThuongHieu(thuong_hieu_id);
                ViewBag.Message = "Tổng số lượng sản phẩm theo thương hiệu là: " + tongSoLuong;
            }
            catch (Exception ex)
            {
                ViewBag.Message = "Lỗi khi tính tổng số lượng sản phẩm: " + ex.Message;
            }

            return View("Result");
        }
    }
}

