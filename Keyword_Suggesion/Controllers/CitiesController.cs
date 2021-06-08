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
    public class CitiesController : Controller
    {
        private string KeywordConnString = ConfigurationManager.ConnectionStrings["KeywordConnection"].ConnectionString;

        // GET: Cities
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult AssignProjectCity(string ProjectName)
        {
            ViewBag.ProjectName = ProjectName.TrimEnd();
            TempData["Message"] = string.Empty;
            SqlConnection con = new SqlConnection(KeywordConnString);
            SqlCommand cmd = new SqlCommand("proc_CityNamelist", con);
            cmd.CommandType = CommandType.StoredProcedure;
            SqlDataAdapter dataAdapter = new SqlDataAdapter(cmd);
            DataTable dataTable = new DataTable();
            dataAdapter.Fill(dataTable);
            if (ProjectName != null)
            {
                SqlCommand cmd1 = new SqlCommand("proc_ProjectCityNamelist", con);
                cmd1.CommandType = CommandType.StoredProcedure;
                cmd1.Parameters.AddWithValue("@ProjectName", ProjectName);
                SqlDataAdapter dataAdapter1 = new SqlDataAdapter(cmd1);
                DataTable dataTable1 = new DataTable();
                dataAdapter1.Fill(dataTable1);
                if (dataTable1.Rows.Count > 0)
                {
                    string Cities = string.Empty;
                    for (int i = 0; i < dataTable1.Rows.Count; i++)
                    {
                        Cities += dataTable1.Rows[i]["CityName"].ToString() + ",";

                    }
                    Cities = Cities.TrimStart(',');
                    TempData["Message"] = string.Join(",", Cities);
                }
            }
            List<ProjectCity> Citylist = new List<ProjectCity>();
            for (int j = 0; j < dataTable.Rows.Count; j++)
            {
                ProjectCity cities = new ProjectCity();
                //cities.CityId = Convert.ToInt32(dataTable.Rows[j]["CityIdCreateCitytName
                cities.ProjectName = ProjectName;
                cities.City = dataTable.Rows[j]["CityName"].ToString();
                //cities.State = dataTable.Rows[j]["State"].ToString();

                Citylist.Add(cities);

            }
            return View(Citylist);
        }



        [HttpPost]
        public ActionResult AssignProjectCity(string[] chkrow, string ProjectName)
        {
            try
            {
                TempData["Message"] = string.Empty;
                //ProjectCity projectCity = new ProjectCity();
                ProjectName = HttpUtility.ParseQueryString(Request.UrlReferrer.Query)["ProjectName"];
                string Cities = string.Join(",", chkrow);
                if (chkrow != null)
                {
                    TempData["Message"] = string.Join(",", chkrow);
                }
                SqlConnection con = new SqlConnection(KeywordConnString);

                SqlCommand del = new SqlCommand("proc_deleteprojectcity", con);
                del.CommandType = CommandType.StoredProcedure;
                del.Parameters.AddWithValue("@projectname", ProjectName);

                con.Open();
                del.ExecuteNonQuery();
                con.Close();
                string[] cityname = Cities.ToString().Split(',');
                for (int i = 0; i < cityname.Length; i++)
                {
                    SqlCommand cmd = new SqlCommand("proc_CreateProjectCity", con);

                    cmd.CommandType = CommandType.StoredProcedure;


                    cmd.Parameters.AddWithValue("@ProjectName", ProjectName.ToString());
                    cmd.Parameters.AddWithValue("@CityName", cityname[i].ToString());

                    con.Open();
                    cmd.ExecuteNonQuery();
                    con.Close();
                }

                if (ProjectName != null)
                    return RedirectToAction("ProjectCityListbyProject", "ProjectCities", new { ProjectName = ProjectName });
                else
                    return RedirectToAction("ProjectList", "Project_Keyword");
            }
            catch (Exception ex)
            {
                return View(ex);
            }
        }

        public ActionResult CityList()
        {
            SqlConnection con = new SqlConnection(KeywordConnString);
            SqlCommand cmd = new SqlCommand("proc_CityNamelist", con);
            cmd.CommandType = CommandType.StoredProcedure;
            SqlDataAdapter dataAdapter = new SqlDataAdapter(cmd);
            DataTable dataTable = new DataTable();
            dataAdapter.Fill(dataTable);
            List<Cities> Citylist = new List<Cities>();
            for (int j = 0; j < dataTable.Rows.Count; j++)
            {
                Cities cities = new Cities();
                cities.CityId = Convert.ToInt32(dataTable.Rows[j]["CityId"]);

                cities.Cityname = dataTable.Rows[j]["Cityname"].ToString();
                cities.State = dataTable.Rows[j]["State"].ToString();
                cities.StateName = dataTable.Rows[j]["StateName"].ToString();

                Citylist.Add(cities);

            }
            return View(Citylist);
        }

        [HttpGet]
        public ActionResult CreateCitytName()
        {
            Cities cities = new Cities();
            return View(cities);
        }
        [HttpPost]
        public ActionResult CreateCitytName(Cities cities)
        {
            if (!ModelState.IsValid)
                return View(cities);

            SqlConnection con = new SqlConnection(KeywordConnString);
            SqlCommand cmd = new SqlCommand("proc_CreateCity", con);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@Cityname", cities.Cityname);
            cmd.Parameters.AddWithValue("@State", cities.State);
            cmd.Parameters.AddWithValue("@StateName", cities.StateName);

            con.Open();
            cmd.ExecuteNonQuery();
            con.Close();
            return RedirectToAction("CityList");
        }

        [HttpGet]
        public ActionResult UpdateCityName(int CityId)
        {
            SqlConnection con = new SqlConnection(KeywordConnString);
            SqlCommand cmd = new SqlCommand("proc_EditCity", con);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@CityId", CityId);
            SqlDataAdapter dataAdapter = new SqlDataAdapter(cmd);
            DataTable dataTable = new DataTable();
            dataAdapter.Fill(dataTable);
            Cities cities = new Cities();
            cities.Cityname = dataTable.Rows[0]["Cityname"].ToString();
            cities.State = dataTable.Rows[0]["State"].ToString();
            cities.StateName = dataTable.Rows[0]["StateName"].ToString();

            return View(cities);
        }
        [HttpPost]
        public ActionResult UpdateCityName(Cities cities)
        {
            if (!ModelState.IsValid)
                return View(cities);

            SqlConnection con = new SqlConnection(KeywordConnString);
            SqlCommand cmd = new SqlCommand("proc_UpdateCity", con);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@Cityname", cities.Cityname);
            cmd.Parameters.AddWithValue("@State", cities.State);
            cmd.Parameters.AddWithValue("@StateName", cities.StateName);

            cmd.Parameters.AddWithValue("@CityId", cities.CityId);

            con.Open();
            cmd.ExecuteNonQuery();
            con.Close();
            return RedirectToAction("CityList");
        }

        public ActionResult DeleteCities(string CityId)
        {
            try
            {
                SqlConnection con = new SqlConnection(KeywordConnString);
                SqlCommand cmd = new SqlCommand("Proc_deletecities", con);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@CityId", CityId);
                con.Open();
                cmd.ExecuteNonQuery();
                con.Close();

                return RedirectToAction("CityList");

            }
            catch (Exception ex)

            {
                return View(ex);    

            }
        }

    }
}