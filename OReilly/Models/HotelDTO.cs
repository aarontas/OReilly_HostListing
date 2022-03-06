using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace OReilly.Models
{
    public class CreateHotelDTO
    {
        [Required]
        [StringLength(maximumLength: 150, ErrorMessage = "Hotel Name is too long")]
        public string Name { get; set; }

        [Required]
        [StringLength(maximumLength: 120, ErrorMessage = "Address is too long")]
        public string Address { get; set; }

        [Required]
        [Range(1, 5)]
        public double Rating { get; set; }

        ///[Required]
        public int CountryId { get; set; }
    }

    public class UpdateHotelDTO : CreateHotelDTO //It is the same because we wont two class because the Single responsability
    { }

    public class HotelDTO : CreateHotelDTO
    {
        public int Id { get; set; }
        //We put CountryDTO instead of Country, because our DTO never have direct reference to the domain object directly
        //Automapper is the only connection between both
        public CountryDTO Country { get; set; }
    }
}
