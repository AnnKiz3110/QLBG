using Oracle.ManagedDataAccess.Client;
using QLBG.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace QLBG.Controllers
{
    public class GiayController : Controller
    {
        private string connectionString = ConfigurationManager.ConnectionStrings["OracleDbContext"].ConnectionString;

        public ActionResult XemChiTietSanPham(int mg)
        {
            SanPham sanpham = null;
            using (var connection = new OracleConnection(connectionString))
            {
                string query = "SELECT * FROM SANPHAM WHERE ID_SanPham = :mg";
                using (var command = new OracleCommand(query, connection))
                {
                    command.Parameters.Add(new OracleParameter("mg", mg));
                    connection.Open();
                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            sanpham = new SanPham
                            {
                                ID_SanPham = reader.GetInt32(reader.GetOrdinal("ID_SanPham")),
                                TenSanPham = reader.GetString(reader.GetOrdinal("TenSanPham")),
                                // Populate other properties of SanPham
                            };
                        }
                    }
                }
            }

            if (sanpham == null)
            {
                return HttpNotFound();
            }
            return View(sanpham);
        }

        public ActionResult Timgiay(string txt_Search)
        {
            List<SanPham> products = new List<SanPham>();

            using (var connection = new OracleConnection(connectionString))
            {
                string query = string.IsNullOrEmpty(txt_Search)
                    ? "SELECT * FROM SANPHAM"
                    : "SELECT * FROM SANPHAM WHERE TenSanPham LIKE :txt_Search";

                using (var command = new OracleCommand(query, connection))
                {
                    if (!string.IsNullOrEmpty(txt_Search))
                    {
                        command.Parameters.Add(new OracleParameter("txt_Search", $"%{txt_Search}%"));
                    }

                    connection.Open();
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var sanpham = new SanPham
                            {
                                ID_SanPham = reader.GetInt32(reader.GetOrdinal("ID_SanPham")),
                                TenSanPham = reader.GetString(reader.GetOrdinal("TenSanPham")),
                                // Populate other properties of SanPham
                            };
                            products.Add(sanpham);
                        }
                    }
                }
            }

            return View(products);
        }

        public ActionResult GiayTheoDM(int MaDM)
        {
            List<SanPham> products = new List<SanPham>();

            using (var connection = new OracleConnection(connectionString))
            {
                string query = @"
            SELECT SP.ID_SanPham, SP.TenSanPham, SP.DonViGia, HA.AnhChinh
            FROM SANPHAM SP
            JOIN HINHANH HA ON SP.ID_Anh = HA.ID_Anh
            WHERE SP.ID_DanhMuc = :MaDM
            ORDER BY SP.DonViGia";

                using (var command = new OracleCommand(query, connection))
                {
                    command.Parameters.Add(new OracleParameter("MaDM", MaDM));

                    connection.Open();
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var sanpham = new SanPham
                            {
                                ID_SanPham = reader.GetInt32(reader.GetOrdinal("ID_SanPham")),
                                TenSanPham = reader.GetString(reader.GetOrdinal("TenSanPham")),
                                DonViGia = reader.GetDecimal(reader.GetOrdinal("DonViGia")), // Assuming DonViGia is decimal
                                AnhChinh = reader.GetString(reader.GetOrdinal("AnhChinh"))
                            };
                            products.Add(sanpham);
                        }
                    }
                }
            }

            if (products.Count == 0)
            {
                ViewBag.TB = "Không có loại giày này";
            }

            return View(products);
        }


    }
}
