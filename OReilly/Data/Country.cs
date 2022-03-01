using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OReilly.Data
{
    public class Country
    {
        //We can use Id or CountryId and EF knows that its the primary key
        public int Id { get; set; }
        public string Name{ get; set; }
        public string ShortName { get; set; }
    }
}
