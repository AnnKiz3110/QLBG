using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace QLBG.Models
{
    public class SanPham
    {
        public int ID_SanPham { get; set; }
        public string TenSanPham { get; set; }
        public int ID_ThuongHieu { get; set; }
        public int ID_DanhMuc { get; set; }
        public int ID_Anh { get; set; }
        public int ID_KichThuoc { get; set; }
        public int ID_Mau { get; set; }
        public string Mota { get; set; }
        public decimal DonViGia { get; set; }
        public int SoLuongTon { get; set; }

     
        public string AnhChinh { get; set; }
        public string Anh1 { get; set; }
        public string Anh2 { get; set; }
    }
}