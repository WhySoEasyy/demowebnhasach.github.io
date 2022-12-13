using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WEBNHASACH.Models;
using WEBNHASACH.Controllers;

namespace WEBNHASACH.Controllers
{
    public class GioHangController : Controller
    {
        DataClasses1DataContext db = new DataClasses1DataContext();
        // GET: GioHang
        public ActionResult GioHang()
        {
            if(Session["idUser"] != null)
            {
                var ListAO = db.GIOHANGs.Where(s => s.MAKH == Session["idUser"].ToString()).ToList();
                return View(ListAO);
            }
            else
            {
                var ListAO = db.GIOHANGs.Where(s => s.MAKH =="").ToList();
                return View(ListAO);
            }
        }
        [HttpPost]
        public ContentResult GioHang(FormCollection formCollection)
        {
            string xacnhan = formCollection["XacNhan"];
            string TTthanhtoan = formCollection["TTthanhtoan"];

            if (xacnhan == "btnHuySP")
            {
                string maDp3 = formCollection["MaXnSP3"];
                using (DataClasses1DataContext db = new DataClasses1DataContext())
                {
                    GIOHANG kq = db.GIOHANGs.Where(r => r.TENSP == maDp3).FirstOrDefault();
                    db.GIOHANGs.DeleteOnSubmit(kq);
                    db.SubmitChanges();
                }
                return Content("Đã xóa khỏi giỏ hàng !!!");
            }
            else if(xacnhan == "btnThanhToan")
            {
                using (DataClasses1DataContext db = new DataClasses1DataContext())
                {
                    var SoHD = db.HOADONs.Count() + 1;
                    var newMaHD = "HD" + SoHD;               
                    HOADON HD = new HOADON
                    {
                        MAHD = newMaHD,
                        MAKH = Session["idUser"].ToString(),
                        MANV = "NV001",
                        MAHTB = "MHTB002",
                        NGAYLAP = DateTime.Now,
                        TONGTIEN = Convert.ToDouble(TTthanhtoan.ToString()),
                        THANHTIEN = Convert.ToDouble(TTthanhtoan.ToString()),
                    };
                    db.HOADONs.InsertOnSubmit(HD);
                    db.SubmitChanges();
                    var itemGH = db.GIOHANGs.Where(s => s.MAKH == Session["idUser"].ToString()).ToList();
                    foreach(var item  in itemGH)
                    {
                        var SoCTHD = db.CTHDs.Count() + 1;
                        var newMaCTHD = "CTHD" + SoCTHD;
                        CTHD CT = new CTHD
                        {
                            MACTHD = newMaCTHD,
                            MAHD = newMaHD,
                            MASP = item.MASP,
                            SOLUONG = item.SOLUONG,
                            GIA = item.GIA,
                            TONGTIEN = item.TONGTIEN,
                        };
                        db.CTHDs.InsertOnSubmit(CT);
                        db.SubmitChanges();
                    }    
                    var kq = db.GIOHANGs.Where(r => r.MAKH == Session["idUser"].ToString()).ToList();
                    foreach(var item in kq)
                    {
                        db.GIOHANGs.DeleteOnSubmit(item);
                    }     
                    db.SubmitChanges();
                }
                return Content("Đã thanh toán !!!");
            }
            else
            {
                return Content("Lỗi vui lòng kiểm tra lại !!!");
            }
        }
    }
}