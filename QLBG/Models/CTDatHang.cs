using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace QLBG.Models
{
    public class CTDatHang
    {
        public int ID_CTDatHang { get; set; }
        public int ID_DatHang { get; set; }
        public int ID_SanPham { get; set; }
        public int SoLuong { get; set; }
        public decimal DonViGia { get; set; }
    }
}