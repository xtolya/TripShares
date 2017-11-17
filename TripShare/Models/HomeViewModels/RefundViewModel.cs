using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace TripShare.Models.HomeViewModels
{
    public class RefundViewModel
    {
        [Required]
        public string Address { get; set; }

        [Required]
        public int Amount { get; set; }
    }
}
