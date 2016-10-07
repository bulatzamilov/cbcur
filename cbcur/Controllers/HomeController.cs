using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace cbcur.Controllers
{
    public class HomeController : Controller
    {
        public class GraphGenerator
        {
            private string graph = @"$(function () {
    $('#curgraph').highcharts({
        title: {
            text: '{0}',
            x: -20 //center
        },
        subtitle: {
            text: 'Source: WorldClimate.com',
            x: -20
        },
        xAxis: {
            categories: ['Jan', 'Feb', 'Mar', 'Apr', 'May', 'Jun',
                'Jul', 'Aug', 'Sep', 'Oct', 'Nov', 'Dec']
        },
        yAxis: {
            title: {
                text: 'Temperature (°C)'
            },
            plotLines: [{
                value: 0,
                width: 1,
                color: '#808080'
            }]
        },
        tooltip: {
            valueSuffix: '°C'
        },
        legend: {
            layout: 'vertical',
            align: 'right',
            verticalAlign: 'middle',
            borderWidth: 0
        },
        series: [{
            name: 'Tokyo',
            data: [7.0, 6.9, 9.5, 14.5, 18.2, 21.5, 25.2, 26.5, 23.3, 18.3, 13.9, 9.6]
        }]
    });
});
";
            public string GetGraph()
            {
                return graph;
            }
        }


        public ActionResult Index()
        {
            var graph = new GraphGenerator();
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
            var i = 0;
            foreach (var currency in App_Start.CB.CurrencyArray)
            {
                // US Dollar as default
                if (currency.Code == "R01235")
                {
                    //curnames.Add(new SelectListItem { Text = currency.Name, Value = (++i).ToString(), Selected = true });
                    curnames.Add(new SelectListItem { Text = currency.Name, Value = currency.Code, Selected = true });
                    ViewBag.CurrencyTitle = currency.Name;
                    currencyrate = currency.GetRate(new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1), DateTime.Today);
                }
                else
                {
                    //curnames.Add(new SelectListItem { Text = currency.Name + " " + currency.Code, Value = (++i).ToString() });
                    curnames.Add(new SelectListItem { Text = currency.Name, Value = currency.Code });

                }
            }
            ViewBag.CurrencyNames = curnames;

            List<SelectListItem> months = new List<SelectListItem>();
            months.Add(new SelectListItem { Text = "JAN", Value = "1" });
            months.Add(new SelectListItem { Text = "FEB", Value = "2" });
            months.Add(new SelectListItem { Text = "MAR", Value = "3" });
            months.Add(new SelectListItem { Text = "APR", Value = "4" });
            months.Add(new SelectListItem { Text = "MAY", Value = "5" });
            months.Add(new SelectListItem { Text = "JUN", Value = "6" });
            months.Add(new SelectListItem { Text = "JUL", Value = "7" });
            months.Add(new SelectListItem { Text = "AUG", Value = "8" });
            months.Add(new SelectListItem { Text = "SEP", Value = "9" });
            months.Add(new SelectListItem { Text = "OCT", Value = "10" });
            months.Add(new SelectListItem { Text = "NOV", Value = "11" });
            months.Add(new SelectListItem { Text = "DEC", Value = "12" });
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

        public ActionResult UpdateChart()
        {
            return View();
        }
    }
}