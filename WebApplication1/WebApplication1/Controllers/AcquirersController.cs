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
    public class AcquirersController : Controller
    {
        private BienesRaicesDBEntities db = new BienesRaicesDBEntities();

        // GET: Acquirers
        public ActionResult Index()
        {
            var acquirers = db.Acquirers.Include(a => a.Inscription).Include(a => a.Person);
            return View(acquirers.ToList());
        }

        // GET: Acquirers/Details/5
        public ActionResult Details(string rut, int? atentionNumber)
        {
            if (rut == null && atentionNumber == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Acquirer acquirer = db.Acquirers.Find(atentionNumber, rut);
            if (acquirer == null)
            {
                return HttpNotFound();
            }
            return View(acquirer);
        }

        // GET: Acquirers/Create
        public ActionResult Create()
        {
            ViewBag.AtentionNumber = new SelectList(db.Inscriptions, "AtentionNumber", "CNE");
            ViewBag.Rut = new SelectList(db.People, "Rut", "Rut");
            return View();
        }

        // POST: Acquirers/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "AtentionNumber,Rut,Percentage")] Acquirer acquirer)
        {
            if (ModelState.IsValid)
            {
                db.Acquirers.Add(acquirer);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.AtentionNumber = new SelectList(db.Inscriptions, "AtentionNumber", "CNE", acquirer.AtentionNumber);
            ViewBag.Rut = new SelectList(db.People, "Rut", "Rut", acquirer.Rut);
            return View(acquirer);
        }

        // GET: Acquirers/Edit/5
        public ActionResult Edit(string rut, int? atentionNumber)
        {
            if (rut == null && atentionNumber == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Acquirer acquirer = db.Acquirers.Find(atentionNumber, rut);
            if (acquirer == null)
            {
                return HttpNotFound();
            }
            ViewBag.AtentionNumber = new SelectList(db.Inscriptions, "AtentionNumber", "CNE", acquirer.AtentionNumber);
            ViewBag.Rut = new SelectList(db.People, "Rut", "Rut", acquirer.Rut);
            return View(acquirer);
        }

        // POST: Acquirers/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "AtentionNumber,Rut,Percentage")] Acquirer acquirer)
        {
            if (ModelState.IsValid)
            {
                db.Entry(acquirer).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.AtentionNumber = new SelectList(db.Inscriptions, "AtentionNumber", "CNE", acquirer.AtentionNumber);
            ViewBag.Rut = new SelectList(db.People, "Rut", "Rut", acquirer.Rut);
            return View(acquirer);
        }

        // GET: Acquirers/Delete/5
        public ActionResult Delete(string rut, int? atentionNumber)
        {
            if (rut == null && atentionNumber == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Acquirer acquirer = db.Acquirers.Find(atentionNumber, rut);
            if (acquirer == null)
            {
                return HttpNotFound();
            }
            return View(acquirer);
        }

        // POST: Acquirers/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(string rut, int? atentionNumber)
        {
            Acquirer acquirer = db.Acquirers.Find(atentionNumber, rut);
            db.Acquirers.Remove(acquirer);
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
