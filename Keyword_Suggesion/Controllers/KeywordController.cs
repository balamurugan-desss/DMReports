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
    public class KeywordController : Controller
    {
        private string KeywordConnString = ConfigurationManager.ConnectionStrings["KeywordConnection"].ConnectionString;
        public ActionResult KeywordS()
        {
            return View();
        }
        // GET: Keyword
        [HttpGet]
        public ActionResult KeywordSearch()
        {
            Keywords Keyword = new Keywords();
            return View(Keyword);
        }   
        [HttpPost]
        public ActionResult KeywordSearch(Keywords Keyword)
        {
            if (!ModelState.IsValid)
                return View(Keyword);
            //GetKeyword(Keyword.CreateProjectName, Keyword.SelectProjectName, Keyword.keywords);
            ViewData["Message"] = "Keywords are Saved in Project " + Keyword.CreateProjectName;
            return View();
            //return RedirectToAction("GetKeyword", new {CreatprojectName = Keyword.CreateProjectName, SelectProjectName = Keyword.SelectProjectName, Keyword = Keyword.keywords, IsSelect = Keyword.Select });
        }


        [HttpPost]
        public ActionResult GetKeyword(Keywords Keyword)
        {
            string key = string.Empty;
            if (Keyword.keywordList != null)
                key = Keyword.keywordList;
            else
                key = Keyword.keywords;

            if (Keyword.IsNew == null || key == null)
            {
                return RedirectToAction("KeywordS");
            }
            string Project = Keyword.SelectProjectName;
            string OffKeyword = Keyword.keywordList;
            string OnKeyword = Keyword.keywords;
            //------------- Create table ----------------
            if (Project != null && OffKeyword != null)
            {
                SqlConnection con = new SqlConnection(KeywordConnString);
                SqlCommand cmd = new SqlCommand("SelectKeywords", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Tablename", "tbl" + Project.Replace(" ", string.Empty));
                cmd.Parameters.AddWithValue("@Keyword", OffKeyword);
                SqlDataAdapter dataAdapter = new SqlDataAdapter(cmd);
                DataTable dataTable_Keywords = new DataTable();
                dataAdapter.Fill(dataTable_Keywords);

                return View(dataTable_Keywords);
            }
            Query query = new Query();
            query.query = OnKeyword;
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
                var modal1 = JsonConvert.DeserializeObject<List<Example>>(fLocation);

            }

            DataTable dt_Keyword = (DataTable)JsonConvert.DeserializeObject(fLocation, (typeof(DataTable)));


            return View(dt_Keyword);
        }
        [HttpPost]
        public string SaveKeyword(string IsNew, string CreateProject, string SelectProject, string keyword)
        {

            Query query = new Query();
            query.query = keyword;
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
                var modal1 = JsonConvert.DeserializeObject<List<Example>>(fLocation);

            }

            DataTable dt_Keyword = (DataTable)JsonConvert.DeserializeObject(fLocation, (typeof(DataTable)));

            dt_Keyword.Columns.Add("ProjectName", typeof(String));
            dt_Keyword.Columns.Add("Mainkeyword", typeof(String));
            var rowsToUpdate = dt_Keyword.AsEnumerable();
            string projectname = string.Empty;
            if (CreateProject != "")
                projectname = CreateProject;
            else
                projectname = SelectProject;

            foreach (var row in rowsToUpdate)
            {
                row.SetField("ProjectName", projectname);
                row.SetField("Mainkeyword", keyword);
            }
            SqlConnection connection = new SqlConnection(KeywordConnString);
            SqlCommand command = new SqlCommand("InsertTbl_SavedKeywords", connection);
            command.CommandType = CommandType.StoredProcedure;
            command.Parameters.AddWithValue("@ProjectName", projectname);
            command.Parameters.AddWithValue("@KeyWordName", keyword);
            connection.Open();
            command.ExecuteNonQuery();
            connection.Close();
            if (IsNew == "1")
            {
                SqlConnection con = new SqlConnection(KeywordConnString);
                SqlCommand cmd = new SqlCommand("InsertTbl_Dynamic_ProjectName", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@ProjectName", CreateProject);
                con.Open();
                cmd.ExecuteNonQuery();
                con.Close();

                SqlConnection con1 = new SqlConnection(KeywordConnString);
                SqlCommand cmd1 = new SqlCommand("Proc_GK_DynamicTbl", con1);
                cmd1.CommandType = CommandType.StoredProcedure;
                cmd1.Parameters.AddWithValue("@TableName", "Tbl" + CreateProject.Replace(" ", string.Empty));
                cmd1.Parameters.AddWithValue("@ColumnName", "ProjectName");
                cmd1.Parameters.AddWithValue("@ColumnDataType", "varchar(max)");
                cmd1.Parameters.AddWithValue("@ColumnName1", "Mainkeyword");
                cmd1.Parameters.AddWithValue("@ColumnDataType1", "varchar(max)");
                cmd1.Parameters.AddWithValue("@ColumnName2", "keyword");
                cmd1.Parameters.AddWithValue("@ColumnDataType2", "varchar(max)");
                cmd1.Parameters.AddWithValue("@ColumnName3", "competition");
                cmd1.Parameters.AddWithValue("@ColumnDataType3", "varchar(max)");
                cmd1.Parameters.AddWithValue("@ColumnName4", "search_volume");
                cmd1.Parameters.AddWithValue("@ColumnDataType4", "int");
                cmd1.Parameters.AddWithValue("@ColumnName5", "monthly_search_volume");
                cmd1.Parameters.AddWithValue("@ColumnDataType5", "varchar(max)");
                cmd1.Parameters.AddWithValue("@id", "Id");
                cmd1.Parameters.AddWithValue("@idDatatype", "int identity");

                con1.Open();
                cmd1.ExecuteNonQuery();
                con1.Close();

             


                //------- Insert Get Keyword Json data -------------//

                using (SqlBulkCopy bulkCopy =
                 new SqlBulkCopy(KeywordConnString, SqlBulkCopyOptions.TableLock))
                {
                    bulkCopy.DestinationTableName = "Tbl" + CreateProject.Replace(" ", string.Empty);
                    foreach (DataColumn column in dt_Keyword.Columns)
                    {
                        if (column.ColumnName != "monthly_search_volume")
                            bulkCopy.ColumnMappings.Add(column.ColumnName, column.ColumnName);
                    }
                    bulkCopy.WriteToServer(dt_Keyword);
                }

                return "Success";

            }
            else
            {
                using (SqlBulkCopy bulkCopy =
                 new SqlBulkCopy(KeywordConnString, SqlBulkCopyOptions.TableLock))
                {
                    bulkCopy.DestinationTableName = "Tbl" + SelectProject.Replace(" ", string.Empty);
                    foreach (DataColumn column in dt_Keyword.Columns)
                    {
                        if (column.ColumnName != "monthly_search_volume")
                            bulkCopy.ColumnMappings.Add(column.ColumnName, column.ColumnName);
                    }
                    bulkCopy.WriteToServer(dt_Keyword);
                }
                return "Success";
            }
        }
        public string CheckProjectName(string ProjectName)
        {
            SqlConnection connection = new SqlConnection(this.KeywordConnString);
            SqlCommand sqlCommand = new SqlCommand("select top 1 ProjectName from Tbl_Dynamic_ProjectName where ProjectName=@ProjectName ", connection);
            sqlCommand.Parameters.AddWithValue("@ProjectName", (object)ProjectName);
            connection.Open();
            if (sqlCommand.ExecuteReader().HasRows)
            {
                connection.Close();
                return "Failure";
            }
            connection.Close();
            return "Success";
        }

        public string CheckKeywordName(string KeywordName, string City, string ProjectName)
        {
            SqlConnection connection = new SqlConnection(this.KeywordConnString);
            SqlCommand sqlCommand = new SqlCommand();
            sqlCommand.Connection = connection;
            if (City != null)
            {
                 sqlCommand.CommandText = "select top 1 keywordname from tbl_SavedKeywords where keywordname = @KeywordName and City=@City and ProjectName = @ProjectName ";
                sqlCommand.Parameters.AddWithValue("@City", (object)City);
            }
            else
            {
                 sqlCommand.CommandText = "select top 1 ProjectKeyword from KeywordbyProject where ProjectKeyword = @KeywordName and ProjectName = @ProjectName ";

            }
            sqlCommand.Parameters.AddWithValue("@keywordname", (object)KeywordName);
            sqlCommand.Parameters.AddWithValue("@ProjectName", (object)ProjectName);

            connection.Open();
            if (sqlCommand.ExecuteReader().HasRows)
            {
                connection.Close();
                return "Failure";
            }
            connection.Close();
            return "Success";
        }

    }
}