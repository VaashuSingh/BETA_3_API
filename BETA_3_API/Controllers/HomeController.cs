using ESCommon;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BETA_3_API.Controllers
{
    public class HomeController : Controller
    {
        //string ConnectionString = ConfigurationManager.ConnectionStrings["ConString"].ToString();
        //private string WebApi = clsMain.MyString(System.Configuration.ConfigurationManager.AppSettings["WebApi"]);

        public ActionResult Index()
        {
            ViewBag.Title = "Home Page";

            return View();
        }
    }
}
