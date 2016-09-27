using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.Validation;
using System.Collections.Generic;

namespace VisitsRepo.Models
{
    public class UserVisitsContext : DbContext
    {
        public UserVisitsContext() 
            : base("name=VisitRepoDBConnection") 
        {
            base.Configuration.ProxyCreationEnabled = false;
        }

        public DbSet<visits> Visits { get; set; }

        public DbSet<user> Users { get; set; }
        public DbSet<state> States { get; set; }
        public DbSet<city> Cities { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<visits>().HasRequired(v => v.user);
            modelBuilder.Entity<visits>().HasRequired(v => v.city);

            modelBuilder.Entity<city>()
                .HasRequired(ci => ci.state)        // require to have state reference
                .WithMany(st => st.cities)          // one to many relation
                .HasForeignKey(ci => ci.stateid);   // mention foreign key used in table
        }

        protected override DbEntityValidationResult ValidateEntity(DbEntityEntry entityEntry, IDictionary<object, object> items)
        {
            var errors = new List<DbValidationError>();
            if (entityEntry.Entity is visits)
            {   
                if (entityEntry.CurrentValues.GetValue<int>("userid") == -1)
                {
                    errors.Add(new DbValidationError("UserID", "UserID is required"));
                }
                if (entityEntry.CurrentValues.GetValue<int>("cityid") == -1)
                {
                    errors.Add(new DbValidationError("CityID", "CityID is required"));
                }

                if (errors.Count > 0)
                {
                    return new DbEntityValidationResult(entityEntry, errors);
                }
            }
            if (entityEntry.Entity is user)
            {
                if (entityEntry.CurrentValues.GetValue<string>("username") == string.Empty)
                {
                    errors.Add(new DbValidationError("UserName", "UserName is required"));
                    return new DbEntityValidationResult(entityEntry, errors);
                }
            }
            return base.ValidateEntity(entityEntry, items);
        }
    }
}