using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ToyStore.Models;

namespace ToyStore.Controllers
{
    public class ShoppingCartController : Controller
    {
        // GET: ShoppingCart
        DBSportStoreEntities db = new DBSportStoreEntities();
       
        public ActionResult ShowCart()
        {
            if (Session["Cart"] == null)
                //  return RedirectToAction("ShowCart", "ShoppingCart");
                return View("EmptyCart");
            Cart _cart = Session["Cart"] as Cart;
            return View(_cart);
        }
        public Cart GetCart()
        {
            Cart cart = Session["Cart"] as Cart;
            if (cart == null || Session["Cart"] == null)
            {
                cart = new Cart();
                Session["Cart"] = cart;
            }
            return cart;
        }
        public ActionResult AddToCart(int id)
        {
            var _pro = db.Products.SingleOrDefault(s => s.ProductID == id);
            if (_pro != null)
            {
                GetCart().Add_Product_Cart(_pro);
            }
            return RedirectToAction("ShowCart", "ShoppingCart");
        }
        public ActionResult Update_Cart_Quantity(FormCollection form)
        {
            Cart cart = Session["Cart"] as Cart;
            int id_pro = int.Parse(form["idPro"]);
            int _quantity = int.Parse(form["cartQuantity"]);
            cart.Update_quantity(id_pro, _quantity);
            return RedirectToAction("ShowCart", "ShoppingCart");
        }
        public ActionResult RemoveCart(int id)
        {
            Cart cart = Session["Cart"] as Cart;
            cart.Remove_CartItem(id);
         
            return RedirectToAction("ShowCart", "ShoppingCart");
        }
        public PartialViewResult BagCart()
        {
            int total_quantity_item = 0;
            Cart cart = Session["Cart"] as Cart;
            if (cart != null)
                total_quantity_item = cart.Total_quantity();
            ViewBag.QuantityCart = total_quantity_item;
            return PartialView("BagCart");
        }
        public ActionResult CheckOut(FormCollection form)
        {
            try
            {
                Cart cart = Session["Cart"] as Cart;
                OrderPro _oder = new OrderPro(); //Bảng hóa đơn sản phẩm
                _oder.DateOrder = DateTime.Now;
                _oder.AddressDeliverry = form["AddressDelivery"];
                _oder.IDCus = int.Parse(form["CodeCustomer"]);
                db.OrderProes.Add(_oder);
                foreach(var item in cart.Items)
                {
                    OrderDetail _order_detail = new OrderDetail(); //lưu dòng sản phẩm vào bảng chi tiết hóa đơn
                    _order_detail.IDOrder = _oder.ID;
                    _order_detail.IDProduct = item._product.ProductID;
                    _order_detail.UnitPrice = (double)item._product.Price;
                    _order_detail.Quantity = item._quantity;
                    db.OrderDetails.Add(_order_detail);
                    //xử lý cập nhật lại số lượng tồn trong bảng product
                    foreach(var p in db.Products.Where(s => s.ProductID == _order_detail.IDProduct))
                    {
                        var update_quan_pro = p.Quantity - item._quantity;// số lượng tồn mới =số lượng tồn -số lượng đã mua
                        p.Quantity = update_quan_pro;//Thực hiện cập nhật lại số lượng tồn cho cột Quantity của bảng Product
                    }
                }
                db.SaveChanges();
                cart.CLeanCart();
                return RedirectToAction("CheckOut_Sucess", "ShoppingCart");
            }
            catch
            {
                return Content("Error checkout. Please check information of Customer ... Thanks.");
            }
            
        }
        public ActionResult CheckOut_Sucess()
        {
            return View();
        }
    }
}