using SpentEnergy.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

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


        public void saveDevice(String nrDev, String nameDev)
        {
            if (nrDev.Equals("") || nameDev.Equals(""))
            {
                throw new Exception("Informe os dados corretamente!!");
            }
            Device device = new Device() {
                NR_DEV = nrDev,
                NAME_DEV = nameDev
            }; 

            db.device.Add(device);

            db.SaveChanges();
        }
    }
}