using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Keyword_Suggesion.Models
{
    public class Cities
    {
        public int CityId { get; set; }
        [Display(Name = "City Name")]
        public string Cityname { get; set; }

        [Display(Name = "State Code")]
        public string State { get; set; }

        [Display(Name = "State Name")]
        public string StateName { get; set; }
    }
}