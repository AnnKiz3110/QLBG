using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace QLBG.Models
{
    public class DatHang
    {
        public int ID_DatHang { get; set; }
        public int ID_KhachHang { get; set; }
        public DateTime NgayDat { get; set; }
        public List<CTDatHang> CTDatHangs { get; set; }
    }
}