namespace SpentEnergy.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("spentenergy.spent")]
    public partial class Spent
    {
        public int ID { get; set; }

        public int ID_DEV { get; set; }

        public float? TENSION { get; set; }

        public float? POTENCY { get; set; }

        public float? CALC { get; set; }

        public float? VALUE_MED { get; set; }

        public DateTime? HOUR_REG { get; set; }

        public virtual Device device { get; set; }
    }
}
