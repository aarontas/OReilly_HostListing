using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OReilly.Data
{
    public class ApiUser : IdentityUser //The IdentityUser have field to secure indentify
    {
        public string FristName { get; set; }
        public string LastName { get; set; }
    }
}
