using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ToyStore.Models;

namespace ToyStore.Controllers
{
    public class CustomersController : Controller
    {
        DBSportStoreEntities db = new DBSportStoreEntities();
        public ActionResult Index(string _name)
        {

            if (_name == null)
                return View(db.Customers.ToList());
            else
                return View(db.Customers.Where(s => s.NameCus.Contains(_name)).ToList());
            
        }

        public ActionResult Create()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Create(Customer cate)
        {
            try
            {
                db.Customers.Add(cate);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            catch
            {
                return Content("Error Craete New");
            }
        }
        public ActionResult Details(string name)
        {
            return View(db.Customers.Where(s => s.NameCus == name).FirstOrDefault());
        }
        public ActionResult Edit(string name)
        {
            return View(db.Customers.Where(s => s.NameCus ==name ).FirstOrDefault());
        }
        [HttpPost]
        public ActionResult Edit(string name, Customer cate)
        {
            db.Entry(cate).State = System.Data.Entity.EntityState.Modified;
            db.SaveChanges();
            return RedirectToAction("Index");
        }
        public ActionResult Delete(string name)
        {
            return View(db.Customers.Where(s => s.NameCus == name).FirstOrDefault());
        }
        [HttpPost]
        public ActionResult Delete(string name, Customer cate)
        {
            try
            {
                cate = db.Customers.Where(s => s.NameCus == name).FirstOrDefault();
                db.Customers.Remove(cate);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            catch
            {
                return Content("This data is using in other table, Error Delete!");
            }
        }
    }
}