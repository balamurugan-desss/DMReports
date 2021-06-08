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
    public class ProjectReportController : Controller
    {
        private string KeywordConnString = ConfigurationManager.ConnectionStrings["KeywordConnection"].ConnectionString;
        // GET: ProjectReport
        public ActionResult ListProjectreports()
        {
            SqlConnection con = new SqlConnection(KeywordConnString);
            SqlCommand cmd = new SqlCommand("proc_listTblProjectReport", con);
            cmd.CommandType = CommandType.StoredProcedure;
            SqlDataAdapter dataAdapter = new SqlDataAdapter(cmd);
            DataTable dataTable = new DataTable();
            dataAdapter.Fill(dataTable);
            List<Report> ReportList = new List<Report>();
            for (int j = 0; j < dataTable.Rows.Count; j++)
            {
                Report report = new Report();
                report.project_Report_uid = Convert.ToInt32(dataTable.Rows[j]["project_Report_uid"]);
                report.ProjectName = dataTable.Rows[j]["ProjectName"].ToString();
                report.google_Organic = Convert.ToBoolean(dataTable.Rows[j]["google_Organic"]);
                report.Google_organic_cites = Convert.ToBoolean(dataTable.Rows[j]["Google_organic_cites"]);
                report.Google_my_business_cities = Convert.ToBoolean(dataTable.Rows[j]["Google_my_business_cities"]);
                ReportList.Add(report);

            }
            return View(ReportList);
        }
        [HttpGet]
        public ActionResult InsertProjectreports()
        {
            Report report = new Report();
            return View(report);
            
        }
        [HttpPost]
        public ActionResult InsertProjectreports(Report report)
        {
            if (!ModelState.IsValid)
                return View(report);

            SqlConnection con = new SqlConnection(KeywordConnString);
            SqlCommand cmd = new SqlCommand("proc_InsertTblProjectReport", con);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@ProjectName", report.ProjectName);
            cmd.Parameters.AddWithValue("@google_Organic", report.google_Organic);
            cmd.Parameters.AddWithValue("@Google_organic_cites", report.Google_organic_cites);
            cmd.Parameters.AddWithValue("@Google_my_business_cities", report.Google_my_business_cities);
            con.Open();
            cmd.ExecuteNonQuery();
            con.Close();
            return RedirectToAction("ListProjectreports");

        }
        [HttpGet]
        public ActionResult UpdateProjectreports(int id)
        {
            SqlConnection con = new SqlConnection(KeywordConnString);
            SqlCommand cmd = new SqlCommand("proc_EditTblProjectReport", con);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@project_Report_uid", id);
            SqlDataAdapter dataAdapter = new SqlDataAdapter(cmd);
            DataTable dataTable = new DataTable();
            dataAdapter.Fill(dataTable);
            Report report = new Report();
            report.ProjectName = dataTable.Rows[0]["ProjectName"].ToString();
            report.google_Organic = Convert.ToBoolean(dataTable.Rows[0]["google_Organic"]);
            report.Google_organic_cites = Convert.ToBoolean(dataTable.Rows[0]["Google_organic_cites"]);
            report.Google_my_business_cities = Convert.ToBoolean(dataTable.Rows[0]["Google_my_business_cities"]);
            report.project_Report_uid = Convert.ToInt32(dataTable.Rows[0]["project_Report_uid"]);

            return View(report);
        }
        [HttpPost]
        public ActionResult UpdateProjectreports(Report report)
        {
            if (!ModelState.IsValid)
                return View(report);

            SqlConnection con = new SqlConnection(KeywordConnString);
            SqlCommand cmd = new SqlCommand("proc_UpdateTblProjectReport", con);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@ProjectName", report.ProjectName);
            cmd.Parameters.AddWithValue("@google_Organic", report.google_Organic);
            cmd.Parameters.AddWithValue("@Google_organic_cites", report.Google_organic_cites);
            cmd.Parameters.AddWithValue("@Google_my_business_cities", report.Google_my_business_cities);
            cmd.Parameters.AddWithValue("@project_Report_uid", report.project_Report_uid);
            con.Open();
            cmd.ExecuteNonQuery();
            con.Close();
            return RedirectToAction("ListProjectreports");
        }
        public ActionResult DeleteProjectReport(string Id)
        {
            try
            {
                SqlConnection con = new SqlConnection(KeywordConnString);
                SqlCommand cmd = new SqlCommand("Proc_deletetblprojectReport", con);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@project_report_uid", Id);
                con.Open();
                cmd.ExecuteNonQuery();
                con.Close();

                return RedirectToAction("ListProjectreports");

            }
            catch (Exception ex)

            {
                return View(ex);

            }
        }

    }
}