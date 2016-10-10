using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;


namespace cbcur.App_Start
{
    public class CB
    {
        public static ru.cbr.www.DailyInfo dailyInfo { get; private set; }

        static CB()
        {
            dailyInfo = new ru.cbr.www.DailyInfo();
            DataSet ds = dailyInfo.EnumValutes(Seld: false);
            currencies = new Currency[ds.Tables[0].Rows.Count];
            
            for (int i = 0; i < currencies.Length; i++)
            {
                var currency = new Currency(ds.Tables[0].Rows[i].ItemArray[0] as string,
                    ds.Tables[0].Rows[i].ItemArray[2] as string,
                    ds.Tables[0].Rows[i].ItemArray[3] as string);
                currencies[i] = currency;
            }
        }

        private static Currency[] currencies;


        public static Currency[] CurrencyArray
        {
            get { return currencies; }
            set { currencies = value; }
        }
    }


    public class Currency
    {
        public string Code { get; set; }
        public string Name { get; set; }
        public string RateBase { get; set; }

        public Currency(string code, string name, string ratebase)
        {
            Code = code.Trim();
            Name = name.Trim();
            RateBase = (ratebase != null) ? ratebase.Trim(): ratebase;
        }

        public Tuple<DateTime, decimal>[] GetRate(DateTime from, DateTime to)
        {
            var days = (to - from).Days;
            Tuple<DateTime, decimal>[] res = new Tuple<DateTime, decimal>[31];
            DataSet dayrate = CB.dailyInfo.GetCursDynamic(from, to, Code);
            for (int i = 0; i < dayrate.Tables[0].Rows.Count; i++)
            {
                var day = (DateTime)dayrate.Tables[0].Rows[i].ItemArray[0];
                var rate = (decimal)dayrate.Tables[0].Rows[i].ItemArray[3];
                res[day.Day-1] = Tuple.Create(day, rate);
            }
            return res;
        }
    }
}