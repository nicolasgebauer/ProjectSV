using Microsoft.Ajax.Utilities;
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

                List<Tuple<List<Tuple<string, double>>, double, double>> allUsers = GetUsersJsonToLists(users_info);
                Tuple<List<Tuple<string, double>>, double, double> allAlienatorsInfo = allUsers[0];
                Tuple<List<Tuple<string, double>>, double, double> allAcquirersInfo = allUsers[1];
                //double percentageForAlienator = allAlienatorsInfo.Item2;
                double percentageForAcquirer = allAcquirersInfo.Item2;
                var AllUsers = JObject.Parse(users_info);

                var AcquirersUsers = AllUsers["acquirers_users"];

                var AlienatorsUsers = AllUsers["alienators_users"];
                ;

                if (inscription.CNE == "Regularización de Patrimonio")
                {
                    EquityRegulation(allAcquirersInfo, inscription);
                } else if (inscription.CNE == "Compraventa")
                {
                    BuyingAndSelling(allAcquirersInfo,allAlienatorsInfo,inscription);
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

        public static int? FindNextYear(int year, List<Multyproperty> multiproperties)
        {
            int? nextYear = multiproperties[0].StartCurrencyYear;
            if (nextYear < year) return null;
            foreach (Multyproperty mp in multiproperties)
            {
                if (mp.StartCurrencyYear > year && (mp.StartCurrencyYear - year) < (nextYear - year))
                {
                    nextYear = mp.StartCurrencyYear;
                }
            }
            return nextYear;
        }

        //Función para encontrar el anterior año presente en los multipropietarios según
        // una año y rol
        public static int? FindPastYear(int year, List<Multyproperty> multiproperties)
        {
            int last = multiproperties.Count - 1;
            int? pastYear = multiproperties[last].StartCurrencyYear;
            if (pastYear > year) return null;
            foreach (Multyproperty mp in multiproperties)
            {
                if (mp.StartCurrencyYear < year && (year - mp.StartCurrencyYear) < (year - pastYear))
                {
                    pastYear = mp.StartCurrencyYear;
                }
            }
            return pastYear;
        }

        public void BuyingAndSelling1To1Case(string rutAlienator, string rutAcquirer, double percentageAlienator, double percentageAcquirer, Inscription inscription)
        {
            
            Person alienatorPerson = CreateOrSetPerson(rutAlienator);
            Alienator alienatorInstance = CreateAlienator(alienatorPerson, percentageAlienator, inscription);
            int year = inscription.InscriptionDate.Year;
            List<Multyproperty> multypropertiesAlienator = SearchMulypropertiesinDataBase(inscription, rutAlienator,year);

            List<Multyproperty> multypropertiesAquirer = SearchMulypropertiesinDataBase(inscription, rutAcquirer, year);

            Multyproperty multyAlienator = multypropertiesAlienator[0];
            double persentToTransfer = (double)(multyAlienator.Percentage * percentageAlienator) / 100;
            multyAlienator.Percentage = multyAlienator.Percentage - persentToTransfer;
            Person acquirerPerson = CreateOrSetPerson(rutAcquirer);
            Acquirer acquirerInstance = CreateAcquirer(acquirerPerson, persentToTransfer, inscription);
            db.Entry(multyAlienator);
            db.SaveChanges();
            CreateMultypropertyForBuyingAndSelling(acquirerInstance, inscription);

        }

        private List<Tuple<List<Tuple<string, double>>, double, double>> GetUsersJsonToLists(string people_info)
        {
            List<Tuple<List<Tuple<string, double>>, double, double>> allPeople = new List<Tuple<List<Tuple<string, double>>, double, double>>();
            List<Tuple<string, double>> alienators = new List<Tuple<string, double>>();
            List<Tuple<string, double>> acquirers = new List<Tuple<string, double>>();
            var jsonUsers = JObject.Parse(people_info);

            var AlienatorsUsers = jsonUsers["alienators_users"];
            double sumPercentageAlienators = AlienatorsUsers.Sum(alienator => Convert.ToDouble(alienator[1]));
            int countAlienators = AlienatorsUsers.Count(alienator => Convert.ToDouble(alienator[1]) == 0);
            double percentageForAlienator = (100 - sumPercentageAlienators) / countAlienators;

            var AcquirersUsers = jsonUsers["acquirers_users"];
            double sumPercentageAcquirers = AcquirersUsers.Sum(aquirer => Convert.ToDouble(aquirer[1]));
            int countAcquirers = AcquirersUsers.Count(aquirer => Convert.ToDouble(aquirer[1]) == 0);
            double percentageForAcquirer = (100 - sumPercentageAcquirers) / countAcquirers;

            if (jsonUsers.ContainsKey("alienators_users"))
            {
                foreach (var alienator_info in AlienatorsUsers)
                {
                    string rutAlienator = alienator_info[0].ToString();
                    double percentageAlienator = Convert.ToDouble(alienator_info[1]);
                    Tuple<string, double> alienatorTuple = new Tuple<string, double>(rutAlienator, percentageAlienator);
                    alienators.Add(alienatorTuple);
                }
                Tuple<List<Tuple<string, double>>, double, double> allAlienatorsTuple = new Tuple<List<Tuple<string, double>>, double, double>(alienators, percentageForAlienator, sumPercentageAlienators);
                allPeople.Add(allAlienatorsTuple);
            }

            if (jsonUsers.ContainsKey("acquirers_users"))
            {
                foreach (var acquirer_info in AcquirersUsers)
                {
                    string rutAcquirer = acquirer_info[0].ToString();
                    double percentageAcquirer = Convert.ToDouble(acquirer_info[1]);
                    Tuple<string, double> acquirerTuple = new Tuple<string, double>(rutAcquirer, percentageAcquirer);
                    acquirers.Add(acquirerTuple);
                }
                Tuple<List<Tuple<string, double>>, double, double> allAcquirersTuple = new Tuple<List<Tuple<string, double>>, double, double>(acquirers, percentageForAcquirer, sumPercentageAcquirers);
                allPeople.Add(allAcquirersTuple);
            }
            return allPeople;
        }

        private void EquityRegulation(Tuple<List<Tuple<string, double>>, double, double> acquirersInfo, Inscription inscription)
        {
            List<Tuple<string, double>> acquirers = acquirersInfo.Item1;
            double percentageForAcquirer = acquirersInfo.Item2;
            double sumPercentageAcquirers = acquirersInfo.Item3;
            foreach (var infoPerAcquirer in acquirers)
            {
                string rut = infoPerAcquirer.Item1;
                double percentage = infoPerAcquirer.Item2;
                Acquirer acquirer;
                Person person = CreateOrSetPerson(rut);
                if (percentage == 0) acquirer = CreateAcquirer(person, percentageForAcquirer, inscription);
                else acquirer = CreateAcquirer(person, percentage, inscription);
                CreateMultypropertyForEquityRegulation(acquirer, inscription);
            }
        }

        private void CreateMultyproperty(string rut,double? percentage,int startYear, int? endYear, Inscription inscription)
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
            multyproperty.StartCurrencyYear = startYear;
            if (endYear != null)multyproperty.EndCurrencyYear = endYear;
            multyproperty.Rut = rut;
            multyproperty.Percentage = percentage;
            db.Multyproperties.Add(multyproperty);
            db.SaveChanges();
        }

        private Person CreateOrSetPerson(string rut)
        {
            Person person;
            Person instancePerson = db.People.Find(rut);
            if (instancePerson == null)
            {
                person = new Person();
                person.Rut = rut;
                db.People.Add(person);
                db.SaveChanges();
            }
            else person = instancePerson;
            return person;
        }

        private Acquirer CreateAcquirer(Person person, double percentage, Inscription inscription)
        {
            Acquirer acquirer = new Acquirer();
            acquirer.AtentionNumber = inscription.AtentionNumber;
            acquirer.Rut = person.Rut;
            acquirer.Percentage = percentage;
            db.Acquirers.Add(acquirer);
            db.SaveChanges();
            return acquirer;
        }

        private Alienator CreateAlienator(Person person, double percentage, Inscription inscription)
        {
            Alienator alienator = new Alienator();
            alienator.AtentionNumber = inscription.AtentionNumber;
            alienator.Rut = person.Rut;
            alienator.Percentage = percentage;
            db.Alienators.Add(alienator);
            db.SaveChanges();
            return alienator;
        }

        private void ChangeFinishYearsInMultyproperties (int year, Inscription inscription)
        {
            List<Multyproperty> multyproperties = SearchMulypropertiesinDataBase(inscription);
            int? pastYear = null;
            if (multyproperties.Count > 0)
            {
                pastYear = FindPastYear(year, multyproperties);
                if (pastYear != null)
                {
                    List<Multyproperty> multypropertiesToChange = multyproperties.Where(
                        mp => mp.StartCurrencyYear == pastYear
                        ).ToList();
                    foreach (Multyproperty pastMP in multypropertiesToChange)
                    {
                        pastMP.EndCurrencyYear = year - 1;
                        db.Entry(pastMP);
                        db.SaveChanges();
                    }
                }
            }
        }

        private void CreateMultypropertyForEquityRegulation(Acquirer acquirer, Inscription inscription)
        {
            List<Multyproperty> multyproperties = SearchMulypropertiesinDataBase(inscription);
            bool ifCreateMultyproperty = true;
            int? endYear = null;
            int yearOfStart;
            if (inscription.InscriptionDate.Year < 2019) yearOfStart = 2019;
            else yearOfStart = inscription.InscriptionDate.Year;

            if (multyproperties.Count > 0)
            {
                List<Multyproperty> multypropertiesSameYear = multyproperties.Where(
                    mp => mp.StartCurrencyYear == yearOfStart
                    ).ToList();
                if (multypropertiesSameYear.Count > 0)
                {
                    foreach (Multyproperty sameYearMP in multypropertiesSameYear)
                    {
                        if (Double.Parse(sameYearMP.InscriptionNumber) > Double.Parse(inscription.InscriptionNumber))
                        {
                            ifCreateMultyproperty = false;
                            break;
                        }
                        db.Multyproperties.Remove(sameYearMP);
                        db.SaveChanges();
                    }
                }

                multyproperties = SearchMulypropertiesinDataBase(inscription);
                int? nextYear = null;
                if (multyproperties.Count > 0 && ifCreateMultyproperty == true )
                {
                    ChangeFinishYearsInMultyproperties(yearOfStart, inscription);
                    nextYear = FindNextYear(yearOfStart, multyproperties);
                }
                    
                if (nextYear != null)
                {
                    Multyproperty nextMP = multyproperties.Find(mp => mp.StartCurrencyYear == nextYear);
                    endYear= nextMP.StartCurrencyYear-1;
                }
            }
            if (ifCreateMultyproperty == true)
            {
                CreateMultyproperty(acquirer.Rut, acquirer.Percentage, yearOfStart, endYear, inscription);
            }
        }

        private void CreateMultypropertyForBuyingAndSelling(Acquirer acquirer,Inscription inscription)
        {
            List<Multyproperty> multyproperties = SearchMulypropertiesinDataBase(inscription);
            bool ifCreateMultyproperty = true;
            int? endYear = null;
            int yearOfStart;
            if (inscription.InscriptionDate.Year < 2019) yearOfStart = 2019;
            else yearOfStart = inscription.InscriptionDate.Year;
            if (multyproperties.Count > 0)
            {
                List<Multyproperty> multypropertiesSameYear = multyproperties.Where(
                    mp => mp.StartCurrencyYear == yearOfStart
                    ).ToList();
                if (multypropertiesSameYear.Count > 0)
                {
                    foreach (Multyproperty sameYearMP in multypropertiesSameYear)
                    {
                        if (Double.Parse(sameYearMP.InscriptionNumber) > Double.Parse(inscription.InscriptionNumber))
                        {
                            ifCreateMultyproperty = false;
                            break;
                        }
                        endYear = sameYearMP.EndCurrencyYear;
                    }
                }
            }
            if (ifCreateMultyproperty == true)
            {
                CreateMultyproperty(acquirer.Rut, acquirer.Percentage, yearOfStart, endYear, inscription);
            }
        }

        private List<Multyproperty> SearchMulypropertiesinDataBase(Inscription inscription, string rut = null, int? year  = null)
        {
            List<Multyproperty> multyproperties;
            if (rut == null && year == null)
            {
                multyproperties = db.Multyproperties.Where(
               mp => mp.Comunne == inscription.Comunne
               && mp.Block == inscription.Block
               && mp.Site == inscription.Site
               && mp.AtentionNumber != inscription.AtentionNumber
                ).OrderByDescending(mp => mp.InscriptionDate).ToList();
            }
            else
            {
                multyproperties = db.Multyproperties.Where(
                mp => mp.Comunne == inscription.Comunne
                && mp.Block == inscription.Block
                && mp.Site == inscription.Site
                && mp.AtentionNumber != inscription.AtentionNumber
                && mp.Rut == rut
                && mp.InscriptionYear == year
                ).OrderByDescending(mp => mp.InscriptionDate).ToList();
            }
            return multyproperties;
        }

        private void BuyingAndSelling(Tuple<List<Tuple<string, double>>, double, double> acquirersInfo, Tuple<List<Tuple<string, double>>, double, double> alienatorsInfo, Inscription inscription)
        {
            List<Tuple<string, double>> acquirers = acquirersInfo.Item1;
            double percentageForAcquirer = acquirersInfo.Item2;
            double sumPercentageAcquirers = acquirersInfo.Item3;

            List<Tuple<string, double>> alienators = alienatorsInfo.Item1;
            double percentageForAlienators = alienatorsInfo.Item2;
            double sumPercentageAlienators = alienatorsInfo.Item3;

            if (sumPercentageAcquirers == 100)
            {

            }
            else if (acquirers.Count == 1 && alienators.Count == 1)
            {
                Tuple<string, double> acquirer = acquirers[0];
                string rutAcquirer = acquirer.Item1;
                double percentageAcquirer = acquirer.Item2;
                
                Tuple<string, double> alienator = alienators[0];
                string rutAlienator = alienator.Item1;
                double percentageAlienator = alienator.Item2;
             
                BuyingAndSelling1To1Case(rutAlienator, rutAcquirer, percentageAlienator, percentageAcquirer, inscription);
            }
            else
            {

            }
        }
    }

}
