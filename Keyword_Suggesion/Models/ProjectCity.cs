using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Keyword_Suggesion.Models
{
    public class ProjectCity
    {
        public int Project_city_uid { get; set; }

        [Display(Name = "Project Name")]
        public string ProjectName { get; set; }

        [Display(Name = "City Name")]
        public string[] CityName { get; set; }

        [Display(Name = "Keyword Name")]
        public string KeywordName { get; set; }

        public string City { get; set; }

        public List<SelectListItem> CityNameList { get; set; }
    }
}