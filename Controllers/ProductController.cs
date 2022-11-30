using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ToyStore.Models;
using PagedList;
using PagedList.Mvc;
using System.Net;

namespace ToyStore.Controllers
{
    public class ProductController : Controller
    {
        // GET: Product
        DBSportStoreEntities db = new DBSportStoreEntities();
        public ActionResult Index(string category,int? page ,double min=double.MinValue,double max=double.MaxValue)
        {
            int pageSize = 8;
            int pageNum = (page ?? 1);
          
            if (category == null)
            {
                var productList = db.Products.OrderByDescending(x => x.NamePro);
                return View(productList.ToPagedList(pageNum, pageSize));
            }
            else
            {
               var  productList = db.Products.OrderByDescending(x => x.Category).Where(x => x.Category == category);
                return View(productList.ToPagedList(pageNum, pageSize));
            }
          

        }
        public ActionResult Create()
        {
            List<Category> List = db.Categories.ToList();
            ViewBag.listCategory = new SelectList(List, "IDCate", "NameCate", "");
            Product pro = new Product();
            return View(pro);
        }
        public ActionResult SelectCate()
        {
            Category se_cate = new Category();
            se_cate.ListCate = db.Categories.ToList<Category>();
            return PartialView(se_cate);
        }
        [HttpPost]
        public ActionResult Create(Product pro)
        {
            List<Category> list = db.Categories.ToList();
            try
            {
                if (pro.UploadImage != null)
                {
                    string filename = Path.GetFileNameWithoutExtension(pro.UploadImage.FileName);
                    string extent = Path.GetExtension(pro.UploadImage.FileName);
                    filename = filename + extent;
                    pro.ImagePro = "~/Content/images/" + filename;
                    pro.UploadImage.SaveAs(Path.Combine(Server.MapPath("~/Content/images/"), filename));
                }
                ViewBag.listCategory = new SelectList(list, "IDCate", "NameCate", "");
                db.Products.Add(pro);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }
        public ActionResult SearchOption(double min=double.MinValue,double max = double.MaxValue)
        {
            var list = db.Products.Where(p => (double)p.Price >= min && (double)p.Price <= max).ToList();
            return View(list);
        }
    
        public ActionResult Search(string _name)
        {
            if (_name == null)
                return View(db.Products.ToList());
            else
                return View(db.Products.Where(s => s.NamePro.Contains(_name)).ToList());
        }

        public ActionResult Details(int id)
        {
            return View(db.Products.Where(s => s.ProductID == id).FirstOrDefault());
        }
        public ActionResult DetailsList(int id)
        {
            return View(db.Products.Where(s => s.ProductID == id).FirstOrDefault());
        }
      
        public ActionResult Edit(int id)
        {
            return View(db.Products.Where(s => s.ProductID == id).FirstOrDefault());
        }
        [HttpPost]
        public ActionResult Edit(int id, Product cate)
        {
            db.Entry(cate).State = System.Data.Entity.EntityState.Modified;
            db.SaveChanges();
            return RedirectToAction("List");
        }
        public ActionResult Delete(int id)
        {
            return View(db.Products.Where(s => s.ProductID == id).FirstOrDefault());
        }
        [HttpPost]
        public ActionResult Delete(int id, Product cate)
        {
            try
            {
                cate = db.Products.Where(s => s.ProductID == id).FirstOrDefault();
                db.Products.Remove(cate);
                db.SaveChanges();
                return RedirectToAction("List");
            }
            catch
            {
                return Content("This data is using in other table, Error Delete!");
            }
        }
        public ActionResult List(string _name)
        {

            if (_name == null)
                return View(db.Products.ToList());
            else
                return View(db.Products.Where(s => s.NamePro.Contains(_name)).ToList());
            //return View(db.Categories.ToList());
        }
        public ActionResult Home()
        {
            return View();
        }

        public ActionResult TopNew()
        {
            List<Product> proList = db.Products.ToList();
            var query = from od in proList
                        join p in proList on od.ProductID equals p.ProductID into tbl
                        group od by new
                        {
                            idPro = od.ProductID,
                            namePro = od.NamePro,
                            imagePro = od.ImagePro,
                            price = od.Price
                        } into gr
                        orderby gr.Max(s => s.ProductID) descending
                        select new ViewModel
                        {
                            IDPro = gr.Key.idPro,
                            NamePro = gr.Key.namePro,
                            ImgPro = gr.Key.imagePro,
                            pricePro = (decimal)gr.Key.price,
                            Top5_New = gr.Max(s => s.ProductID)
                        };
            return PartialView(query.Take(4).ToList());
        }
    }

}