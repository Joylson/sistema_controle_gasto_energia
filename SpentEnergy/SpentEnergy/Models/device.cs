namespace SpentEnergy.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;


    /// <summary>
    /// Entidade representativa dos disposivos cadastrados
    /// </summary>
    [Table("spentenergy.device")]
    public partial class Device
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Device()
        {
            spent = new HashSet<Spent>();
        }

        public int ID { get; set; }

        [StringLength(30)]
        public string NR_DEV { get; set; }

        [StringLength(50)]
        public string NAME_DEV { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Spent> spent { get; set; }
    }
}
