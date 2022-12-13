using WEBNHASACH.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace WEBNHASACH.Controllers
{
    public class HomeController : Controller
    {
        DataClasses1DataContext objModel = new DataClasses1DataContext();

        // GET: Home
        [HttpGet]
        public PartialViewResult TrangChu()
        {
            var lstSachMoi = objModel.SANPHAMs.Take(52).ToList();
            if (Session["ButtonDN"] == null)
            {
                Session["ButtonDN"] = "Đăng nhập";
                Session["FullName"] = "";
                Session["idUser"] = null;
            }
            return PartialView(lstSachMoi);
        }
        [HttpPost]
        public ContentResult XuLyNutDN(FormCollection formCollection)
        {
            Session["ButtonDN"] = "Đăng nhập";
            Session["FullName"] = "";
            Session["idUser"] = null;
            return Content("Đăng xuất thành công");
        }
        public ActionResult CTSANPHAM(string MaSach)
        {
            var ctsanpham = objModel.SANPHAMs.Where(n => n.MASP == MaSach).FirstOrDefault();
            return View(ctsanpham);
        }
        [HttpPost]
        public ContentResult TrangChu(FormCollection formCollection)
        {
            if(formCollection["XacNhan"] == "DN")
            {
                if (Session["idUser"] != null && formCollection["MaXnGH"] != "undefined")
                {
                    string MaSPGH = formCollection["MaXnGH"];
                    SANPHAM kq = objModel.SANPHAMs.Where(r => r.MASP == MaSPGH).FirstOrDefault();
                    GIOHANG ktSL = objModel.GIOHANGs.Where(r => r.MASP == MaSPGH).FirstOrDefault();
                    if (ktSL != null)
                    {
                        GIOHANG ktSL1 = objModel.GIOHANGs.Where(r => r.MASP == MaSPGH).FirstOrDefault();
                        ktSL1.SOLUONG = ktSL1.SOLUONG + 1;
                        ktSL1.TONGTIEN = kq.GIA * (ktSL1.SOLUONG + 1);
                        objModel.SubmitChanges();
                        return Content("Thêm sản phẩm thành công !!!");

                    }
                    else
                    {
                        GIOHANG tt = new GIOHANG
                        {
                            ID = objModel.GIOHANGs.Count() + 1,
                            ANH = kq.Anh,
                            TENSP = kq.TENSP,
                            MAKH = Session["idUser"].ToString(),
                            SOLUONG = 1,
                            GIA = kq.GIA,
                            TONGTIEN = kq.GIA,
                            MASP = MaSPGH,
                        };
                        objModel.GIOHANGs.InsertOnSubmit(tt);
                        objModel.SubmitChanges();
                        return Content("Thêm sản phẩm thành công !!!");
                    }
                }
                else
                {
                    if(formCollection["MaXnGH"] != "undefined")
                    {
                        return Content("Vui lòng đăng nhập !!!");
                    }    
                    else
                    {
                        return Content("Đang xử lý vui lòng đợi !!!");
                    }    
                }
            }    
            else
            {
                return Content("Lỗi xác nhận !!!");
            }    
            
        }
        // GET: Login
        public ActionResult DangNhap()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult DangNhap(string taikhoan, string matkhau)
        {
                var data = objModel.KHACHHANGs.Where(s => s.TAIKHOAN.Equals(taikhoan) && s.MATKHAU.Equals(matkhau)).ToList();
                if (data.Count() > 0)
                {
                    //add session
                    Session["FullName"] = data.FirstOrDefault().HOTENKH;
                    Session["ButtonDN"] = "Đăng xuất";
                    Session["Email"] = data.FirstOrDefault().EMAIL;
                    Session["idUser"] = data.FirstOrDefault().MAKH;
                return RedirectToAction("TrangChu");
                }
                else
                {
                    ViewBag.error = "Login failed";
                    return RedirectToAction("DangNhap");
                }

        }

        public ActionResult DangKi()
        {
            return View();
        }
    }
}