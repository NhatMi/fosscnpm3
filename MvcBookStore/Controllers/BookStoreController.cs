using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.UI;
using MvcBookStore.Models;

using PagedList;
using PagedList.Mvc;
namespace MvcBookStore.Controllers
{
    public class BookStoreController : Controller
    {
        //Tạo 1 đối tượng chứa toàn bộ CSDL từ dbQLBansach
        QLBANSACHEntities data = new QLBANSACHEntities();

        private List<SACH> Laysachmoi(int count)
        {
            //Sắp xếp giảm dần theo Ngaycapnhat, lấy count đống dấu
            return data.SACHes.OrderByDescending(a => a.Ngaycapnhat).Take(count).ToList();
        }
        
        public ActionResult Index()
        {
            //    // Tao bien quy dinh so san pham trang moi trang
            //    int pageSize = 5;
            //    // tao bien so trang
            //    int pageNum = (page ?? 1);


            //    //Lấy 5 quyển sách mới nhất
            //    var sachmoi = Laysachmoi(15);
            //    return View(sachmoi.ToPagedList(pageNum, pageSize));
            return View();
        }

        public ActionResult SanPhamPartial(int? page)
        {
            int pageSize = 5;
            int pageNum = (page ?? 1);


            //Lấy 5 quyển sách mới nhất

            return View(data.SACHes.Where(n => n.Giaban == 1).ToPagedList(pageNum, pageSize));

        }
        public PartialViewResult chudePartial()
        {
            var lstsach = data.CHUDEs.ToList();
            return PartialView(lstsach);
        }
        public PartialViewResult  NXBPartial()
        {
            var lstnxb = data.NHAXUATBANs.ToList();
            return PartialView(lstnxb);
        }
        public ActionResult SPTheochude( int id)
        {
            var sach = from s in data.SACHes where s.MaCD == id select s;
            return View(sach);
        }
        public ActionResult SPTheoNXB(int id)
        {
            var sach = from s in data.SACHes where s.MaNXB == id select s;
            return View(sach);
        }
        public ActionResult Details(int id)
        {
            var sach = from s in data.SACHes
                       where s.Masach == id
                       select s;
            return View(sach.Single());
        }
    }
}