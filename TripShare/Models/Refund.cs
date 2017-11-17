using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace TripShare.Models
{
    public class Refund
    {
        public int Id { get; set; }
        [Required]
        public string Address { get; set; }
        [Required]
        public int Amount { get; set; }
        [Required]
        public bool Done { get; set; }
    }
}
