using QLBG.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace QLBG.Controllers
{
    public class DanhMucController : Controller
    {
        private OracleDbManager dbManager = new OracleDbManager();

        // GET: DanhMuc 
        public ActionResult DanhMucPar()
        {
            var listCD =dbManager.GetAllDanhMucs().Take(7).ToList();
            return View(listCD);
        }
    }
}