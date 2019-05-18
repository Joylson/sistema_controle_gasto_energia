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


/// <summary>
/// controler de retorno de dados de gasto de energia por aparelho
/// </summary>
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

        /// <summary>
        /// Returno de dados de gasto gerais
        /// </summary>
        /// <returns>json somatoria de gastos</returns>
        public ActionResult InfoSpents()
        {

            //criando enidade que será armazenada 
            //a somatoria dos gasto entre outras informações
            var info = new Infos();
            info.Potency = 0;
            info.Tension = 0;
            info.Calc = 0;
            info.ValueMed = 0;


            //entity framework lista todos os gastos em seguida 
            //submete a um loop somando os dados e adicionado a entidade info 
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

            //return informações en formato de json
            return Json(info, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Dados de consumo medio baseado na variavel CALC  
        /// </summary>
        /// <param name="type">tipo de filtro M(Mês), D(Dias), H(Horas)</param>
        /// <param name="year">Anos filtro</param>
        /// <param name="month">Mês filtro</param>
        /// <param name="day">Dia filtro</param>
        /// <returns>dados de consumo</returns>
        public ActionResult ChartConsumption(String type, int year, int month, int day)
        {

            //Lista todos os dispositivos
            var chartDevices = db.device.OrderBy(x => x.ID).Select(x =>
                new ChartDevice()
                {
                    IdDev = x.ID,
                    NameDisp = x.NAME_DEV
                }
            ).ToList();

            //Percorre todos os dispositivos para encontrar 
            //a somatoria de consumo de cada um
            foreach (ChartDevice chartDevice in chartDevices)
            {
                chartDevice.ChartDev = new List<Chart>();
                //filtro por mês mostra os consumos por mês durante o ano
                if (type == "M")
                {
                    //lista consumos 
                    //query resultante:
                    //SELECT MONTH(HOUR_REG), AVG(CALC) FROM SPENT
                    //WHERE ID_DEV = @ID_DEV AND YEAR(HOUR_REG) = @YEAR
                    //GROUP BY MONTH(HOUR_REG)
                    chartDevice.ChartDev = db.spent.ToList().
                        Where(
                            x => x.ID_DEV == chartDevice.IdDev //dispotivo
                            && Convert.ToDateTime(x.HOUR_REG).Year == year//ano
                        )
                        .GroupBy(
                            x => Convert.ToDateTime(x.HOUR_REG).Month //agrupado por mes
                        ).
                        Select(x => new Chart//variaveis resultantes da consulta 
                        {
                            AxisX = x.Key, //key valor presente no group(MONTH(HOUR_REG))
                            AxisY = Decimal.Parse(x.Average(a => a.CALC).ToString()) // x.average(a => a.CALC) -> media valor CALC
                        }).ToList();
                }
                //filtro por dia
                if (type == "D")
                {

                    //query resultante:
                    //SELECT DAY(HOUR_REG), AVG(CALC) FROM SPENT
                    //WHERE ID_DEV = @ID_DEV AND YEAR(HOUR_REG) = @YEAR AND MONTH(HOUR_REG) = @MONTH
                    //GROUP BY DAY(HOUR_REG)
                    chartDevice.ChartDev = db.spent.ToList().
                        Where(
                            x => x.ID_DEV == chartDevice.IdDev //id dispositivo
                            && Convert.ToDateTime(x.HOUR_REG).Year == year //ano de consulta
                            && Convert.ToDateTime(x.HOUR_REG).Month == month //mes de consulta
                        )
                        .GroupBy(
                            x => Convert.ToDateTime(x.HOUR_REG).Day//agrupado por dia
                        ).
                        Select(x => new Chart
                        {
                            AxisX = x.Key, // key valor presente no group(DAY(HOUR_REG))
                            AxisY = Decimal.Parse(x.Average(a => a.CALC).ToString())// x.average(a => a.CALC) -> media valor CALC
                        }).ToList();
                }

                //filtro por hora
                if (type == "H")
                {
                    //query resultante:
                    //SELECT HOUR(HOUR_REG), AVG(CALC) FROM SPENT
                    //WHERE ID_DEV = @ID_DEV AND YEAR(HOUR_REG) = @YEAR AND MONTH(HOUR_REG) = @MONTH AND DAY(HOUR_REG) = @DAY
                    //GROUP BY HOUR(HOUR_REG)
                    chartDevice.ChartDev = db.spent.ToList().
                        Where(
                            x => x.ID_DEV == chartDevice.IdDev 
                            && Convert.ToDateTime(x.HOUR_REG).Year == year //ano de consulta
                            && Convert.ToDateTime(x.HOUR_REG).Month == month //mes de consulta
                            && Convert.ToDateTime(x.HOUR_REG).Day == day//dia de consulta
                        )
                        .GroupBy(
                            x => Convert.ToDateTime(x.HOUR_REG).Hour//agrupado por hora
                        ).
                        Select(x => new Chart
                        {
                            AxisX = x.Key + 1, //key valor presente no group(DAY(HOUR_REG)) ->
                            //necesario somar um por padrao as horas começa com 0:00 e isso afeta nos graficos
                            AxisY = Decimal.Parse(x.Average(a => a.CALC).ToString())// x.average(a => a.CALC) -> media valor CALC
                        }).ToList();
                }
            }
            return Json(chartDevices, JsonRequestBehavior.AllowGet);
        }


        /// <summary>
        /// Dados de potencia medio baseado na coluna POTENCY
        /// </summary>
        /// <param name="type">tipo de filtro M(Mês), D(Dias), H(Horas)</param>
        /// <param name="year">filtro ano</param>
        /// <param name="month">filtro mes</param>
        /// <param name="day">filtro dia</param>
        /// <returns>json com a media de valor de potencia pro dispositivo</returns>
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

                //Os filtros são os mesmo especificados a cima seguindo a sequencia de tipo
                if (type == "M")
                {
                    //query resultante:
                    //SELECT MONTH(HOUR_REG), AVG(POTENCY) FROM SPENT
                    //WHERE ID_DEV = @ID_DEV AND YEAR(HOUR_REG) = @YEAR
                    //GROUP BY MONTH(HOUR_REG)
                    chartDevice.ChartDev = db.spent.ToList().
                        Where(
                            x => x.ID_DEV == chartDevice.IdDev 
                            && Convert.ToDateTime(x.HOUR_REG).Year == year
                        )
                        .GroupBy(
                            x => Convert.ToDateTime(x.HOUR_REG).Month
                        ).
                        Select(x => new Chart
                        {
                            AxisX = x.Key,
                            AxisY = Decimal.Parse(x.Average(a => a.POTENCY).ToString())// x.average(a => a.POTENCY) -> media valor POTENCY
                        }).ToList();
                }

                if (type == "D")
                {
                    //query resultante:
                    //SELECT DAY(HOUR_REG), AVG(POTENCY) FROM SPENT
                    //WHERE ID_DEV = @ID_DEV AND YEAR(HOUR_REG) = @YEAR AND MONTH(HOUR_REG) = @MONTH
                    //GROUP BY DAY(HOUR_REG)
                    chartDevice.ChartDev = db.spent.ToList().
                        Where(
                            x => x.ID_DEV == chartDevice.IdDev 
                            && Convert.ToDateTime(x.HOUR_REG).Year == year
                            && Convert.ToDateTime(x.HOUR_REG).Month == month
                        )
                        .GroupBy(
                            x => Convert.ToDateTime(x.HOUR_REG).Day
                        ).
                        Select(x => new Chart
                        {
                            AxisX = x.Key,
                            AxisY = Decimal.Parse(x.Average(a => a.POTENCY).ToString())// x.average(a => a.POTENCY) -> media valor POTENCY
                        }).ToList();
                }

                if (type == "H")
                {
                    //query resultante:
                    //SELECT HOUR(HOUR_REG), AVG(POTENCY) FROM SPENT
                    //WHERE ID_DEV = @ID_DEV AND YEAR(HOUR_REG) = @YEAR AND MONTH(HOUR_REG) = @MONTH AND DAY(HOUR_REG) = @DAY
                    //GROUP BY HOUR(HOUR_REG)
                    chartDevice.ChartDev = db.spent.ToList().
                        Where(
                            x => x.ID_DEV == chartDevice.IdDev 
                            && Convert.ToDateTime(x.HOUR_REG).Year == year
                            && Convert.ToDateTime(x.HOUR_REG).Month == month 
                            && Convert.ToDateTime(x.HOUR_REG).Day == day
                        )
                        .GroupBy(
                            x => Convert.ToDateTime(x.HOUR_REG).Hour
                        ).
                        Select(x => new Chart
                        {
                            AxisX = x.Key + 1,
                            AxisY = Decimal.Parse(x.Average(a => a.POTENCY).ToString())// x.average(a => a.POTENCY) -> media valor POTENCY
                        }).ToList();
                }
            }

            return Json(chartDevices, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Dados de gasto baseado na coluna VALUE_MED
        /// </summary>
        /// <param name="type">tipo de filtro M(Mês), D(Dias), H(Horas)</param>
        /// <param name="year">filtro ano</param>
        /// <param name="month">filtro mes</param>
        /// <param name="day">filtro dia</param>
        /// <returns>json com a somatoria de gasto por dispositivo</returns>
        public ActionResult ChartSpents(String type, int year, int month, int day)
        {
            var chartDevices = db.device.OrderBy(x => x.ID).Select(x =>
                new ChartDevice()
                {
                    IdDev = x.ID,
                    NameDisp = x.NAME_DEV
                }
            ).ToList();


            //Os filtros são os mesmo especificados a cima seguindo a sequencia de tipo
            foreach (ChartDevice chartDevice in chartDevices)
            {
                chartDevice.ChartDev = new List<Chart>();
                if (type == "M")
                {

                    //query resultante:
                    //SELECT MONTH(HOUR_REG), SUM(VALUE_MED) FROM SPENT
                    //WHERE ID_DEV = @ID_DEV AND YEAR(HOUR_REG) = @YEAR
                    //GROUP BY MONTH(HOUR_REG)
                    chartDevice.ChartDev = db.spent.ToList().
                        Where(
                            x => x.ID_DEV == chartDevice.IdDev 
                            && Convert.ToDateTime(x.HOUR_REG).Year == year
                        )
                        .GroupBy(
                            x => Convert.ToDateTime(x.HOUR_REG).Month
                        ).
                        Select(x => new Chart
                        {
                            AxisX = x.Key,
                            AxisY = Decimal.Parse(x.Sum(a => a.VALUE_MED).ToString())// x.Sum(a => a.POTENCY) -> somatoria valor VALUE_MED
                        }).ToList();
                }

                if (type == "D")
                {

                    //query resultante:
                    //SELECT DAY(HOUR_REG), SUM(VALUE_MED) FROM SPENT
                    //WHERE ID_DEV = @ID_DEV AND YEAR(HOUR_REG) = @YEAR AND MONTH(HOUR_REG) = @MONTH
                    //GROUP BY DAY(HOUR_REG)
                    chartDevice.ChartDev = db.spent.ToList().
                        Where(
                            x => x.ID_DEV == chartDevice.IdDev 
                            && Convert.ToDateTime(x.HOUR_REG).Year == year
                            && Convert.ToDateTime(x.HOUR_REG).Month == month
                        )
                        .GroupBy(
                            x => Convert.ToDateTime(x.HOUR_REG).Day
                        ).
                        Select(x => new Chart
                        {
                            AxisX = x.Key,
                            AxisY = Decimal.Parse(x.Sum(a => a.VALUE_MED).ToString())// x.Sum(a => a.VALUE_MED) -> somatoria valor VALUE_MED
                        }).ToList();
                }

                if (type == "H")
                {
                    //query resultante:
                    //SELECT HOUR(HOUR_REG), SUM(POTENCY) FROM SPENT
                    //WHERE ID_DEV = @ID_DEV AND YEAR(HOUR_REG) = @YEAR AND MONTH(HOUR_REG) = @MONTH AND DAY(HOUR_REG) = @DAY
                    //GROUP BY HOUR(HOUR_REG)
                    chartDevice.ChartDev = db.spent.ToList().
                        Where(
                            x => x.ID_DEV == chartDevice.IdDev 
                            && Convert.ToDateTime(x.HOUR_REG).Year == year
                            && Convert.ToDateTime(x.HOUR_REG).Month == month 
                            && Convert.ToDateTime(x.HOUR_REG).Day == day
                        )
                        .GroupBy(x => Convert.ToDateTime(x.HOUR_REG).Hour).
                        Select(x => new Chart
                        {
                            AxisX = x.Key + 1,
                            AxisY = Decimal.Parse(x.Sum(a => a.VALUE_MED).ToString())// x.Sum(a => a.VALUE_MED) -> somatoria valor VALUE_MED
                        }).ToList();
                }
            }
            return Json(chartDevices, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Dados de consumo,potencia ou gasto gerais por mes
        /// </summary>
        /// <param name="type">tipo de filtro P(Potencia), T(Tensão), C(CONSUMO)</param>
        /// <param name="year">filtro por ano</param>
        /// <returns></returns>
        public ActionResult ChartPieInfo(String type, int year)
        {

            var spents = db.spent.ToList();


            //listandos todos os gastos, potencia ou consumo por mes
            //query resultante:
            //SELECT SUM(POTENCY) FROM SPENT
            //WHERE YEAR(HOUR_REG) = @YEAR 
            //################################
            //SELECT SUM(CALC) FROM SPENT
            //WHERE YEAR(HOUR_REG) = @YEAR
            //################################
            //SELECT SUM(VALUE_MED) FROM SPENT
            //WHERE YEAR(HOUR_REG) = @YEAR
            var total = spents.Where(
                    x => Convert.ToDateTime(x.HOUR_REG).Year == year//filtro por mes
                )
                .OrderBy(
                    x => Convert.ToDateTime(x.HOUR_REG).Month//ordenado por mes
                )
                .GroupBy(
                    x => Convert.ToDateTime(x.HOUR_REG).Month//agrupado por mes
                ).ToList().Select(x =>
                new
                {
                    value = Convert.ToDecimal(x.Sum(s => (type == "P" ? s.POTENCY :  //tipo vai definir qual vai ser a somatoria P(Potencia), T(Tension), C(consumo)
                    (type == "T" ? s.TENSION : (type == "C" ? s.CALC : s.VALUE_MED))))),
                    month = Convert.ToDateTime(x.First().HOUR_REG).Month  //numero do mes correspondente
                }).ToList();

            var total_array = new Decimal[12];

            //percorre a listagem e atribui a um array
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
