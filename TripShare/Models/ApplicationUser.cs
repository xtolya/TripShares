using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;


namespace TripShare.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string Address { get; set; }
        [MinLength(2), MaxLength(50)]
        public string FirstName { get; set; }
        [MinLength(2), MaxLength(50)]
        public string LastName { get; set; }
        public string Wif { get; set; }
        public string ScriptHash { get; set; }
    }
}
