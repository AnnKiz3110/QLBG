﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using QLBG.Models;

namespace QLBG.Controllers
{
    public class AdminController : Controller
    {
        // GET: Admin
        private OracleDbManager dbManager = new OracleDbManager();

        public ActionResult Index()
        {
            return View();
        }
        public ActionResult ShowDanhMuc()
        {
            var danhMucList = dbManager.GetAllDanhMucs(); // Sử dụng phương thức lấy danh mục từ OracleDbManager
            return View(danhMucList);
        }
        public ActionResult CreateDM()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateDM(DanhMuc danhMuc)
        {
            if (dbManager.DanhMucExists(danhMuc.ID_DanhMuc))
            {
                ViewBag.TB = "Trùng ID!";
                return View();
            }

            if (ModelState.IsValid)
            {
                dbManager.InsertDanhMuc(danhMuc);
                ViewBag.TB = null;
                return RedirectToAction("ShowDanhMuc");
            }

            return View(danhMuc);
        }

        public ActionResult EditDM(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var danhMuc = dbManager.GetDanhMucById(id.Value);

            if (danhMuc == null)
            {
                return HttpNotFound();
            }

            return View(danhMuc);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditDM(DanhMuc model)
        {
            if (ModelState.IsValid)
            {
                var updated = dbManager.UpdateDanhMuc(model);

                if (updated)
                {
                    return RedirectToAction("ShowDanhMuc");
                }
            }

            return View(model);
        }

        public ActionResult DeleteDM(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var danhMuc = dbManager.GetDanhMucById(id.Value);

            if (danhMuc == null)
            {
                return HttpNotFound();
            }

            return View(danhMuc);
        }

        [HttpPost, ActionName("DeleteDM")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmedDM(int id)
        {
            var deleted = dbManager.DeleteDanhMuc(id);

            if (deleted)
            {
                return RedirectToAction("ShowDanhMuc");
            }

            return HttpNotFound();
        }


        public ActionResult ShowUser()
        {
            var users = dbManager.GetAllUsers();
            return View(users);
        }

        public ActionResult ShowDetailUser(int id)
        {
            var user = dbManager.GetUserById(id);
            if (user == null)
            {
                return HttpNotFound();
            }

            if (user.Role == 0)
            {
                Session["role"] = "Người dùng";
            }
            else
            {
                Session["role"] = "Admin";
            }

            return View(user);
        }

        public ActionResult EditUser(int id)
        {
            var user = dbManager.GetUserById(id);
            if (user == null)
            {
                return HttpNotFound();
            }

            return View(user);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditUser(User user)
        {
            if (ModelState.IsValid)
            {
                var existingUser = dbManager.GetUserById(user.ID_KhachHang);
                if (existingUser == null)
                {
                    return HttpNotFound();
                }

                dbManager.UpdateUser(user);

                return RedirectToAction("ShowUser");
            }

            return View(user);
        }
        public ActionResult DeleteUser(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var user = dbManager.GetUserById(id.Value);

            if (user == null)
            {
                return HttpNotFound();
            }

            return View(user);
        }

        // POST: Admin/DeleteUser/5
        [HttpPost, ActionName("DeleteUser")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmedUser(int id)
        {
            var deleted = dbManager.DeleteUser(id);

            if (deleted)
            {
                return RedirectToAction("ShowUser");
            }

            return HttpNotFound();
        }

        public ActionResult ShowListProduct(int page = 1, int pageSize = 6)
        {
            var listED = dbManager.GetAllSanPhams()
                         .Select(a => new SanPham()
                         {
                             ID_SanPham = a.ID_SanPham,
                             TenSanPham = a.TenSanPham,
                             AnhChinh = dbManager.GetAnhById(a.ID_Anh)?.AnhChinh, // Assuming GetAnhById method exists
                             Mota = a.Mota,
                             DonViGia = a.DonViGia,
                             SoLuongTon = a.SoLuongTon
                         }).ToList();

            var totalItems = listED.Count();
            var model = listED.OrderBy(s => s.ID_SanPham).Skip((page - 1) * pageSize).Take(pageSize).ToList();
            ViewBag.TotalItems = totalItems;
            ViewBag.CurrentPage = page;
            ViewBag.PageSize = pageSize;
            ViewBag.TotalPages = (int)Math.Ceiling((double)totalItems / pageSize);
            return View(model);
        }
        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(SanPham sanPham)
        {
            if (dbManager.SanPhamExists(sanPham.ID_SanPham))
            {
                ViewBag.TB = "Trùng id!!!";
                return View();
            }

            if (ModelState.IsValid)
            {
                dbManager.InsertSanPham(sanPham);
                return RedirectToAction("ShowListProduct");
            }

            return View(sanPham);
        }
        public ActionResult Edit(int id)
        {
            var sanPham = dbManager.GetSanPhamById(id);

            if (sanPham == null)
            {
                return HttpNotFound();
            }

            return View(sanPham);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(SanPham sanPham)
        {
            if (ModelState.IsValid)
            {
                var existingSanPham = dbManager.GetSanPhamById(sanPham.ID_SanPham);

                if (existingSanPham != null)
                {
                    dbManager.UpdateSanPham(sanPham);
                    return RedirectToAction("ShowListProduct");
                }
            }

            return View(sanPham);
        }

        public ActionResult Delete(int id)
        {
            var sanPham = dbManager.GetSanPhamById(id);

            if (sanPham == null)
            {
                return HttpNotFound();
            }

            return View(sanPham);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            var sanPham = dbManager.GetSanPhamById(id);

            if (sanPham == null)
            {
                return HttpNotFound();
            }

            var CTDathang = dbManager.GetCTDathangBySanPhamId(id); // Assuming GetCTDathangBySanPhamId method exists
            dbManager.DeleteAllCTDathang(id); // Assuming DeleteAllCTDathang method exists

            dbManager.DeleteSanPham(id);
            return RedirectToAction("ShowListProduct");
        }

       

    }
}




