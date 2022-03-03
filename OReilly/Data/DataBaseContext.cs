using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using OReilly.Configurations.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OReilly.Data
{
    public class DataBaseContext : IdentityDbContext<ApiUser>//We need this to secure our access. In the ApiUser class we have a kind of Indetity user with properties that we need to check
    {
        //Options come from startup
        public DataBaseContext(DbContextOptions options) : base(options)
        { }

        //Countries is the database table name
        public DbSet<Country> Countries { get; set; }
        public DbSet<Hotel> Hotels { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            //Configure the roles
            builder.ApplyConfiguration(new CountryConfiguration());
            builder.ApplyConfiguration(new HotelConfiguration());
            builder.ApplyConfiguration(new RoleConfiguration());
        }
    }
}
