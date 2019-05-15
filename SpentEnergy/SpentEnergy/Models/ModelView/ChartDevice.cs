using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SpentEnergy.Models.ModelView
{
    public class ChartDevice
    {
        public int IdDev { get; set; }
        public string NameDisp { get; set; }
        public List<Chart> ChartDev { get; set; }
    }
}