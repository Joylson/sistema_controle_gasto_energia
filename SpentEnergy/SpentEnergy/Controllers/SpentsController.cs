using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using SpentEnergy.Models;
using SpentEnergy.Models.ModelView;

namespace SpentEnergy.Controllers
{
    public class SpentsController : Controller
    {
        private Context db = new Context();

        public ActionResult Index()
        {
            var spent = db.spent.Include(s => s.device);
            return View(spent.ToList());
        }

        public ActionResult InfoSpents()
        {
            var info = new Infos();
            info.Potency = 0;
            info.Tension = 0;
            info.Calc = 0;
            info.ValueMed = 0;

            db.spent.ToList()
                .ForEach(x =>
                    info = new Infos()
                    {
                        Potency = info.Potency + Convert.ToDecimal(x.POTENCY),
                        Tension = info.Tension + Convert.ToDecimal(x.TENSION),
                        Calc = info.Calc + Convert.ToDecimal(x.CALC),
                        ValueMed = info.ValueMed + Convert.ToDecimal(x.VALUE_MED)
                    }
                );

            return Json(info, JsonRequestBehavior.AllowGet);
        }


        public ActionResult ChartConsumption(String type, int year, int month, int day)
        {
            var chartDevices = db.device.OrderBy(x => x.ID).Select(x =>
                new ChartDevice()
                {
                    IdDev = x.ID,
                    NameDisp = x.NAME_DEV
                }
            ).ToList();

            foreach (ChartDevice chartDevice in chartDevices)
            {
                chartDevice.ChartDev = new List<Chart>();
                if (type == "M")
                {
                    chartDevice.ChartDev = db.spent.ToList().
                        Where(x => x.ID_DEV == chartDevice.IdDev && Convert.ToDateTime(x.HOUR_REG).Year == year)
                        .GroupBy(x => Convert.ToDateTime(x.HOUR_REG).Month).
                        Select(x => new Chart
                        {
                            AxisX = x.Key,
                            AxisY = Decimal.Parse(x.Average(a => a.CALC).ToString())
                        }).ToList();
                }

                if (type == "D")
                {
                    chartDevice.ChartDev = db.spent.ToList().
                        Where(x => x.ID_DEV == chartDevice.IdDev && Convert.ToDateTime(x.HOUR_REG).Year == year
                       && Convert.ToDateTime(x.HOUR_REG).Month == month)
                        .GroupBy(x => Convert.ToDateTime(x.HOUR_REG).Day).
                        Select(x => new Chart
                        {
                            AxisX = x.Key,
                            AxisY = Decimal.Parse(x.Average(a => a.CALC).ToString())
                        }).ToList();
                }

                if (type == "H")
                {
                    chartDevice.ChartDev = db.spent.ToList().
                        Where(x => x.ID_DEV == chartDevice.IdDev && Convert.ToDateTime(x.HOUR_REG).Year == year
                       && Convert.ToDateTime(x.HOUR_REG).Month == month && Convert.ToDateTime(x.HOUR_REG).Day == day)
                        .GroupBy(x => Convert.ToDateTime(x.HOUR_REG).Hour).
                        Select(x => new Chart
                        {
                            AxisX = x.Key + 1,
                            AxisY = Decimal.Parse(x.Average(a => a.CALC).ToString())
                        }).ToList();
                }
            }
            return Json(chartDevices, JsonRequestBehavior.AllowGet);
        }


        public ActionResult ChartPotency(String type, int year, int month, int day)
        {
            var chartDevices = db.device.OrderBy(x => x.ID).Select(x =>
                new ChartDevice()
                {
                    IdDev = x.ID,
                    NameDisp = x.NAME_DEV
                }
            ).ToList();

            foreach (ChartDevice chartDevice in chartDevices)
            {
                chartDevice.ChartDev = new List<Chart>();
                if (type == "M")
                {
                    chartDevice.ChartDev = db.spent.ToList().
                        Where(x => x.ID_DEV == chartDevice.IdDev && Convert.ToDateTime(x.HOUR_REG).Year == year)
                        .GroupBy(x => Convert.ToDateTime(x.HOUR_REG).Month).
                        Select(x => new Chart
                        {
                            AxisX = x.Key,
                            AxisY = Decimal.Parse(x.Average(a => a.POTENCY).ToString())
                        }).ToList();
                }

                if (type == "D")
                {
                    chartDevice.ChartDev = db.spent.ToList().
                        Where(x => x.ID_DEV == chartDevice.IdDev && Convert.ToDateTime(x.HOUR_REG).Year == year
                       && Convert.ToDateTime(x.HOUR_REG).Month == month)
                        .GroupBy(x => Convert.ToDateTime(x.HOUR_REG).Day).
                        Select(x => new Chart
                        {
                            AxisX = x.Key,
                            AxisY = Decimal.Parse(x.Average(a => a.POTENCY).ToString())
                        }).ToList();
                }

                if (type == "H")
                {
                    chartDevice.ChartDev = db.spent.ToList().
                        Where(x => x.ID_DEV == chartDevice.IdDev && Convert.ToDateTime(x.HOUR_REG).Year == year
                       && Convert.ToDateTime(x.HOUR_REG).Month == month && Convert.ToDateTime(x.HOUR_REG).Day == day)
                        .GroupBy(x => Convert.ToDateTime(x.HOUR_REG).Hour).
                        Select(x => new Chart
                        {
                            AxisX = x.Key + 1,
                            AxisY = Decimal.Parse(x.Average(a => a.POTENCY).ToString())
                        }).ToList();
                }
            }

            return Json(chartDevices, JsonRequestBehavior.AllowGet);
        }

        public ActionResult ChartSpents(String type, int year, int month, int day)
        {
            var chartDevices = db.device.OrderBy(x => x.ID).Select(x =>
                new ChartDevice()
                {
                    IdDev = x.ID,
                    NameDisp = x.NAME_DEV
                }
            ).ToList();

            foreach (ChartDevice chartDevice in chartDevices)
            {
                chartDevice.ChartDev = new List<Chart>();
                if (type == "M")
                {
                    chartDevice.ChartDev = db.spent.ToList().
                        Where(x => x.ID_DEV == chartDevice.IdDev && Convert.ToDateTime(x.HOUR_REG).Year == year)
                        .GroupBy(x => Convert.ToDateTime(x.HOUR_REG).Month).
                        Select(x => new Chart
                        {
                            AxisX = x.Key,
                            AxisY = Decimal.Parse(x.Sum(a => a.VALUE_MED).ToString())
                        }).ToList();
                }

                if (type == "D")
                {
                    chartDevice.ChartDev = db.spent.ToList().
                        Where(x => x.ID_DEV == chartDevice.IdDev && Convert.ToDateTime(x.HOUR_REG).Year == year
                       && Convert.ToDateTime(x.HOUR_REG).Month == month)
                        .GroupBy(x => Convert.ToDateTime(x.HOUR_REG).Day).
                        Select(x => new Chart
                        {
                            AxisX = x.Key,
                            AxisY = Decimal.Parse(x.Sum(a => a.VALUE_MED).ToString())
                        }).ToList();
                }

                if (type == "H")
                {
                    chartDevice.ChartDev = db.spent.ToList().
                        Where(x => x.ID_DEV == chartDevice.IdDev && Convert.ToDateTime(x.HOUR_REG).Year == year
                       && Convert.ToDateTime(x.HOUR_REG).Month == month && Convert.ToDateTime(x.HOUR_REG).Day == day)
                        .GroupBy(x => Convert.ToDateTime(x.HOUR_REG).Hour).
                        Select(x => new Chart
                        {
                            AxisX = x.Key + 1,
                            AxisY = Decimal.Parse(x.Sum(a => a.VALUE_MED).ToString())
                        }).ToList();
                }
            }
            return Json(chartDevices, JsonRequestBehavior.AllowGet);
        }

        public ActionResult ChartPieInfo(String type, int year)
        {

            var spents = db.spent.ToList();

            var total = spents.Where(x => Convert.ToDateTime(x.HOUR_REG).Year == year)
                .OrderBy(x => Convert.ToDateTime(x.HOUR_REG).Month)
                .GroupBy(x => Convert.ToDateTime(x.HOUR_REG).Month).ToList()
                .Select(x =>
                new
                {
                    value = Convert.ToDecimal(x.Sum(s => (type == "P" ? s.POTENCY :
                    (type == "T" ? s.TENSION : (type == "C" ? s.CALC : s.VALUE_MED))))),
                    month = Convert.ToDateTime(x.First().HOUR_REG).Month
                }).ToList();
            var total_array = new Decimal[12];

            total.ForEach(x =>
                total_array[x.month - 1] = x.value
            );

            return Json(total_array, JsonRequestBehavior.AllowGet);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
