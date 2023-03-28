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
    public class InscriptionsController : Controller
    {
        private BienesRaicesDBEntities db = new BienesRaicesDBEntities();

        // GET: Inscriptions
        public ActionResult Index()
        {
            return View(db.Inscriptions.ToList());
        }

        // GET: Inscriptions/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Inscription inscription = db.Inscriptions.Find(id);
            if (inscription == null)
            {
                return HttpNotFound();
            }
            return View(inscription);
        }

        // GET: Inscriptions/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Inscriptions/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "AtentionNumber,CNE,Comunne,Block,Site,Page,InscriptionNumber,InscriptionDate")] Inscription inscription)
        {
            if (ModelState.IsValid)
            {
                db.Inscriptions.Add(inscription);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(inscription);
        }

        // GET: Inscriptions/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Inscription inscription = db.Inscriptions.Find(id);
            if (inscription == null)
            {
                return HttpNotFound();
            }
            return View(inscription);
        }

        // POST: Inscriptions/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "AtentionNumber,CNE,Comunne,Block,Site,Page,InscriptionNumber,InscriptionDate")] Inscription inscription)
        {
            if (ModelState.IsValid)
            {
                db.Entry(inscription).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(inscription);
        }

        // GET: Inscriptions/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Inscription inscription = db.Inscriptions.Find(id);
            if (inscription == null)
            {
                return HttpNotFound();
            }
            return View(inscription);
        }

        // POST: Inscriptions/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Inscription inscription = db.Inscriptions.Find(id);
            db.Inscriptions.Remove(inscription);
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
