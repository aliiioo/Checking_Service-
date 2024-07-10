using Business.Repository.Interface;
using Business.Statics;
using Infrastructure.DTOs;
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
                    return BadRequest();
                }
                catch (Exception ex)
                {
                    return BadRequest();
                }
            }
            else
            {
                return BadRequest();
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

            var UserDetails = Inputs.UsersDetail.GroupBy(x => (x.Id, x.Date));
            var CurrentDate = UserDetails.First().First().Date;

            var UsersInfos = _logic.SaveUserInOut(UserDetails, CurrentDate,Inputs.UsersId);

            return View(UsersInfos);

        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}