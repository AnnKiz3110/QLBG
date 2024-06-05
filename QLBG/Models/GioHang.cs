using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace QLBG.Models
{
    public class GioHang
    {
        private OracleDbManager dbManager = new OracleDbManager();

        public int ID_SanPham { get; set; }
        public string TenSanPham { get; set; }
        public string AnhBia { get; set; }
        public string AnhPhu1 { get; set; }
        public string AnhPhu2 { get; set; }
        public double Gia { get; set; }
        public int SoLuong { get; set; }
        public Double ThanhTien { get { return SoLuong * Gia; } set { } }

        public object DonGia { get; internal set; }

        public GioHang(int masanpham)
        {
            ID_SanPham = masanpham;
            var sp = dbManager.GetSanPham(masanpham);
            if (sp != null)
            {
                TenSanPham = sp.TenSanPham;
                AnhBia = sp.AnhChinh;
                AnhPhu1 = sp.Anh1;
                AnhPhu2 = sp.Anh2;
                Gia = (double)sp.DonViGia;
                SoLuong = 1;
            }
            else
            {
                throw new Exception("Sản phẩm không tồn tại.");
            }
        }
    }
}
