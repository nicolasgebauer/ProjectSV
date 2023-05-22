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
        public ActionResult Create([Bind(Include = "AtentionNumber,CNE,Comunne,Block,Site,Page,InscriptionNumber,InscriptionDate")] Inscription inscription, string usersInfo)
        {
            if (ModelState.IsValid)
            {
                db.Inscriptions.Add(inscription);
                db.SaveChanges();

                List<Tuple<List<Tuple<string, double>>, double, double>> allUsersInfo = GetUsersJsonToLists(usersInfo);
                Tuple<List<Tuple<string, double>>, double, double> allAlienatorsInfo = allUsersInfo[0];
                Tuple<List<Tuple<string, double>>, double, double> allAcquirersInfo = allUsersInfo[1];
                //double percentageForAlienator = allAlienatorsInfo.Item2;
                double percentageForAcquirer = allAcquirersInfo.Item2;
                var allUsers = JObject.Parse(usersInfo);

                var AcquirersUsers = allUsers[Globals.AcquirersUsersKey];

                var alienatorsUsers = allUsers[Globals.AlienatorsUsersKey];
                ;

                if (inscription.CNE == Globals.RegularizacionPatrimonioKey)
                {
                    EquityRegulation(allAcquirersInfo, inscription);
                } else if (inscription.CNE == "Compraventa")
                {
                    BuyingAndSelling(allAcquirersInfo, allAlienatorsInfo, inscription);
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
            List<Multyproperty> multypropertiesAlienator = SearchMulypropertiesinDataBase(inscription, year, rutAlienator);

            Multyproperty multyAlienator = multypropertiesAlienator[0];
            double percentageTransfer = (double)(multyAlienator.Percentage * percentageAlienator) / Globals.MaxPercentage;
            double? percentageAlienatorBeforeChange = multyAlienator.Percentage;
            multyAlienator.Percentage -= percentageTransfer;
            db.Entry(multyAlienator);
            db.SaveChanges();

            double percentageRecived = (double)(percentageAlienatorBeforeChange * percentageAcquirer) / Globals.MaxPercentage;
            Person acquirerPerson = CreateOrSetPerson(rutAcquirer);
            Acquirer acquirerInstance = CreateAcquirer(acquirerPerson, percentageRecived, inscription);
           
            List<Multyproperty> multypropertiesAcquirer = SearchMulypropertiesinDataBase(inscription, year, rutAcquirer);
            if (multypropertiesAcquirer.Count > 0)
            {
                Multyproperty multypropertyAcquirer = multypropertiesAcquirer[0];
                multypropertyAcquirer.Percentage += percentageRecived;
                db.Entry(multypropertyAcquirer);
                db.SaveChanges();
            }
            else CreateMultypropertyForBuyingAndSelling(acquirerInstance, inscription);
            StandardizeMultyproperties(year, inscription);

        }

        public void BuyingAndSellingPercentageEqualTo100(List<Tuple<string, double>> acquirers, List<Tuple<string, double>> alienators, Inscription inscription)
        {
            int year = inscription.InscriptionDate.Year;
            double? sumPercentagetoTransfer = 0;
            foreach (var alienatorInfo in alienators)
            {
                string rutAlienator = alienatorInfo.Item1;
                double percentageAlienator = alienatorInfo.Item2;
                Person alienatorPerson = CreateOrSetPerson(rutAlienator);
                Alienator alienatorInstance = CreateAlienator(alienatorPerson, percentageAlienator, inscription);
                Multyproperty multypropertyAlienator = SearchMulypropertiesinDataBase(inscription, year, rutAlienator)[0];
                sumPercentagetoTransfer += multypropertyAlienator.Percentage;
                if (multypropertyAlienator.StartCurrencyYear == year) db.Multyproperties.Remove(multypropertyAlienator);
                else
                {
                    multypropertyAlienator.EndCurrencyYear = year-1;
                    db.Entry(multypropertyAlienator);
                }
                db.SaveChanges();
            }
            foreach (var acquirerInfo in acquirers)
            {
                string rutAcquirer = acquirerInfo.Item1;
                double percentageAcquirer = acquirerInfo.Item2;
                Person acquirerPerson = CreateOrSetPerson(rutAcquirer);
                double percentageToRecive = (double)(sumPercentagetoTransfer * percentageAcquirer) / Globals.MaxPercentage;
                Acquirer acquirerInstance = CreateAcquirer(acquirerPerson, percentageToRecive, inscription);
                List<Multyproperty> multypropertiesAcquirer = SearchMulypropertiesinDataBase(inscription, year, rutAcquirer);
                if (multypropertiesAcquirer.Count > Globals.EmptyKey)
                {
                    Multyproperty multypropertyAcquirer = multypropertiesAcquirer[0];
                    multypropertyAcquirer.Percentage += percentageToRecive;
                    db.Entry(multypropertyAcquirer);
                    db.SaveChanges();
                }
                else CreateMultypropertyForBuyingAndSelling(acquirerInstance, inscription);
                StandardizeMultyproperties(year, inscription);
            }
        }

        public void BuyingAndSellingThirdCase(List<Tuple<string, double>> acquirers, List<Tuple<string, double>> alienators, Inscription inscription)
        {
            int year = inscription.InscriptionDate.Year;

            foreach (var alienatorInfo in alienators)
            {
                string rutAlienator = alienatorInfo.Item1;
                double percentageAlienator = alienatorInfo.Item2;
                Person alienatorPerson = CreateOrSetPerson(rutAlienator);
                Alienator alienatorInstance = CreateAlienator(alienatorPerson, percentageAlienator, inscription);
                Multyproperty multypropertyAlienator = SearchMulypropertiesinDataBase(inscription, year, rutAlienator)[0];
                multypropertyAlienator.Percentage -= percentageAlienator;
                if (multypropertyAlienator.Percentage < 0)
                {
                    multypropertyAlienator.Percentage = 0;
                    db.Multyproperties.Remove(multypropertyAlienator);
                    db.SaveChanges();
                } 
                db.Entry(multypropertyAlienator);
                db.SaveChanges();
            }
            foreach(var acquirersInfo in acquirers)
            {
                string rutAcquirer = acquirersInfo.Item1;
                double percentageAcquirer = acquirersInfo.Item2;
                Person acquirerPerson = CreateOrSetPerson(rutAcquirer);
                Acquirer acquirerInstance = CreateAcquirer(acquirerPerson, percentageAcquirer, inscription);
                List<Multyproperty> multypropertiesAcquirer = SearchMulypropertiesinDataBase(inscription, year, rutAcquirer);
                if (multypropertiesAcquirer.Count > Globals.EmptyKey)
                {
                    Multyproperty multypropertyAcquirer = multypropertiesAcquirer[0];
                    multypropertyAcquirer.Percentage += percentageAcquirer;
                    db.Entry(multypropertyAcquirer);
                    db.SaveChanges();
                }
                else CreateMultypropertyForBuyingAndSelling(acquirerInstance, inscription);
            }

            StandardizeMultyproperties(year, inscription);

        }

        //Example of returned object: List<Tuple<List<Tuple<string, double>>, double, double>> 
        //{
        //    ""alienators_users"": [
        //        [""Alienator1"", 10.5],
        //        [""Alienator2"", 15.75]
        //    ],
        //    ""acquirers_users"": [
        //        [""Acquirer1"", 20.25],
        //        [""Acquirer2"", 30.15],
        //        [""Acquirer3"", 5.5]
        //    ]
        //}
        private List<Tuple<List<Tuple<string, double>>, double, double>> GetUsersJsonToLists(string peopleInfo)
        {
            List<Tuple<List<Tuple<string, double>>, double, double>> allPeople = new List<Tuple<List<Tuple<string, double>>, double, double>>();
            List<Tuple<string, double>> alienators = new List<Tuple<string, double>>();
            List<Tuple<string, double>> acquirers = new List<Tuple<string, double>>();
            var jsonUsers = JObject.Parse(peopleInfo);

            var alienatorsUsers = jsonUsers[Globals.AlienatorsUsersKey];
            double sumPercentageAlienators = alienatorsUsers.Sum(alienator => Convert.ToDouble(alienator[1]));
            int countAlienators = alienatorsUsers.Count(alienator => Convert.ToDouble(alienator[1]) == 0);
            double percentageForAlienator = (Globals.MaxPercentage - sumPercentageAlienators) / countAlienators;

            var AcquirersUsers = jsonUsers[Globals.AcquirersUsersKey];
            double sumPercentageAcquirers = AcquirersUsers.Sum(aquirer => Convert.ToDouble(aquirer[1]));
            int countAcquirers = AcquirersUsers.Count(aquirer => Convert.ToDouble(aquirer[1]) == 0);
            double percentageForAcquirer = (Globals.MaxPercentage - sumPercentageAcquirers) / countAcquirers;

            if (jsonUsers.ContainsKey(Globals.AlienatorsUsersKey))
            {
                foreach (var alienatorInfo in alienatorsUsers)
                {
                    string rutAlienator = alienatorInfo[0].ToString();
                    double percentageAlienator = Convert.ToDouble(alienatorInfo[1]);
                    Tuple<string, double> alienatorTuple = new Tuple<string, double>(rutAlienator, percentageAlienator);
                    alienators.Add(alienatorTuple);
                }
                Tuple<List<Tuple<string, double>>, double, double> allAlienatorsTuple = new Tuple<List<Tuple<string, double>>, double, double>(alienators, percentageForAlienator, sumPercentageAlienators);
                allPeople.Add(allAlienatorsTuple);
            }

            if (jsonUsers.ContainsKey(Globals.AcquirersUsersKey))
            {
                foreach (var acquirerInfo in AcquirersUsers)
                {
                    string rutAcquirer = acquirerInfo[0].ToString();
                    double percentageAcquirer = Convert.ToDouble(acquirerInfo[1]);
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
            if (multyproperties.Count > Globals.EmptyKey)
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
            if (inscription.InscriptionDate.Year < Globals.OldestYearKey) yearOfStart = Globals.OldestYearKey;
            else yearOfStart = inscription.InscriptionDate.Year;

            if (multyproperties.Count > Globals.EmptyKey)
            {
                List<Multyproperty> multypropertiesSameYear = multyproperties.Where(
                    mp => mp.StartCurrencyYear == yearOfStart
                    ).ToList();
                if (multypropertiesSameYear.Count > Globals.EmptyKey)
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
                if (multyproperties.Count > Globals.EmptyKey && ifCreateMultyproperty == true )
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
                StandardizeMultyproperties(yearOfStart, inscription);
            }
        }

        private void CreateMultypropertyForBuyingAndSelling(Acquirer acquirer,Inscription inscription)
        {
            List<Multyproperty> multyproperties = SearchMulypropertiesinDataBase(inscription);
            bool ifCreateMultyproperty = true;
            int? endYear = null;
            int yearOfStart;
            if (inscription.InscriptionDate.Year < Globals.OldestYearKey) yearOfStart = Globals.OldestYearKey;
            else yearOfStart = inscription.InscriptionDate.Year;
            if (multyproperties.Count > Globals.EmptyKey)
            {
                List<Multyproperty> multypropertiesSameYear = multyproperties.Where(
                    mp => mp.StartCurrencyYear == yearOfStart
                    ).ToList();
                if (multypropertiesSameYear.Count > Globals.EmptyKey)
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

        private List<Multyproperty> SearchMulypropertiesinDataBase(Inscription inscription, int? year  = null, string rut = null)
        {
            List<Multyproperty> multyproperties = new List<Multyproperty> ();
            if (rut == null && year == null)
            {
                multyproperties = db.Multyproperties.Where(
                mp => mp.Comunne == inscription.Comunne
                && mp.Block == inscription.Block
                && mp.Site == inscription.Site
                && mp.AtentionNumber != inscription.AtentionNumber
                ).OrderByDescending(mp => mp.InscriptionDate).ToList();
            }
            else if(rut == null)
            {
                multyproperties = db.Multyproperties.Where(
                mp => mp.Comunne == inscription.Comunne
                && mp.Block == inscription.Block
                && mp.Site == inscription.Site
                && mp.InscriptionYear == year
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
                && mp.InscriptionYear <= year
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

            if (sumPercentageAcquirers == Globals.MaxPercentage)
            {
                BuyingAndSellingPercentageEqualTo100(acquirers, alienators,inscription);
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
                Tuple<string, double> acquirer = acquirers[0];
                string rutAcquirer = acquirer.Item1;
                double percentageAcquirer = acquirer.Item2;

                Tuple<string, double> alienator = alienators[0];
                string rutAlienator = alienator.Item1;
                double percentageAlienator = alienator.Item2; 
                BuyingAndSellingThirdCase(acquirers, alienators, inscription);
            }
        }

        private void StandardizeMultyproperties(int year,Inscription inscription)
        {
            List<Multyproperty> newStateMultiproperties = SearchMulypropertiesinDataBase(inscription, year);
            double? sumActualPercentage = newStateMultiproperties.Sum(x => x.Percentage);
            if (sumActualPercentage > Globals.MaxPercentage)
            {
                foreach (Multyproperty multyproperty in newStateMultiproperties)
                {
                    double? newPercentage = (double)(multyproperty.Percentage * Globals.MaxPercentage) / sumActualPercentage;
                    multyproperty.Percentage = Math.Round((double)newPercentage,2);
                    db.Entry(multyproperty);
                }
            }
            else
            {
                foreach (Multyproperty multyproperty in newStateMultiproperties)
                {
                    multyproperty.Percentage = Math.Round((double)multyproperty.Percentage, 2);
                    db.Entry(multyproperty);
                }
            }
            db.SaveChanges();
        }
    }

}
