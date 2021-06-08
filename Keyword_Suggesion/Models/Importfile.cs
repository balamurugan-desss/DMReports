using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Keyword_Suggesion.Models
{
    public class Importfile
    {
        [Required]
        [Display(Name = "Import Type")]
        public string ImportType { get; set; }
    }
}