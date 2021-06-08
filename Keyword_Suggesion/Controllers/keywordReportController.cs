using Keyword_Suggesion.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Keyword_Suggesion.Controllers
{
    [LoginCheck]

    public class keywordReportController : Controller
    {
        private string KeywordConnString = ConfigurationManager.ConnectionStrings["KeywordConnection"].ConnectionString;

        // GET: keywordReport
        public ActionResult Report(string Keyword, string Project)
        {
            //string Keyword = "Tools";
            SqlConnection con = new SqlConnection(KeywordConnString);
            SqlCommand cmd = new SqlCommand("proc_KeywordReport", con);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@ProjectKeyword", Keyword);
            cmd.Parameters.AddWithValue("@ProjectName", Project);
            SqlDataAdapter dataAdapter = new SqlDataAdapter(cmd);
            DataTable dataTable = new DataTable();
            dataAdapter.Fill(dataTable);
            return View(dataTable);
        }
    }
}