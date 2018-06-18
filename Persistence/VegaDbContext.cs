using Microsoft.EntityFrameworkCore;
using vega.Models;

namespace vega.Persistence
{
    public class VegaDbContext:DbContext
    {
        public DbSet<Vehicle> Vehicles { get; set; }   
        public DbSet<Make> Makes { get; set; }

        public DbSet<Feature> Features { get; set; }

        public VegaDbContext(DbContextOptions<VegaDbContext> options)
        : base(options)
        {
            
        }

        // declare compose pk for m-n relation
        protected override void OnModelCreating(ModelBuilder modelBuilder) {
            // add m to n relation's pk
            modelBuilder.Entity<VehicleFeature>().HasKey(vf => new {
                vf.FeatureId, vf.VehicleId
            });
        }
        
    }
}