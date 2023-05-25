using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Antlr.Runtime;
using MvcBookStore.Models;
using PagedList;
using PagedList.Mvc;

namespace MvcBookStore.Controllers
{
    public class AdminController : Controller
    {
        QLBANSACHEntities data = new QLBANSACHEntities();
        // GET: Admin
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult Sach()
        {
            int pageNumber = (page ?? 1);
            int pageSize = 7;
            return View(data.SACHes.ToList().OrderBy(n => n.Masach).ToPagedList(pageNumber, pageNumber));
        }
        [HttpGet]
        public ActionResult Login()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Login(FormCollection collection)
        {
            //Gán các giá trị người dùng nhập dữ liệu cho các biến
            var tendn = collection["username"];
            var matkhau = collection["password"];
            if (String.IsNullOrEmpty(tendn))
            {
                ViewData["Loi1"] = "Phải nhập tên đăng nhập";
            }    
            else
            {
                //Gán giá trị cho đối tượng được tạo mới (ad)
                Admin ad = data.Admins.SingleOrDefault(n => n.UserAdmin == tendn && n.PassAdmin == matkhau);
                if (ad != null)
                {
                    //ViewBag.Thongbao = "Chúc mừng đăng nhập thành công";
                    Session["Taikhoanadmin"] = ad;
                    return RedirectToAction("Index", "Admin");
                }
                else
                    ViewBag.Thongbao = "Tên đăng nhập hoặc mật khẩu không đúng";
            }    
            return View();
        }
        [HttpGet]
        public ActionResult ThemmoiSach()
        {
            ViewBag.MaCD = new SelectList(data.CHUDEs.ToList().OrderBy(n => n.TenChuDe), "MaCD", "TenChude");
            ViewBag.MaNXB = new SelectList(data.NHAXUATBANs.ToList().OrderBy(n => n.TenNXB), "MaNXB", "TenNXB");
            return View();
        }
        [HttpPost]
        [ValidateInput(false)]
        public ActionResult ThemmoiSach(SACH sach, HttpPostedFileBase fileupload)
        {
            //Đưa dữ liệu vào dropdownload
            ViewBag.MaCD = new SelectList(data.CHUDEs.ToList().OrderBy(n => n.TenChuDe), "MaCD", "TenChude");
            ViewBag.MaNXB = new SelectList(data.NHAXUATBANs.ToList().OrderBy(n => n.TenNXB), "MaNXB", "TenNXB");
            //Kiểm tra đường dẫn file
            if( fileupload == null)
            {
                ViewBag.Thongbao = "Vui lòng chọn ảnh bìa";
                return View();
            }
            //Thêm vào CSDL
            else
            {
                if (ModelState.IsValid)
                {
                    //Lưu tên file lưu ý bổ sung thư viện using System.IO;
                    var filename = Path.GetFileName(fileupload.FileName);
                    //Lưu đường dẫn của file 
                    var path = Path.Combine(Sever.MapPath("~/imgs"), filename);
                    //Kiểm tra hình ảnh tồn tại chưa?
                    if (System.IO.File.Exists(path))
                    {
                        ViewBag.Thongbao = "Hình ảnh đã tồn tại";
                    }
                    else
                    {
                        //Lưu hình ảnh vào đường dẫn
                        fileupload.SaveAs(path);
                    }
                    sach.Anhbia = filename;
                    //Lưu vào CSDL
                    data.SACHes.InsertOnSubmit(sach);
                    data.SubmitChanges();
                }
            }
                return RedirectToAction("Sach");
        }
        //Chỉnh sửa sản phẩm
        [HttpGet]
        public ActionResult Suasach( int id)
        {
            //lay ra doi tuong sach theo ma
            SACH sach = data.SACHes.SingleOrDefault(n => n.Masach == id);
            if (sach==null)
            {
                Response.StatusCode = 404;
                return null;
            }
            //Dua du lieu dropdownload
            //Lay ds tu table chu de, sap xep tang dan theo ten chu de, chon lay gia tri MaCd, hien thi TenCD

            ViewBag.MaCD = new SelectList(data.CHUDEs.ToList().OrderBy(n => n.TenChuDe), "MaCD", "TenChude", sach.MaCD);
            ViewBag.MaNXB = new SelectList(data.NHAXUATBANs.ToList().OrderBy(n => n.TenNXB), "MaNXB", "TenNXB", sach.MaNXB);
            return View(sach);
        }
        [HttpPost]
        [ValidateInput(false)]
        public ActionResult Suasach( SACH sach, HttpPostedFileBase fileUpload)
        {
            //dua du lieu vao dropdownload

            ViewBag.MaCD = new SelectList(data.CHUDEs.ToList().OrderBy(n => n.TenChuDe), "MaCD", "TenChude");
            ViewBag.MaNXB = new SelectList(data.NHAXUATBANs.ToList().OrderBy(n => n.TenNXB), "MaNXB", "TenNXB");
            //kiem tra duong dan file
            if (fileUpload == null)
            {
                ViewBag.Thongbao = "Vui lòng chọn ảnh bìa";
                return View();
            }    
            //them vào csdl
            else
            {
                if (ModelState.IsValid)
                {
                    //Luu ten file, luu ý bo sung thu vien using System.IO;
                    var fileName = Path.GetFileName(fileUpload.FileName);
                    //Luu duong dan file
                    var path = Path.Combine(Sever.MapPath("~/imgs"), fileName);
                    //Kiem tra hinh anh ton tai chua
                    if (System.IO.File.Exists(path))
                        ViewBag.Thongbao = "Hình ảnh đã tồn tại";
                    else
                    {
                        //Luu hinh anh vao duong dan
                        fileUpload.SaveAs(path);
                    }
                    sach.Anhbia = fileName;
                    //Luu vao CSDL
                    UpdateModel(sach);
                    data.SubmitChanges();
                }    
                return RedirectToAction("Sach");
            }    
        }
        //Hiển thị sản phẩm
        public ActionResult Chitietsach(int id)
        {
            //Lấy ra đối tượng sách theo mã
            SACH sach = data.SACHes.SingleOrDefault(n => n.Masach == id);
            ViewBag.Masach = sach.Masach;
            if (sach == null)
            {
                Response.StatusCode = 404;
                return null;
            }
            return View(sach);
        }
        //Xóa sản phẩm
        [HttpGet]
        public ActionResult Xoasach(int id)
        {
            //lay ra doi tuong sach can xoa theo ma
            SACH sach = data.SACHes.SingleOrDefault(n => n.Masach == id);
            ViewBag.Masach = sach.Masach;
            if ( sach == null)
            {
                Response.StatusCode = 404;
                return null;
            }
            return View(sach);
        }
        [HttpPost, ActionName("Xoasach")]
        public ActionResult Xacnhanxoa( int id)
        {
            //lay ra doi tuong sach can xoa theo ma
            SACH sach = data.SACHes.SingleOrDefault(n => n.Masach == id);
            ViewBag.Masach = sach.Masach;
            if (sach == null)
            {
                Response.StatusCode = 404;
                return null;
            }
            data.SACHes.DeleteOnSubmit(sach);
            data.SubmitChanges();
            return RedirectToAction("Sach");
        }
    }
}