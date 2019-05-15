namespace SpentEnergy.Models
{
    using System;
    using System.Data.Entity;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Linq;

    public partial class Context : DbContext
    {
        public Context()
            : base("name=Context")
        {
            Database.SetInitializer(new CreateDatabaseIfNotExists<Context>());
        }

        public virtual DbSet<Device> device { get; set; }
        public virtual DbSet<Spent> spent { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Device>()
                .Property(e => e.NR_DEV)
                .IsUnicode(false);

            modelBuilder.Entity<Device>()
                .Property(e => e.NAME_DEV)
                .IsUnicode(false);

            modelBuilder.Entity<Device>()
                .HasMany(e => e.spent)
                .WithRequired(e => e.device)
                .HasForeignKey(e => e.ID_DEV)
                .WillCascadeOnDelete(false);
        }
    }
}
