using System.Data.Entity;
using WindowsFormsApp1.Model;

namespace WindowsFormsApp1.Data
{
    public class DataContext : DbContext
    {
        public DbSet<People> people { get; set; }
        public DataContext() : base("name=ConnectionString")
        {
            Database.SetInitializer(new CreateDatabaseIfNotExists<DataContext>());
        }
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
        }
    }
}
