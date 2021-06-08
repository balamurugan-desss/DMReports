using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Keyword_Suggesion.Models
{
    public class Keywords
    {
        [Required]
        [Display(Name = "Do You Want Create New Project Name?")]
        public string IsNew { get; set; }

        [Display(Name = "Create Project Name")]
        public string CreateProjectName { get; set; }

        [Display(Name = "Select Project Name")]
        public string SelectProjectName { get; set; }

        [Required] 
        public string keywords { get; set; }

        public string keywordList { get; set; }

    }
}