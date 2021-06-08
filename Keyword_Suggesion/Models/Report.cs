using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Keyword_Suggesion.Models
{
    public class Report
    {
        public int project_Report_uid { get; set; }
        [Required]
        [Display(Name = "Project Name")]
        public string ProjectName { get; set; }

        [Display(Name = "Organic Status")]
        public bool google_Organic { get; set; }

        [Display(Name = "Organic Cities Status")]
        public bool Google_organic_cites { get; set; }

        [Display(Name = "GMB Status")]
        public bool Google_my_business_cities { get; set; }
    }
}