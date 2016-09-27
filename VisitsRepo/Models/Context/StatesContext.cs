using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.Validation;
using System.Collections.Generic;

namespace VisitsRepo.Models
{
    public class StatesContext : DbContext
    {
        public StatesContext() 
            : base("name=VisitRepoDBConnection") 
        {
            base.Configuration.ProxyCreationEnabled = false;
        }

        public DbSet<state> States { get; set; }
        public DbSet<city> Cities { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<city>()
                .HasRequired(ci => ci.state)        // require to have state reference
                .WithMany(st => st.cities)          // one to many relation
                .HasForeignKey(ci => ci.stateid);   // mention foreign key used in table
        }

        protected override DbEntityValidationResult ValidateEntity(DbEntityEntry entityEntry, IDictionary<object, object> items)
        {
            var errors = new List<DbValidationError>();
            if (entityEntry.Entity is state)
            {
                if (entityEntry.CurrentValues.GetValue<string>("name") == string.Empty)
                {
                    errors.Add(new DbValidationError("Name", "Name is required"));
                }
                if (entityEntry.CurrentValues.GetValue<string>("abbreviation") == string.Empty)
                {
                    errors.Add(new DbValidationError("Abbreviation", "Abbreviation is required"));
                }

                if (errors.Count > 0)
                {
                    return new DbEntityValidationResult(entityEntry, errors);
                }
            }
            if (entityEntry.Entity is city)
            {
                if (entityEntry.CurrentValues.GetValue<int>("stateid") == -1)
                {
                    errors.Add(new DbValidationError("StateID", "StateID is required"));
                }
                if (entityEntry.CurrentValues.GetValue<string>("name") == string.Empty)
                {
                    errors.Add(new DbValidationError("Name", "Name is required"));
                }

                if (errors.Count > 0)
                {
                    return new DbEntityValidationResult(entityEntry, errors);
                }
            }
            return base.ValidateEntity(entityEntry, items);
        }
    }
}