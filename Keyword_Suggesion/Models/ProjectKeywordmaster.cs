using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Keyword_Suggesion.Models
{
    public class ProjectKeywordmaster
    {
        public int Id { get; set; }
        
        [Display(Name = "Project Name")]
        public string ProjectName { get; set; }

        [Display(Name = "Keyword")]
        public string Keyword { get; set; }

        [Display(Name = "City")]
        public string City { get; set; }


    }
}