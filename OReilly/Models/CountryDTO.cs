using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace OReilly.Models
{
    //We create a layer to use instead of the Entity. We can use validation
    public class CreateCountryDTO
    {
        //Its are all the field necesary for the creation
        [Required]
        [StringLength(maximumLength: 50, ErrorMessage = "Country Name is too long")]
        public string Name { get; set; }

        [Required]
        [StringLength(maximumLength: 2, ErrorMessage = "ShortCountry Name is too long")]
        public string ShortName { get; set; }
    }

    public class UpdateCountryDTO : CreateCountryDTO 
    {
        public IList<CreateHotelDTO> Hotels { get; set; }
    }

    public class CountryDTO : CreateCountryDTO
    {
        public int Id { get; set; }
        public IList<HotelDTO> Hotels { get; set; }//Its seems like the Country domain with the list. The DTO have to match with de CreateDTO and the Domain
    }
}
