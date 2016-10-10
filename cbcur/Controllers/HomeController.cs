using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace cbcur.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            Tuple<DateTime, decimal>[] currencyrate = {Tuple.Create(DateTime.Today, (decimal)0)};

            //ViewBag.CurrencyGraph = graph.GetGraph();
            DateTime today = DateTime.Today;
            var monthdayscount = DateTime.DaysInMonth(today.Year, today.Month);
            string[] monthdays = new string[monthdayscount];
            for (int k = 1; k <= monthdayscount; k++)
                monthdays[k - 1] = k.ToString();
            ViewBag.Days = monthdays;
            
            //DataSet ds = cbcur.App_Start.CB.dailyInfo.EnumValutes(Seld: true);
            List<SelectListItem> curnames = new List<SelectListItem>();

            foreach (var currency in App_Start.CB.CurrencyArray)
            {
                // US Dollar as default
                if (currency.Code == "R01235")
                {
                    curnames.Add(new SelectListItem { Text = currency.Name, Value = currency.Code, Selected = true });
                    ViewBag.CurrencyTitle = currency.Name;
                    currencyrate = currency.GetRate(new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1), DateTime.Today);
                }
                else
                {
                    curnames.Add(new SelectListItem { Text = currency.Name, Value = currency.Code });
                }
            }
            ViewBag.CurrencyNames = curnames;

            List<SelectListItem> months = new List<SelectListItem>();
            for(int i=1; i<=12; i++)
            {
                if(i == today.Month)
                {
                    months.Add(new SelectListItem { Text = new DateTime(today.Year, i, 1).ToString("MMM") , Value = i.ToString(), Selected = true });

                }
                else
                {
                    months.Add(new SelectListItem { Text = new DateTime(today.Year, i, 1).ToString("MMM"), Value = i.ToString() });
                }
            }
            
            ViewBag.Months = months;
            ViewBag.Month = DateTime.Now.ToString("MMM");
          
            
            string[] rates = new string[currencyrate.Length];
            for(int l=0;l<currencyrate.Length; l++)
            {
                if (currencyrate[l] != null)
                    rates[l] = currencyrate[l].Item2.ToString().Replace(',','.');
            }
            ViewBag.CurrencyRates = rates;

            return View();
        }

        [HttpPost]
        public JsonResult UpdateChart()
        {
            string json;
            using (var reader = new StreamReader(Request.InputStream))
            {
                json = reader.ReadToEnd();
            }
            dynamic currency_ = JsonConvert.DeserializeObject(json);
            int month = currency_.Month;
            string currencyCode = currency_.CurrencyCode;

            string title = "";
            Tuple<DateTime, decimal>[] curRate = { Tuple.Create(DateTime.Today, (decimal)0) };

            DateTime today = DateTime.Today;
            DateTime startDate = new DateTime(today.Year, month, 1);
            int monthdayscount = DateTime.DaysInMonth(today.Year, month);
            string[] days = new string[monthdayscount];
            for (int k = 1; k <= monthdayscount; k++)
                days[k - 1] = k.ToString();
            DateTime finishDate = new DateTime(today.Year, month, monthdayscount);
            foreach (var currency in App_Start.CB.CurrencyArray)
            {
                if (currency.Code == currencyCode)
                {
                    title = currency.Name;
                    curRate = currency.GetRate(from: startDate, to: finishDate);
                }
            }

            string[] rates = new string[curRate.Length];
            for (int l = 0; l < curRate.Length; l++)
            {
                if (curRate[l] != null)
                    rates[l] = curRate[l].Item2.ToString().Replace(',', '.');
                else
                {
                    rates[l] = "null";
                }
            }
            string monthname = startDate.ToString("MMM");
            return Json(new {title = title,
                             month = monthname,
                             days = days,
                             rates = rates
            });
        }
    }
}