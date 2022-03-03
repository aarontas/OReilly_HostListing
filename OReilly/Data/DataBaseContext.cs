using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
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

            //Hardcode the data in the database
            builder.Entity<Country>().HasData(
                    new Country
                    {
                        Id = 1,
                        Name = "Jamaica",
                        ShortName = "JM"
                    },
                    new Country
                    {
                        Id = 2,
                        Name = "Bahamas",
                        ShortName = "BS"
                    },
                    new Country
                    {
                        Id = 3,
                        Name = "Cayman Islan",
                        ShortName = "CI"
                    }
                );
            builder.Entity<Hotel>().HasData(
                    new Hotel
                    {
                        Id = 1,
                        Name = "Sandals Resort and Spa",
                        Address = "Negril",
                        CountryId = 1,
                        Rating = 4.5
                    },
                    new Hotel
                    {
                        Id = 2,
                        Name = "Confort Suites",
                        Address = "Negril",
                        CountryId = 3,
                        Rating = 4.3
                    }, new Hotel
                    {
                        Id = 3,
                        Name = "Grand Palldium",
                        Address = "Negril",
                        CountryId = 2,
                        Rating = 4
                    }
                );


        }
    }
}
