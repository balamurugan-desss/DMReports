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
    public class Project_KeywordController : Controller
    {
        private string KeywordConnString = ConfigurationManager.ConnectionStrings["KeywordConnection"].ConnectionString;
        // GET: Project_Keyword
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult ProjectList()
        {
            Session["ProjectName"] = null;
            SqlConnection con = new SqlConnection(KeywordConnString);
            SqlCommand cmd = new SqlCommand("proc_ProjectNamelist", con);
            cmd.CommandType = CommandType.StoredProcedure;
            SqlDataAdapter dataAdapter = new SqlDataAdapter(cmd);
            DataTable dataTable = new DataTable();
            dataAdapter.Fill(dataTable);
            List<Project> ProjectList = new List<Project>();
            for (int j = 0; j < dataTable.Rows.Count; j++)
            {
                Project project = new Project();
                project.ProjectName = dataTable.Rows[j]["ProjectName"].ToString();
                ProjectList.Add(project);

            }
            return View(ProjectList);
        }


        [HttpGet]
        public ActionResult CreateProjectName()
        {
            Project project = new Project();

            return View(project);
        }
        [HttpPost]
        public ActionResult CreateProjectName(Project project)
        {
            if (!ModelState.IsValid)
                return View(project);

            SqlConnection con = new SqlConnection(KeywordConnString);
            SqlCommand cmd = new SqlCommand("proc_CreateProject", con);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@ProjectName", project.ProjectName);
            cmd.Parameters.AddWithValue("@google_Organic", project.google_Organic);
            cmd.Parameters.AddWithValue("@Google_organic_cites", project.Google_organic_cites);
            cmd.Parameters.AddWithValue("@Google_my_business_cities", project.Google_my_business_cities);


            con.Open();
            cmd.ExecuteNonQuery();
            con.Close();

            //SqlConnection con1 = new SqlConnection(KeywordConnString);
            //SqlCommand cmd1 = new SqlCommand("Proc_GK_DynamicTbl", con1);
            //cmd1.CommandType = CommandType.StoredProcedure;
            //cmd1.Parameters.AddWithValue("@TableName", "Tbl" + project.ProjectName.Replace(" ", string.Empty));
            //cmd1.Parameters.AddWithValue("@ColumnName", "ProjectName");
            //cmd1.Parameters.AddWithValue("@ColumnDataType", "varchar(max)");
            //cmd1.Parameters.AddWithValue("@ColumnName1", "Mainkeyword");
            //cmd1.Parameters.AddWithValue("@ColumnDataType1", "varchar(max)");
            //cmd1.Parameters.AddWithValue("@ColumnName2", "keyword");
            //cmd1.Parameters.AddWithValue("@ColumnDataType2", "varchar(max)");
            //cmd1.Parameters.AddWithValue("@ColumnName3", "competition");
            //cmd1.Parameters.AddWithValue("@ColumnDataType3", "varchar(max)");
            //cmd1.Parameters.AddWithValue("@ColumnName4", "search_volume");
            //cmd1.Parameters.AddWithValue("@ColumnDataType4", "int");
            //cmd1.Parameters.AddWithValue("@ColumnName5", "monthly_search_volume");
            //cmd1.Parameters.AddWithValue("@ColumnDataType5", "varchar(max)");
            //cmd1.Parameters.AddWithValue("@id", "Id");
            //cmd1.Parameters.AddWithValue("@idDatatype", "int identity");

            //con1.Open();
            //cmd1.ExecuteNonQuery();
            //con1.Close();
            return RedirectToAction("AssignProjectCity", "Cities", new {ProjectName = project.ProjectName });
        }
        public ActionResult UpdateProjectName(string ProjectName)
        {
            try
            {

                SqlConnection con = new SqlConnection(KeywordConnString);
                SqlCommand cmd = new SqlCommand("proc_GetProject", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@ProjectName", ProjectName);
                SqlDataAdapter dataAdapter = new SqlDataAdapter(cmd);
                DataTable dataTable = new DataTable();
                dataAdapter.Fill(dataTable);
                Project project = new Project();
                if (dataTable.Rows.Count > 0)
                {
                    project.Id = Convert.ToInt32(dataTable.Rows[0]["Id"].ToString());
                    project.ProjectName = dataTable.Rows[0]["ProjectName"].ToString();
                    project.google_Organic = Convert.ToBoolean(dataTable.Rows[0]["google_Organic"].ToString());
                    project.Google_organic_cites = Convert.ToBoolean(dataTable.Rows[0]["Google_organic_cites"].ToString());
                    project.Google_my_business_cities = Convert.ToBoolean(dataTable.Rows[0]["Google_my_business_cities"].ToString());

                }
                return View(project);
            }
            catch(Exception ex)
            {
                return View(ex);
            }
        }
        [HttpPost]
        public ActionResult UpdateProjectName(Project project)
        {
            try
            {
                if (!ModelState.IsValid)
                    return View(project);

                SqlConnection con = new SqlConnection(KeywordConnString);
                SqlCommand cmd = new SqlCommand("proc_UpdateProject", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@ProjectName", project.ProjectName);
                cmd.Parameters.AddWithValue("@google_Organic", project.google_Organic);
                cmd.Parameters.AddWithValue("@Google_organic_cites", project.Google_organic_cites);
                cmd.Parameters.AddWithValue("@Google_my_business_cities", project.Google_my_business_cities);


                con.Open();
                cmd.ExecuteNonQuery();
                con.Close();
                return RedirectToAction("ProjectList");
            }
            catch (Exception ex)
            {
                return View(ex);
            }
        }
        
        public ActionResult SelectedProjectList(string ProjectName)
        {

            ViewBag.ProjectName = ProjectName.Trim();
            SqlConnection con = new SqlConnection(KeywordConnString);
            SqlCommand cmd = new SqlCommand("proc_ProjectNamelistSelect", con);
            cmd.Parameters.AddWithValue("@ProjectName", ProjectName);
            cmd.CommandType = CommandType.StoredProcedure;
            SqlDataAdapter dataAdapter = new SqlDataAdapter(cmd);
            DataTable dataTable = new DataTable();
            dataAdapter.Fill(dataTable);
            List<Project> ProjectList = new List<Project>();
            for (int j = 0; j < dataTable.Rows.Count; j++)
            {
                Project project = new Project();
                project.ProjectName = dataTable.Rows[j]["ProjectName"].ToString();
                Session["ProjectName"] = project.ProjectName.ToString().Trim();
                if (string.IsNullOrEmpty(dataTable.Rows[j]["google_Organic"].ToString()))
                    project.google_Organic = Convert.ToBoolean("false");
                else
                    project.google_Organic = Convert.ToBoolean(dataTable.Rows[j]["google_Organic"]);
                if (string.IsNullOrEmpty(dataTable.Rows[j]["Google_organic_cites"].ToString()))
                    project.Google_organic_cites = Convert.ToBoolean("false");
                else
                    project.Google_organic_cites = Convert.ToBoolean(dataTable.Rows[j]["Google_organic_cites"]);
                if (string.IsNullOrEmpty(dataTable.Rows[j]["Google_my_business_cities"].ToString()))
                    project.Google_my_business_cities = Convert.ToBoolean("false");
                else
                    project.Google_my_business_cities = Convert.ToBoolean(dataTable.Rows[j]["Google_my_business_cities"]);

                ProjectList.Add(project);

            }
            return View(ProjectList);
        }

        public ActionResult ProjectKeywordList(string Project)
        {
            ViewBag.Project = Project.TrimEnd();
            SqlConnection con = new SqlConnection(KeywordConnString);
            SqlCommand cmd = new SqlCommand("proc_Keywordlist", con);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@ProjectName", Project);
            SqlDataAdapter dataAdapter = new SqlDataAdapter(cmd);
            DataTable dataTable_Keywords = new DataTable();
            dataAdapter.Fill(dataTable_Keywords);
            List<ProjectKeyword> KeywordList = new List<ProjectKeyword>();
            for (int j = 0; j < dataTable_Keywords.Rows.Count; j++)
            {
                ProjectKeyword keyword = new ProjectKeyword();
                keyword.KeywordName = dataTable_Keywords.Rows[j]["KeywordName"].ToString();
                keyword.ProjectName = dataTable_Keywords.Rows[j]["ProjectName"].ToString();
                KeywordList.Add(keyword);

            }
            ViewBag.ProjectName = Project;
            return View(KeywordList);
        }


        [HttpGet]
        public ActionResult CreateKeyword( string Project)
        {
            ProjectKeyword projectKeyword = new ProjectKeyword();
            projectKeyword.ProjectName = Project;
            ViewBag.Project = Project;
            return View(projectKeyword);
        }
        [HttpPost]
        public ActionResult CreateKeyword(ProjectKeyword projectKeyword)
        {
            if (!ModelState.IsValid)
                return View(projectKeyword);

            string Project = projectKeyword.ProjectName;

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

            //var rowsToUpdate = dt_Keyword.AsEnumerable();
            //foreach (var row in rowsToUpdate)
            //{
            //    row.SetField("ProjectName", Project);
            //    row.SetField("Mainkeyword", projectKeyword.KeywordName);
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
            SqlCommand cmd = new SqlCommand("proc_CreateKeyword", con);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@ProjectName", Project);
            cmd.Parameters.AddWithValue("@KeywordName", projectKeyword.KeywordName);
            con.Open();
            cmd.ExecuteNonQuery();
            con.Close();
            return RedirectToAction("ProjectKeywordList",new {Project = Project });
        }

        public ActionResult SuggesionKeywords(string Keyword , string Project)
        {
            Query query = new Query();
            query.query = Keyword;
            var json1 = new JavaScriptSerializer().Serialize(query);
            byte[] dataBytes1 = Encoding.UTF8.GetBytes(json1);
            string uri1 = "http://keywordpos.zunamelt.com/api/keyword-idea/";
            HttpWebRequest request1 = (HttpWebRequest)WebRequest.Create(uri1);
            request1.AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate;
            request1.ContentLength = dataBytes1.Length;
            request1.ContentType = "application/json";
            request1.Method = "POST";
            request1.Headers["Authorization"] = "token " + Session["token"];
            using (Stream requestBody1 = request1.GetRequestStream())
            {
                requestBody1.Write(dataBytes1, 0, dataBytes1.Length);
            }
            string fLocation = string.Empty;
            using (HttpWebResponse response1 = (HttpWebResponse)request1.GetResponse())
            using (Stream stream1 = response1.GetResponseStream())
            using (StreamReader reader1 = new StreamReader(stream1))
            {
                var objText = reader1.ReadToEnd();
                fLocation = objText.Replace("\"data\":", "");
                fLocation = fLocation.Substring(1);
                fLocation = fLocation.Remove(fLocation.Length - 1);
                //var modal1 = JsonConvert.DeserializeObject<List<Example>>(fLocation);

            }
            ViewBag.Project = Project;
            DataTable dt_Keyword = (DataTable)JsonConvert.DeserializeObject(fLocation, (typeof(DataTable)));
            DataColumnCollection columns = dt_Keyword.Columns;
            if (!columns.Contains("keyword"))
            {
                dt_Keyword.Columns.Add("Keyword", typeof(System.String));
                dt_Keyword.Columns.Add("Competition", typeof(System.String));
                dt_Keyword.Columns.Add("Search Volume", typeof(System.String));
                dt_Keyword.Columns.Add("Monthly Search Volume", typeof(System.String));
                dt_Keyword.AcceptChanges();
            }
            else
            {
                dt_Keyword.Columns["keyword"].ColumnName = "Keyword";
                dt_Keyword.Columns["competition"].ColumnName = "Competition";
                dt_Keyword.Columns["search_volume"].ColumnName = "Search Volume";
                dt_Keyword.Columns["monthly_search_volume"].ColumnName = "Monthly Search Volume";
                dt_Keyword.AcceptChanges();
            }
            return View(dt_Keyword);
        }

        public ActionResult DeleteProjectKeyWord(string Id)
        {
            try
            {
                SqlConnection con = new SqlConnection(KeywordConnString);
                SqlCommand cmd = new SqlCommand("proc_deletetbl_SavedKeywords", con);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@Id", Id);
                con.Open();
                cmd.ExecuteNonQuery();
                con.Close();

                return RedirectToAction("ProjectKeywordList");

            }
            catch (Exception ex)

            {
                return View(ex);

            }
        }


        //==============================================================================//
        public ActionResult KeywordListbyProject(string Project)
        {
            ViewBag.Project = Project.TrimEnd();
            SqlConnection con = new SqlConnection(KeywordConnString);
            SqlCommand cmd = new SqlCommand("proc_listKeywordsbyProject", con);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@ProjectName", Project);
            SqlDataAdapter dataAdapter = new SqlDataAdapter(cmd);
            DataTable dataTable_Keywords = new DataTable();
            dataAdapter.Fill(dataTable_Keywords);
            List<ProjectKeyword> KeywordList = new List<ProjectKeyword>();
            for (int j = 0; j < dataTable_Keywords.Rows.Count; j++)
            {
                ProjectKeyword keyword = new ProjectKeyword();
                keyword.KeywordName = dataTable_Keywords.Rows[j]["ProjectKeyword"].ToString();
                keyword.ProjectName = dataTable_Keywords.Rows[j]["ProjectName"].ToString();
                KeywordList.Add(keyword);

            }
            ViewBag.ProjectName = Project;
            return View(KeywordList);
        }

        [HttpGet]
        public ActionResult CreateKeywordbyProject(string Project, string CityName)
        {
            ProjectKeyword projectKeyword = new ProjectKeyword();
            projectKeyword.ProjectName = Project;
            ViewBag.ProjectName = Project;

            return View(projectKeyword);
        }
        [HttpPost]
        public ActionResult CreateKeywordbyProject(ProjectKeyword projectKeyword)
        {
            if (!ModelState.IsValid)
                return View(projectKeyword);
            string Project = projectKeyword.ProjectName;
            if (projectKeyword.KeywordName == null)
            {
                return RedirectToAction("KeywordListbyProject", new { Project = Project });
            }
            SqlConnection con = new SqlConnection(KeywordConnString);
            SqlCommand cmd = new SqlCommand("proc_InsertKeywordsbyProject", con);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@ProjectName", Project);
            cmd.Parameters.AddWithValue("@ProjectKeyword", projectKeyword.KeywordName);

            con.Open();
            cmd.ExecuteNonQuery();
            con.Close();

            TempData["Keyword"] = "Keyword has been Inserted, Do you want to create one more? or click save to exit";

            return Redirect(Request.UrlReferrer.ToString());


            //return View();
            //}
        }
        public ActionResult CitybyKeyword(string ProjectName, string Keyword)
        {
            try
            {

                ViewBag.ProjectName = ProjectName.TrimEnd();
                ViewBag.Keyword = Keyword.Trim();
                SqlConnection con = new SqlConnection(KeywordConnString);
                SqlCommand cmd = new SqlCommand("proc_listTblKeywordsCities", con);
                cmd.Parameters.AddWithValue("@ProjectName", ProjectName);
                cmd.Parameters.AddWithValue("@ProjectKeyword", Keyword);
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
                        projectCity.KeywordName = dataTable.Rows[0]["ProjectKeyword"].ToString();
                        projectCity.City = dataTable.Rows[i]["ProjectKeycities"].ToString();

                        //projectCity.CityName = dataTable.Rows[0]["ProjectKeyCities"].ToString();


                        projectCitylist.Add(projectCity);


                    }
                }
                return View(projectCitylist);
            }
            catch (Exception ex)
            {
                return View(ex);
            }
        }
        public ActionResult AssignKeywordCities(string ProjectName, string Keyword)
        {
            try
            {
                ViewBag.ProjectName = ProjectName.TrimEnd();
                ViewBag.Keyword = Keyword.TrimEnd();

                TempData["Message"] = string.Empty;
                SqlConnection con = new SqlConnection(KeywordConnString);
                SqlCommand cmd = new SqlCommand("proc_ProjectCitylistbyProjectName", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@ProjectName", ProjectName);
                SqlDataAdapter dataAdapter = new SqlDataAdapter(cmd);
                DataTable dataTable = new DataTable();
                dataAdapter.Fill(dataTable);
                if (ProjectName != null)
                {
                    SqlCommand cmd1 = new SqlCommand("proc_listTblKeywordsCities", con);
                    cmd1.CommandType = CommandType.StoredProcedure;
                    cmd1.Parameters.AddWithValue("@ProjectName", ProjectName);
                    cmd1.Parameters.AddWithValue("@ProjectKeyword", Keyword);
                    SqlDataAdapter dataAdapter1 = new SqlDataAdapter(cmd1);
                    DataTable dataTable1 = new DataTable();
                    dataAdapter1.Fill(dataTable1);
                    if (dataTable1.Rows.Count > 0)
                    {
                        string cityname = string.Empty;
                        for (int i = 0; i < dataTable1.Rows.Count; i++)
                        {
                            cityname += dataTable1.Rows[i]["ProjectKeyCities"].ToString() + ",";

                        }
                        cityname = cityname.TrimStart(',');
                        TempData["Message"] = string.Join(",", cityname);
                    }
                }
                List<ProjectCity> Citylist = new List<ProjectCity>();
                if (dataTable.Rows.Count > 0)
                {
                    for (int i = 0; i < dataTable.Rows.Count; i++)
                    {
                        ProjectCity projectCity = new ProjectCity();
                        projectCity.ProjectName = dataTable.Rows[i]["ProjectName"].ToString();
                        projectCity.City = dataTable.Rows[i]["CityName"].ToString();
                        Citylist.Add(projectCity);
                    }
                }

                return View(Citylist);
            }
            catch (Exception ex)
            {
                return View(ex);
            }
        }
        [HttpPost]
        public ActionResult AssignKeywordCities(string[] chkrow, string ProjectName, string Keyword)
        {
            try
            {
                TempData["Message"] = string.Empty;
                //ProjectCity projectCity = new ProjectCity();
                ProjectName = HttpUtility.ParseQueryString(Request.UrlReferrer.Query)["ProjectName"];
                Keyword = HttpUtility.ParseQueryString(Request.UrlReferrer.Query)["Keyword"];

                string Cities = string.Join(",", chkrow);
                if (chkrow != null)
                {
                    TempData["Message"] = string.Join(",", chkrow);
                }


                SqlConnection con = new SqlConnection(KeywordConnString);

                SqlCommand del = new SqlCommand("proc_deleteKeywordsCities", con);
                del.CommandType = CommandType.StoredProcedure;
                del.Parameters.AddWithValue("@projectname", ProjectName);
                del.Parameters.AddWithValue("@ProjectKeyword", Keyword);


                con.Open();
                del.ExecuteNonQuery();
                con.Close();

                string[] cityname = Cities.ToString().Split(',');
                for (int i = 0; i < cityname.Length; i++)
                {
                    SqlCommand cmd = new SqlCommand("proc_InsertKeywordcities", con);
                    cmd.CommandType = CommandType.StoredProcedure;


                    cmd.Parameters.AddWithValue("@ProjectName", ProjectName.ToString());
                    cmd.Parameters.AddWithValue("@ProjectKeyword", Keyword.ToString());
                    cmd.Parameters.AddWithValue("@ProjectKeyCities", cityname[i]);

                    con.Open();
                    cmd.ExecuteNonQuery();

                    con.Close();
                }
                return RedirectToAction("CitybyKeyword", new { ProjectName = ProjectName, Keyword = Keyword });


            }
            catch (Exception ex)
            {
                return View(ex);
            }
        }
    }
}