using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace TripShare.Models
{
    public class Passenger
    {
        [Required]
        public int Id { get; set; }
        public string UserId { get; set; }
        public string TripId { get; set; }
    }
}
