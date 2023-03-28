using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using WebApplication1.Models;

namespace WebApplication1.Controllers
{
    public class MultypropertiesController : Controller
    {
        private BienesRaicesDBEntities db = new BienesRaicesDBEntities();

        // GET: Multyproperties
        public ActionResult Index()
        {
            return View(db.Multyproperties.ToList());
        }

        // GET: Multyproperties/Details/5
        public ActionResult Details(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Multyproperty multyproperty = db.Multyproperties.Find(id);
            if (multyproperty == null)
            {
                return HttpNotFound();
            }
            return View(multyproperty);
        }

        // GET: Multyproperties/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Multyproperties/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Comunne,Block,Site,Rut,Percentage,Page,InscriptionNumber,InscriptionYear,InscriptionDate,StartCurrencyYear,EndCurrencyYear")] Multyproperty multyproperty)
        {
            if (ModelState.IsValid)
            {
                db.Multyproperties.Add(multyproperty);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(multyproperty);
        }

        // GET: Multyproperties/Edit/5
        public ActionResult Edit(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Multyproperty multyproperty = db.Multyproperties.Find(id);
            if (multyproperty == null)
            {
                return HttpNotFound();
            }
            return View(multyproperty);
        }

        // POST: Multyproperties/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Comunne,Block,Site,Rut,Percentage,Page,InscriptionNumber,InscriptionYear,InscriptionDate,StartCurrencyYear,EndCurrencyYear")] Multyproperty multyproperty)
        {
            if (ModelState.IsValid)
            {
                db.Entry(multyproperty).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(multyproperty);
        }

        // GET: Multyproperties/Delete/5
        public ActionResult Delete(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Multyproperty multyproperty = db.Multyproperties.Find(id);
            if (multyproperty == null)
            {
                return HttpNotFound();
            }
            return View(multyproperty);
        }

        // POST: Multyproperties/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(string id)
        {
            Multyproperty multyproperty = db.Multyproperties.Find(id);
            db.Multyproperties.Remove(multyproperty);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
