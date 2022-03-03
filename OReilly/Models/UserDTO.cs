using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace OReilly.Models
{
    /// <summary>
    /// This is a custom class that we need to the registration
    /// At least need to be like our ApiUser class because need this fields
    /// </summary>

    public class UserDTO : LoginDTO
    {
        public string FristName { get; set; }
        public string LastName { get; set; }

        [DataType(DataType.PhoneNumber)]
        public string PhoneNumber { get; set; }


    }

    //This is the same class but only with the fields that we need to login. The last one is necessary to register a new user
    public class LoginDTO
    {
        [Required]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }

        [Required]
        [StringLength(15, ErrorMessage = "Your password is limited to {2} to {1} characters", MinimumLength = 10)]
        public string Password { get; set; }
    }
}
