using SpentEnergy.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

/// <summary>
/// Cotroler de Devices 
/// </summary>
namespace SpentEnergy.Controllers
{
    public class DevicesController : Controller
    {

        Context db = new Context();

        // GET: Devices
        public ActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// Salva o dispositivo
        /// </summary>
        /// <param name="nrDev">numero do dispositivo</param>
        /// <param name="nameDev">nome do dispositivo</param>
        public void saveDevice(String nrDev, String nameDev)
        {
            //verifica se os parametros são nulos
            if (nrDev.Equals("") || nameDev.Equals(""))
            {
                throw new Exception("Informe os dados corretamente!!");
            }
            //Cria dispositivo para ser salva
            Device device = new Device() {
                NR_DEV = nrDev,
                NAME_DEV = nameDev
            }; 
            //Salva dispositivo
            db.device.Add(device);
            //commita alterações
            db.SaveChanges();
        }
    }
}