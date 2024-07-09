using Business.Repository.Interface;
using Business.Statics;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using PosSets.Models;
using System;
using System.Diagnostics;
using System.Diagnostics.Eventing.Reader;
using System.Xml.Schema;

namespace PosSets.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ILogic _logic;
        public HomeController(ILogger<HomeController> logger, ILogic logic)
        {
            _logger = logger;
            _logic = logic;
        }
        public IActionResult Index()
        {
            return View();
        }


        [HttpPost]
        public IActionResult UploadFile(IFormFile file)
        {
            if (file != null && file.Length > 0)
            {
                try
                {
                    var isvalid = _logic.UploadFiles(file);
                    if (isvalid)
                    {
                        return RedirectToAction("ReadData");
                    }
                    return NotFound();
                }
                catch (Exception ex)
                {
                    return NotFound();
                }
            }
            else
            {
                return NotFound();
            }
        }

        public IActionResult ReadData()
        {

            var lines = _logic.InputReder();
            if (lines == null || lines.Count() < 0)
            {
                return View();
            }
            // Validate Input and Get Users Names
            var Inputs = _logic.UserDetailsOfLines(lines);
            if (Inputs.ErrorLine != 0)
            {
                ViewBag.Error = Inputs.ErrorLine;
                return View();
            }

            var UserDetailsGroup = Inputs.UsersDetail.GroupBy(x => (x.Id, x.Date));
            var CurrentDate = UserDetailsGroup.First().First().Date;


            foreach (var person in UserDetailsGroup)
            {

                // Create UserInfo
                if (CurrentDate == person.Key.Date)
                {
                    Inputs.UsersId.Remove(person.First().Id);
                }
                else
                {
                    CurrentDate = person.Key.Date;
                    // Date Have Chaneged Absence mush Write
                }
                if (person.Count() % 2 != 0)
                {
                    // Error Type                        
                }
                var FirstArrive = person.First().Time;
                var Arrive = person.First().Time;
                var Out = person.First().Time;
                var TotalTime=TimeSpan.Zero;
                var counter=person.Count();
                for (int i = 0; i < person.Count(); i++)
                {
                    if (i==0)
                    {
                        continue;
                    }
                    if (i%2!=0)
                    {
                        Arrive=person.First().Time;
                    }
                    if (i % 2 == 0 && person.First().Time > LawOfTime.MinOfArrive)
                    {
                        if (Arrive<LawOfTime.MinOfArrive)
                        {
                            Arrive=LawOfTime.MinOfArrive;
                            FirstArrive = LawOfTime.MinOfArrive;
                        }
                        if (person.First().Time>LawOfTime.MaxOfChckout)
                        {
                            Out = LawOfTime.MaxOfChckout;
                        }
                        Out=person.First().Time;
                    }
                    TotalTime += Out-Arrive;


                }
                // check USerInfo Status
                if (FirstArrive>LawOfTime.MaxOfArrive)
                {
                    // Set Delay Status
                }
                if (TotalTime >= LawOfTime.StandarTime)
                {
                    if (counter>2)
                    {
                        // Leave_Hours
                    }
                    // Normal

                }
               
            }


            //  IEnumerable<IGrouping<(int Id, DateTime Date), InputDetail>> enumerable = VaidltorAndNames.Item1.GroupBy(x => (x.Id, x.Date));

            //enumerable.FirstOrDefault(x => x.Key == (1, new DateTime()));


            //var UserInfo = _logic.EntryAndExitRegistration(VaidltorAndNames.Item1, VaidltorAndNames.Item2);
            //_logic.CreateFiles(UserInfo);
            //return View(UserInfo);

            return null;
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}