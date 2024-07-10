using Business.Repository.Interface;
using Business.Statics;
using Infrastructure.Models;
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
            var UserInfos = new List<UserInfo>();

            var Absence = new Dictionary<int, string>(Inputs.UsersId);

            int Row = 0;
            for (int ii = 0; ii < UserDetailsGroup.Count(); ii++)
            {
                var person = UserDetailsGroup.ElementAt(ii);
                var UserInfo = new UserInfo();
                Row++;
                // Create UserInfo
                if (CurrentDate == person.Key.Date)
                {
                    Absence.Remove(person.First().Id);
                }
                if (ii+1==UserDetailsGroup.Count() || ii+1< UserDetailsGroup.Count())
                {
                    if (ii+1 == UserDetailsGroup.Count() || CurrentDate != UserDetailsGroup.ElementAt(ii+1).Key.Date)
                    {
                        var AbsenceCount = Absence.Count;

                        for (int j = 0; j < AbsenceCount; j++)
                        {
                            UserInfo User = new UserInfo();
                            User.Id = Absence.ElementAt(j).Key;
                            User.Day = ShamsiDayOfWeek.GetDayShamsi(CurrentDate.Date.DayOfWeek.ToString());
                            User.Name = Absence.ElementAt(j).Value;
                            User.FirstArrive = LawOfTime.ZeroTime;
                            User.LastCheckout = LawOfTime.ZeroTime;
                            User.Record.Add("0:0");
                            User.row = Row++;
                            User.WorkTime = LawOfTime.ZeroTime;
                            User.Status.Add(type.daily_leave);
                            User.DateTime = CurrentDate.Date;
                            UserInfos.Add(User);
                        }
                        Absence.Clear();
                        Absence = new Dictionary<int, string>(Inputs.UsersId);
                        if (ii+1!= UserDetailsGroup.Count())
                        {
                            CurrentDate = UserDetailsGroup.ElementAt(ii + 1).Key.Date;
                        }
                        // Date Have Chaneged Absence mush Write
                    }
                }

                var FirstArrive = person.First().Time;
                var Arrive = person.First().Time;
                var Out = person.First().Time;
                var TotalTime = TimeSpan.Zero;
                var counter = person.Count();
                var Recoreds = new List<string>();
                for (int i = 0; i < person.Count(); i++)
                {
                    Recoreds.Add(person.ElementAt(i).Time.ToString());
                    if (i == 0)
                    {
                        continue;
                    }
                    if (i % 2 == 0)
                    {
                        Arrive = person.ElementAt(i).Time;
                    }
                    if (i % 2 != 0 && person.ElementAt(i).Time > LawOfTime.MinOfArrive)
                    {
                        if (Arrive < LawOfTime.MinOfArrive)
                        {
                            Arrive = LawOfTime.MinOfArrive;
                        }
                        if (person.ElementAt(i).Time > LawOfTime.MaxOfChckout)
                        {
                            Out = LawOfTime.MaxOfChckout;
                        }
                        else
                        {
                            Out = person.ElementAt(i).Time;
                        }
                    }
                    if (i % 2 != 0 && person.ElementAt(i).Time < LawOfTime.MinOfArrive)
                    {
                        Out = Arrive;
                    }
                    if (i % 2 != 0)
                    {
                        TotalTime += Out - Arrive;
                    }

                }

                var Status = new List<type>();
                // check USerInfo Status
                if (counter % 2 != 0)
                {
                    Status.Add(type.error);
                }
                else
                {
                    if (FirstArrive > LawOfTime.MaxOfArrive)
                    {
                        Status.Add(type.delay);
                    }
                    if (Out < LawOfTime.MinOfCheckout)
                    {
                        Status.Add(type.hourly_leave);
                    }
                    else if (TotalTime >= LawOfTime.StandarTime)
                    {
                        if (counter > 2)
                        {
                            Status.Add(type.hourly_leave);
                        }
                        else
                        {
                            Status.Add(type.Normal);
                        }
                    }
                    else
                    {
                        Status.Add(type.hourly_leave);
                    }
                }
                UserInfo.Id = person.First().Id;
                UserInfo.Day = ShamsiDayOfWeek.GetDayShamsi(person.Key.Date.DayOfWeek.ToString());
                UserInfo.Name = person.First().Name;
                UserInfo.FirstArrive = FirstArrive;
                UserInfo.LastCheckout = Out;
                UserInfo.Record.AddRange(Recoreds);
                UserInfo.row = Row;
                UserInfo.WorkTime = TotalTime;
                UserInfo.Status.AddRange(Status);
                UserInfo.DateTime = person.Key.Date;
                UserInfos.Add(UserInfo);
            }

            return View(UserInfos);
       
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}