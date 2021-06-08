using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Keyword_Suggesion.Models
{
    public class ProjectKeywordstatus
    {
        public int Id { get; set; }

        [Display(Name = "Url")]
        public string Url { get; set; }

        [Display(Name = "Processed Date")]
        public DateTime Processed_Date { get; set; }

        [Display(Name = "keyword")]
        public string keyword { get; set; }

        [Display(Name = "City")]
        public string City { get; set; }

        [Display(Name = "Position")]
        public int Position { get; set; }

        [Display(Name = "For processed month year")]
        public string for_processed_moth_year { get; set; }

        [Display(Name = "Type")]
        public string Type { get; set; }
    }
}