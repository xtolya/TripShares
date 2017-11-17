using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using TripShare.Abstract;

namespace TripShare.Models.ManageViewModels
{
    public class WithdrawViewModel
    {
        [Required]
        public string Address { get; set; }
        [Required]
        public int Amount { get; set; }
        [Required]
        public int Asset { get; set; }
    }
}
