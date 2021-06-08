using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;

namespace Keyword_Suggesion.Models
{
    public class Login
    {
        [Required]
        [Display(Name = "User Name")]
        public string username { get; set; }
        [Required]

        [Display(Name = "Password")]
        public string password { get; set; }
    }

    public class Query
    {
        public string query { get; set; }
    }
    [DataContract]
    public class MonthlySearchVolume
    {

        [DataMember(Name = "year")]
        public int year { get; set; }

        [DataMember(Name = "month")]
        public int month { get; set; }

        [DataMember(Name = "count")]
        public int count { get; set; }
    }

    [DataContract]
    public class Example
    {

        [DataMember(Name = "keyword")]
        public string keyword { get; set; }

        [DataMember(Name = "competition")]
        public double competition { get; set; }

        [DataMember(Name = "search_volume")]
        public int search_volume { get; set; }

        [DataMember(Name = "monthly_search_volume")]
        public IList<MonthlySearchVolume> monthly_search_volume { get; set; }
    }
}