using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace TripShare.Models.TripViewModels
{
    public class RegisterTripFormViewModel
    {
        [Required]
        public DateTime Date { get; set; }
        [Required, MinLength(2)]
        public string From { get; set; }
        [Required, MinLength(2)]
        public string To { get; set; }
        public string AdditionalWay { get; set; }
        [Required]
        [Range(1, 300, ErrorMessage = "Please enter a value bigger than {1} and less than {2}")]
        public int SeatsCount { get; set; }
        [Required]
        [Range(0, int.MaxValue, ErrorMessage = "Please enter a value bigger than {1}")]
        public int Price { get; set; }
        [Range(0, int.MaxValue, ErrorMessage = "Please enter a value bigger than {1}")]
        [Required]
        public int Deposit { get; set; }
        [Required]
        public DateTime CancelDate { get; set; }
    }
}
