using Keyword_Suggesion.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;

namespace Keyword_Suggesion.Controllers
{
    [LoginCheck]
    public class ProjectCitiesController : Controller
    {
        private string KeywordConnString = ConfigurationManager.ConnectionStrings["KeywordConnection"].ConnectionString;

        // GET: ProjectCities
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult ProjectCityList()
        {
            if (Session["ProjectName"] == null)
            {
                TempData["Message"] = "Please select a Project!";
                return RedirectToAction("ProjectList", "Project_Keyword");

            }
            SqlConnection con = new SqlConnection(KeywordConnString);
            SqlCommand cmd = new SqlCommand("proc_ProjectCitylist", con);
            string ProjectName = Session["ProjectName"].ToString();
            cmd.Parameters.AddWithValue("@ProjectName", ProjectName);
            cmd.CommandType = CommandType.StoredProcedure;
            SqlDataAdapter dataAdapter = new SqlDataAdapter(cmd);
            DataTable dataTable = new DataTable();
            dataAdapter.Fill(dataTable);
            List<ProjectCity> projectCitylist = new List<ProjectCity>();
            for (int j = 0; j < dataTable.Rows.Count; j++)
            {

                ProjectCity projectCity = new ProjectCity();
                projectCity.Project_city_uid = Convert.ToInt32(dataTable.Rows[j]["project_city_uid"]);

                projectCity.ProjectName = dataTable.Rows[j]["ProjectName"].ToString();
                projectCity.City = dataTable.Rows[j]["CityName"].ToString();


                projectCitylist.Add(projectCity);

            }
            return View(projectCitylist);
        }

        [HttpGet]
        public ActionResult CreateProjectCitytName()
        {
            ProjectCity projectCity = new ProjectCity();

            projectCity.CityNameList = ListCityName();
            return View(projectCity);
        }
        [HttpPost]
        public ActionResult CreateProjectCitytName(ProjectCity projectCity)
        {
            if (!ModelState.IsValid)
                return View(projectCity);

            string Cities = string.Empty;
            if (projectCity.CityName != null)
            {
                Cities = ConvertStringArrayToString(projectCity.CityName);
            }
            
            SqlConnection con = new SqlConnection(KeywordConnString);
            SqlCommand cmd = new SqlCommand("proc_CreateProjectCity", con);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@ProjectName", projectCity.ProjectName);
            cmd.Parameters.AddWithValue("@CityName", Cities);

            con.Open();
            cmd.ExecuteNonQuery();
            con.Close();
            return RedirectToAction("ProjectCityList");
        }

        [HttpGet]
        public ActionResult UpdateProjectCityName(int id)
        {
            SqlConnection con = new SqlConnection(KeywordConnString);
            SqlCommand cmd = new SqlCommand("proc_EditProjectCity", con);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@project_city_uid", id);
            SqlDataAdapter dataAdapter = new SqlDataAdapter(cmd);
            DataTable dataTable = new DataTable();
            dataAdapter.Fill(dataTable);
            ProjectCity projectCity = new ProjectCity();
            projectCity.ProjectName = dataTable.Rows[0]["ProjectName"].ToString();
            projectCity.Project_city_uid = Convert.ToInt32(dataTable.Rows[0]["Project_city_uid"]);
            if (!string.IsNullOrEmpty(dataTable.Rows[0]["Cityname"].ToString()))
            {
                projectCity.CityNameList = ListCityName();
                string City = dataTable.Rows[0]["Cityname"].ToString();
                projectCity.CityName = City.Split(',');
                for (int i = 0; i < projectCity.CityName.Length; i++)
                {

                    foreach (var item in projectCity.CityNameList)
                    {
                        if (projectCity.CityName[i] == item.Text.ToString())
                        {
                            item.Selected = true;
                        }
                    }
                }
            }
            return View(projectCity);
        }
        [HttpPost]
        public ActionResult UpdateProjectCityName(ProjectCity projectCity)
        {
            if (!ModelState.IsValid)
                return View(projectCity);

            string Cities = string.Empty;
            if (projectCity.CityName != null)
            {
                Cities = ConvertStringArrayToString(projectCity.CityName);
            }

            SqlConnection con = new SqlConnection(KeywordConnString);
            SqlCommand cmd = new SqlCommand("proc_UpdateProjectCity", con);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@ProjectName", projectCity.ProjectName);
            cmd.Parameters.AddWithValue("@CityName", Cities);
            cmd.Parameters.AddWithValue("@project_city_uid", projectCity.Project_city_uid);

            con.Open();
            cmd.ExecuteNonQuery();
            con.Close();
            return RedirectToAction("ProjectCityList");
        }


        public static List<SelectListItem> ListCityName()
        {

            SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["KeywordConnection"].ConnectionString);

            List<SelectListItem> CityList = new List<SelectListItem>();
            {
                string query = "select CityName from tblCities group by CityName ";
                SqlCommand cmd = new SqlCommand(query, con);
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                da.Fill(dt);
                for (int i = 0; i < dt.Rows.Count; i++)
                {

                    CityList.Add(new SelectListItem
                    {

                        Text = dt.Rows[i]["CityName"].ToString(),
                        Value = dt.Rows[i]["CityName"].ToString()
                    });
                }
            }

            return CityList;

        }
        public string ConvertStringArrayToString(string[] array)
        {
            // Concatenate all the elements into a StringBuilder.
            StringBuilder builder = new StringBuilder();
            foreach (string value in array)
            {
                builder.Append(value);
                builder.Append(',');
            }
            return builder.ToString().TrimEnd(',');
        }

        public ActionResult DeleteProjectCities(string Id)
        {
            try
            {
                SqlConnection con = new SqlConnection(KeywordConnString);
                SqlCommand cmd = new SqlCommand("proc_deletetblProjectCities", con);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@project_city_uid", Id);
                con.Open();
                cmd.ExecuteNonQuery();
                con.Close();

                return RedirectToAction("ProjectCityList");

            }
            catch (Exception ex)

            {
                return View(ex);

            }
        }
        public ActionResult ProjectKeywordListbycity(string CityName, string ProjectName)
        {
            try
            {
                    ViewBag.CityName = CityName.TrimEnd();
                    ViewBag.ProjectName = ProjectName.TrimEnd();
                    SqlConnection con = new SqlConnection(KeywordConnString);
                    SqlCommand cmd = new SqlCommand("proc_Keywordlistbycity", con);
                    cmd.CommandType = CommandType.StoredProcedure;                   
                    cmd.Parameters.AddWithValue("@City", CityName);
                    cmd.Parameters.AddWithValue("@ProjectName", ProjectName);
                    SqlDataAdapter dataAdapter = new SqlDataAdapter(cmd);
                    DataTable dataTable_Keywords = new DataTable();
                    dataAdapter.Fill(dataTable_Keywords);
                    List<ProjectKeyword> KeywordList = new List<ProjectKeyword>();
                    for (int j = 0; j < dataTable_Keywords.Rows.Count; j++)
                    //for (int j = 0; j < arrayValues.Length; j++)

                    {
                        ProjectKeyword keyword = new ProjectKeyword();
                        keyword.KeywordName = dataTable_Keywords.Rows[j]["KeywordName"].ToString();
                        keyword.ProjectName = dataTable_Keywords.Rows[j]["ProjectName"].ToString();
                        keyword.City = dataTable_Keywords.Rows[j]["City"].ToString();

                        KeywordList.Add(keyword);

                    }
                return View(KeywordList);
               
            }
            catch (Exception ex)
            {
                return View(ex);
            }
        }

        public ActionResult ProjectCityListbyProject(string ProjectName)
        {
            if (Session["ProjectName"] != null)
                ViewBag.ProjectName = Session["ProjectName"];
            else
                ViewBag.ProjectName = ProjectName;

            SqlConnection con = new SqlConnection(KeywordConnString);
            SqlCommand cmd = new SqlCommand("proc_ProjectCitylistbyProjectName", con);
            if (Session["ProjectName"] != null)
                cmd.Parameters.AddWithValue("@ProjectName", Session["ProjectName"]);
            else
                cmd.Parameters.AddWithValue("@ProjectName", ProjectName);

            cmd.CommandType = CommandType.StoredProcedure;
            SqlDataAdapter dataAdapter = new SqlDataAdapter(cmd);
            DataTable dataTable = new DataTable();
            dataAdapter.Fill(dataTable);
            List<ProjectCity> projectCitylist = new List<ProjectCity>();
            if (dataTable.Rows.Count > 0)
            {
                for (int i = 0; i < dataTable.Rows.Count; i++)
                {


                    ProjectCity projectCity = new ProjectCity();
                    //projectCity.Project_city_uid = Convert.ToInt32(dataTable.Rows[j]["project_city_uid"]);

                    projectCity.ProjectName = dataTable.Rows[0]["ProjectName"].ToString();
                    projectCity.City = dataTable.Rows[i]["cityname"].ToString();


                    projectCitylist.Add(projectCity);

                }
            }
            return View(projectCitylist);
        }
        [HttpGet]
        public ActionResult CreateKeywordbyCity(string Project, string CityName)
        {
            ProjectKeyword projectKeyword = new ProjectKeyword();
            projectKeyword.ProjectName = Project;
            projectKeyword.City = CityName;
            ViewBag.ProjectName = Project;
            ViewBag.CityName = CityName;

            return View(projectKeyword);
        }
        [HttpPost]
        public ActionResult CreateKeywordbyCity(ProjectKeyword projectKeyword)
        {
            if (!ModelState.IsValid)
                return View(projectKeyword);
            string Project = projectKeyword.ProjectName;
            string CityName = projectKeyword.City;
            if (projectKeyword.KeywordName == null)
            {
             return RedirectToAction("ProjectKeywordListbycity", new { CityName = CityName, ProjectName = Project });
            }
            

            //Query query = new Query();
            //query.query = projectKeyword.KeywordName;
            //var json1 = new JavaScriptSerializer().Serialize(query);
            //byte[] dataBytes1 = Encoding.UTF8.GetBytes(json1);
            //string uri1 = "http://keywordpos.zunamelt.com/api/keyword-idea/";
            //HttpWebRequest request1 = (HttpWebRequest)WebRequest.Create(uri1);
            //request1.AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate;
            //request1.ContentLength = dataBytes1.Length;
            //request1.ContentType = "application/json";
            //request1.Method = "POST";
            //request1.Headers["Authorization"] = "token " + Session["token"];
            //using (Stream requestBody1 = request1.GetRequestStream())
            //{
            //    requestBody1.Write(dataBytes1, 0, dataBytes1.Length);
            //}
            //string fLocation = string.Empty;
            //using (HttpWebResponse response1 = (HttpWebResponse)request1.GetResponse())
            //using (Stream stream1 = response1.GetResponseStream())
            //using (StreamReader reader1 = new StreamReader(stream1))
            //{
            //    var objText = reader1.ReadToEnd();
            //    fLocation = objText.Replace("\"data\":", "");
            //    fLocation = fLocation.Substring(1);
            //    fLocation = fLocation.Remove(fLocation.Length - 1);
            //    //var modal1 = JsonConvert.DeserializeObject<List<Example>>(fLocation);

            //}

            //DataTable dt_Keyword = (DataTable)JsonConvert.DeserializeObject(fLocation, (typeof(DataTable)));

            //dt_Keyword.Columns.Add("ProjectName", typeof(String));
            //dt_Keyword.Columns.Add("Mainkeyword", typeof(String));
            ////dt_Keyword.Columns.Add("City", typeof(String));

            //var rowsToUpdate = dt_Keyword.AsEnumerable();
            //foreach (var row in rowsToUpdate)
            //{
            //    row.SetField("ProjectName", Project);
            //    row.SetField("Mainkeyword", projectKeyword.KeywordName);
            //   // row.SetField("City", CityName);

            //}

            //using (SqlBulkCopy bulkCopy =
            // new SqlBulkCopy(KeywordConnString, SqlBulkCopyOptions.TableLock))
            //{
            //    bulkCopy.DestinationTableName = "Tbl" + Project.Replace(" ", string.Empty);
            //    foreach (DataColumn column in dt_Keyword.Columns)
            //    {
            //        if (column.ColumnName != "monthly_search_volume")
            //            bulkCopy.ColumnMappings.Add(column.ColumnName, column.ColumnName);
            //    }
            //    bulkCopy.WriteToServer(dt_Keyword);
            //}

            SqlConnection con = new SqlConnection(KeywordConnString);
            SqlCommand cmd = new SqlCommand("proc_CreateKeywordbyCity", con);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@ProjectName", Project);
            cmd.Parameters.AddWithValue("@KeywordName", projectKeyword.KeywordName);
            cmd.Parameters.AddWithValue("@City", CityName);

            con.Open();
            cmd.ExecuteNonQuery();
            con.Close();

            TempData["Keyword"] = "Keyword has been Inserted, Do you want to create one more? or click save to exit";
            return Redirect(Request.UrlReferrer.ToString());
            
            
            //return View();
            //}
        }
    }
}