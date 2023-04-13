using Newtonsoft.Json.Linq;
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
        public ActionResult Create([Bind(Include = "AtentionNumber,CNE,Comunne,Block,Site,Page,InscriptionNumber,InscriptionDate")] Inscription inscription, string users_info)
        {
            if (ModelState.IsValid)
            {
                db.Inscriptions.Add(inscription);
                db.SaveChanges();
                var AllUsers = JObject.Parse(users_info);
                if (AllUsers.ContainsKey("alienators_users"))
                {
                    var AlienatorsUsers = AllUsers["alienators_users"];

                    foreach (var alienator_info in AlienatorsUsers)
                    {
                        string rutPerson = alienator_info[0].ToString();
                        double percentagePerson = Convert.ToDouble(alienator_info[1]);
                        CreatePeople("alienators", rutPerson, percentagePerson, inscription);
                    }
                }
                if (AllUsers.ContainsKey("acquirers_users"))
                {
                    var AcquirersUsers = AllUsers["acquirers_users"];

                    foreach (var acquirer_info in AcquirersUsers)
                    {
                        string rutPerson = acquirer_info[0].ToString();
                        double percentagePerson = Convert.ToDouble(acquirer_info[1]);
                        CreatePeople("acquirers", rutPerson, percentagePerson, inscription);

                    }
                }
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
    public void CreatePeople(string typePeople, string rutPerson, double persentagePerson, Inscription inscription)
        {
            Person person;
            Person instance = db.People.Find(rutPerson);
            if (instance == null)
            {
                person = new Person();
                person.Rut = rutPerson;
                db.People.Add(person);
                db.SaveChanges();
            }
            else person = instance;
            
            if (typePeople == "alienators") 
            {
                Alienator alienator = new Alienator();
                alienator.AtentionNumber = inscription.AtentionNumber;
                alienator.Rut = person.Rut;
                alienator.Percentage = persentagePerson;
                db.Alienators.Add(alienator);
                db.SaveChanges();
            }
            else if (typePeople == "acquirers")
            {
                Acquirer acquirer = new Acquirer();
                acquirer.AtentionNumber = inscription.AtentionNumber;
                acquirer.Rut = person.Rut;
                acquirer.Percentage = persentagePerson;
                db.Acquirers.Add(acquirer);
                db.SaveChanges();
                crateMultyproperty(rutPerson, persentagePerson, inscription);
            }
        }
        public void crateMultyproperty(string rutPerson, double persentagePerson, Inscription inscription)
        {
            Multyproperty multyproperty = new Multyproperty();
            multyproperty.Comunne = inscription.Comunne;
            multyproperty.Block = inscription.Block;
            multyproperty.Site = inscription.Site;
            multyproperty.AtentionNumber = inscription.AtentionNumber;
            multyproperty.Page = inscription.Page;
            multyproperty.InscriptionNumber = inscription.InscriptionNumber;
            multyproperty.InscriptionDate = inscription.InscriptionDate;
            multyproperty.InscriptionYear = inscription.InscriptionDate.Year;
            multyproperty.StartCurrencyYear = inscription.InscriptionDate.Year;
            multyproperty.Rut = rutPerson;
            multyproperty.Percentage = persentagePerson;
            db.Multyproperties.Add(multyproperty);
            db.SaveChanges();   
        }
    }
}
