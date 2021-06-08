using Keyword_Suggesion.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.OleDb;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Keyword_Suggesion.Controllers
{
    [LoginCheck]

    public class OrganicController : Controller
    {
        private string KeywordConnString = ConfigurationManager.ConnectionStrings["KeywordConnection"].ConnectionString;

        // GET: Organic
        public ActionResult Organiclist()

        {
          
            SqlConnection con = new SqlConnection(KeywordConnString);
            SqlCommand cmd = new SqlCommand("proc_tblKeywordOrganiclist", con);
            //string ProjectName = Session["ProjectName"].ToString();
            //cmd.Parameters.AddWithValue("@ProjectName", ProjectName);
            cmd.CommandType = CommandType.StoredProcedure;
            SqlDataAdapter dataAdapter = new SqlDataAdapter(cmd);
            DataTable dataTable = new DataTable();
            dataAdapter.Fill(dataTable);
            List<ProjectKeywordmaster> organiclist = new List<ProjectKeywordmaster>();
            for (int j = 0; j < dataTable.Rows.Count; j++)
            {
                ProjectKeywordmaster organic = new ProjectKeywordmaster();
                organic.Id = Convert.ToInt32(dataTable.Rows[j]["Id"]);
                organic.ProjectName = dataTable.Rows[j]["Project_Name"].ToString();
                organic.Keyword = dataTable.Rows[j]["Keyword"].ToString();
                organiclist.Add(organic);

            }
            return View(organiclist);
        }

        [HttpGet]
        public ActionResult CreateOrganic()
        {
            ProjectKeywordmaster organic = new ProjectKeywordmaster();
            return View(organic);
        }
        [HttpPost]
        public ActionResult CreateOrganic(ProjectKeywordmaster master)
        {
            try
            {
                if (!ModelState.IsValid)
                    return View(master);

                SqlConnection con = new SqlConnection(KeywordConnString);
                SqlCommand cmd = new SqlCommand("proc_CreatetblKeywordOrganic", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Project_Name", master.ProjectName);
                cmd.Parameters.AddWithValue("@Keyword", master.Keyword);


                con.Open();
                cmd.ExecuteNonQuery();
                con.Close();
                return RedirectToAction("Organiclist");
            }
            catch (Exception ex)
            {
                return View(ex);
            }
        }

        [HttpGet]
        public ActionResult UpdateOrganic(int id)
        {
            SqlConnection con = new SqlConnection(KeywordConnString);
            SqlCommand cmd = new SqlCommand("proc_EditTblKeywordOrganic", con);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@Id", id);
            SqlDataAdapter dataAdapter = new SqlDataAdapter(cmd);
            DataTable dataTable = new DataTable();
            dataAdapter.Fill(dataTable);
            ProjectKeywordmaster organic = new ProjectKeywordmaster();
            if (dataTable.Rows.Count > 0)
            {
                organic.Id = Convert.ToInt32(dataTable.Rows[0]["Id"].ToString());
                organic.ProjectName = dataTable.Rows[0]["Project_Name"].ToString();
                organic.Keyword = dataTable.Rows[0]["Keyword"].ToString();
            }
            return View(organic);
        }
        [HttpPost]
        public ActionResult UpdateOrganic(ProjectKeywordmaster organic)
        {
            try
            {
                if (!ModelState.IsValid)
                    return View(organic);
                SqlConnection connection = new SqlConnection(KeywordConnString);
                SqlCommand sqlCommand = new SqlCommand("proc_UpdateTblKeywordOrganic", connection);
                sqlCommand.CommandType = CommandType.StoredProcedure;

                sqlCommand.Parameters.AddWithValue("@Id", Convert.ToInt32(organic.Id));
                if (organic.ProjectName != null)
                    sqlCommand.Parameters.AddWithValue("@Project_Name", organic.ProjectName);
                else
                    organic.ProjectName = string.Empty;
                if (organic.Keyword != null)
                    sqlCommand.Parameters.AddWithValue("@Keyword", organic.Keyword);
                else
                    organic.Keyword = string.Empty;
                
                connection.Open();
                sqlCommand.ExecuteNonQuery();
                connection.Close();
                return RedirectToAction("Organiclist");
            }
            catch (Exception ex)
            {
                return View(ex);
            }
        }

        public ActionResult DeleteOrganic(int Id)
        {
            try
            {
                SqlConnection con = new SqlConnection(KeywordConnString);
                SqlCommand cmd = new SqlCommand("proc_deleteTblKeywordOrganic", con);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@Id", Id);
                con.Open();
                cmd.ExecuteNonQuery();
                con.Close();

                return RedirectToAction("Organiclist");

            }
            catch (Exception ex)

            {
                return View(ex);

            }
        }

        public ActionResult ImportFromExcel(HttpPostedFileBase postedFile)
        {
            try
            {

                if (postedFile != null)
                {
                    string filePath = string.Empty;
                    string path = Server.MapPath("~/Uploads/");
                    if (!Directory.Exists(path))
                    {
                        Directory.CreateDirectory(path);
                    }
                    var FileName = Path.ChangeExtension(postedFile.FileName, ".csv");
                    filePath = path + Path.GetFileName(FileName);
                    string extension = Path.GetExtension(FileName);
                    postedFile.SaveAs(filePath);
                    string conString = string.Empty;
                    switch (extension)
                    {
                        case ".csv": //csv.
                            conString = ConfigurationManager.ConnectionStrings["csvConstring"].ConnectionString;
                            break;
                        case ".xls": //Excel 97-03.
                            conString = ConfigurationManager.ConnectionStrings["Excel03ConString"].ConnectionString;
                            break;
                        case ".xlsx": //Excel 07 and above.
                            conString = ConfigurationManager.ConnectionStrings["Excel07ConString"].ConnectionString;
                            break;
                    }
                    conString = string.Format(conString, filePath);
                    DataTable exceldt = new DataTable();
                    using (OleDbConnection connExcel = new OleDbConnection(conString))
                    {
                        using (OleDbCommand cmdExcel = new OleDbCommand())
                        {
                            using (OleDbDataAdapter odaExcel = new OleDbDataAdapter())
                            {

                                cmdExcel.Connection = connExcel;

                                //Get the name of First Sheet.
                                connExcel.Open();
                                DataTable dtExcelSchema;
                                dtExcelSchema = connExcel.GetOleDbSchemaTable(OleDbSchemaGuid.Tables, null);
                                string sheetName = dtExcelSchema.Rows[0]["TABLE_NAME"].ToString();
                                connExcel.Close();

                                //Read Data from First Sheet.
                                connExcel.Open();
                                cmdExcel.CommandText = "SELECT * From [" + sheetName + "]";
                                odaExcel.SelectCommand = cmdExcel;
                                odaExcel.Fill(exceldt);
                                connExcel.Close();


                                using (SqlConnection con = new SqlConnection(KeywordConnString))
                                {

                                    using (SqlBulkCopy sqlBulkCopy = new SqlBulkCopy(con))
                                    {
                                        //Set the database table name.
                                        sqlBulkCopy.DestinationTableName = "tblKeywordOrganic";

                                        //[OPTIONAL]: Map the Excel columns with that of the database table
                                        sqlBulkCopy.ColumnMappings.Add("Project_Name", "Project_Name");
                                        sqlBulkCopy.ColumnMappings.Add("keyword ", "Keyword");
                                        con.Open();
                                        sqlBulkCopy.WriteToServer(exceldt);
                                        con.Close();
                                    }
                                }

                            }
                        }

                    }
                }

                return Redirect(Request.UrlReferrer.PathAndQuery);
            }
            catch (Exception ex)
            {
                return View(ex);
            }
        }


        public ActionResult ImportFromExcelstatus(HttpPostedFileBase postedFile)
        {
            try
            {

                if (postedFile != null)
                {
                    string filePath = string.Empty;
                    string path = Server.MapPath("~/Uploads/");
                    if (!Directory.Exists(path))
                    {
                        Directory.CreateDirectory(path);
                    }
                    var FileName = Path.ChangeExtension(postedFile.FileName, ".csv");
                    filePath = path + Path.GetFileName(FileName);
                    string extension = Path.GetExtension(FileName);
                    postedFile.SaveAs(filePath);
                    string conString = string.Empty;
                    switch (extension)
                    {
                        case ".csv": //csv.
                            conString = ConfigurationManager.ConnectionStrings["csvConstring"].ConnectionString;
                            break;
                        case ".xls": //Excel 97-03.
                            conString = ConfigurationManager.ConnectionStrings["Excel03ConString"].ConnectionString;
                            break;
                        case ".xlsx": //Excel 07 and above.
                            conString = ConfigurationManager.ConnectionStrings["Excel07ConString"].ConnectionString;
                            break;
                    }
                    conString = string.Format(conString, filePath);
                    DataTable exceldt = new DataTable();
                    using (OleDbConnection connExcel = new OleDbConnection(conString))
                    {
                        using (OleDbCommand cmdExcel = new OleDbCommand())
                        {
                            using (OleDbDataAdapter odaExcel = new OleDbDataAdapter())
                            {

                                cmdExcel.Connection = connExcel;

                                //Get the name of First Sheet.
                                connExcel.Open();
                                DataTable dtExcelSchema;
                                dtExcelSchema = connExcel.GetOleDbSchemaTable(OleDbSchemaGuid.Tables, null);
                                string sheetName = dtExcelSchema.Rows[0]["TABLE_NAME"].ToString();
                                connExcel.Close();

                                //Read Data from First Sheet.
                                connExcel.Open();
                                cmdExcel.CommandText = "SELECT * From [" + sheetName + "]";
                                odaExcel.SelectCommand = cmdExcel;
                                odaExcel.Fill(exceldt);
                                connExcel.Close();


                                using (SqlConnection con = new SqlConnection(KeywordConnString))
                                {

                                    using (SqlBulkCopy sqlBulkCopy = new SqlBulkCopy(con))
                                    {
                                        //Set the database table name.
                                        sqlBulkCopy.DestinationTableName = "tblKeywordOrganicStatus";

                                        //[OPTIONAL]: Map the Excel columns with that of the database table
                                        sqlBulkCopy.ColumnMappings.Add("url", "Url");
                                        sqlBulkCopy.ColumnMappings.Add("Processed Date", "Processed_Date");
                                        sqlBulkCopy.ColumnMappings.Add("keyword ", "keyword");
                                        sqlBulkCopy.ColumnMappings.Add("Position", "Position");
                                        sqlBulkCopy.ColumnMappings.Add("for_processed moth_year", "for_processed_moth_year");
                                        sqlBulkCopy.ColumnMappings.Add("Type", "Type");
                                        con.Open();
                                        sqlBulkCopy.WriteToServer(exceldt);
                                        con.Close();
                                    }
                                }

                            }
                        }

                    }
                }

                return Redirect(Request.UrlReferrer.PathAndQuery);
            }
            catch (Exception ex)
            {
                return View(ex);
            }
        }

        public ActionResult OrganicStatus(string ProjectName)

        {
            try
            {
                    ViewBag.ProjectName = ProjectName.Trim();
                SqlConnection con = new SqlConnection(KeywordConnString);
                SqlCommand cmd = new SqlCommand("proc_tblKeywordOrganicStatuslist", con);
                cmd.Parameters.AddWithValue("@url", ProjectName);
                cmd.CommandType = CommandType.StoredProcedure;
                SqlDataAdapter dataAdapter = new SqlDataAdapter(cmd);
                DataTable dataTable = new DataTable();
                dataAdapter.Fill(dataTable);
                List<ProjectKeywordstatus> organicstatuslist = new List<ProjectKeywordstatus>();
                for (int j = 0; j < dataTable.Rows.Count; j++)
                {
                    ProjectKeywordstatus organicstatus = new ProjectKeywordstatus();
                    organicstatus.Id = Convert.ToInt32(dataTable.Rows[j]["Id"]);
                    organicstatus.Url = dataTable.Rows[j]["Url"].ToString();
                    organicstatus.Processed_Date = Convert.ToDateTime(dataTable.Rows[j]["Processed_Date"].ToString());
                    organicstatus.keyword = dataTable.Rows[j]["keyword"].ToString();
                    //organicstatus.City = dataTable.Rows[j]["City"].ToString();
                    organicstatus.Position = Convert.ToInt32(dataTable.Rows[j]["Position"].ToString());
                    organicstatus.for_processed_moth_year = dataTable.Rows[j]["for_processed_moth_year"].ToString();
                    organicstatus.Type = dataTable.Rows[j]["Type"].ToString();
                    organicstatuslist.Add(organicstatus);

                }
                return View(organicstatuslist);
            }
            catch (Exception ex)
            {
                return View(ex);
            }
        }
        [HttpGet]
        public ActionResult CreateOrganicStatus(string ProjectName)
        {
            ProjectKeywordstatus master = new ProjectKeywordstatus();
            master.Url = ProjectName.Trim();
            ViewBag.ProjectName = ProjectName.Trim();
            return View(master);
        }
        [HttpPost]
        public ActionResult CreateOrganicStatus(ProjectKeywordstatus master)
        {
            try
            {
                if (!ModelState.IsValid)
                    return View(master);

                SqlConnection con = new SqlConnection(KeywordConnString);
                SqlCommand cmd = new SqlCommand("proc_CreatetblKeywordOrganicStatus", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@url", master.Url);
                if (master.Processed_Date != null)
                    cmd.Parameters.AddWithValue("@Processed_Date", master.Processed_Date);
                else
                    master.Processed_Date = DateTime.Now;

                cmd.Parameters.AddWithValue("@keyword", master.keyword);
                cmd.Parameters.AddWithValue("@Position", master.Position);
                cmd.Parameters.AddWithValue("@for_processed_moth_year", master.for_processed_moth_year);
                cmd.Parameters.AddWithValue("@Type", master.Type);

                con.Open();
                cmd.ExecuteNonQuery();
                con.Close();
                return RedirectToAction("OrganicStatus", new { ProjectName = master.Url });
            }
            catch (Exception ex)
            {
                return View(ex);
            }
        }

        [HttpGet]
        public ActionResult UpdateOrganicStatus(int id)
        {
            try
            {
                SqlConnection con = new SqlConnection(KeywordConnString);
                SqlCommand cmd = new SqlCommand("proc_GettblKeywordOrganicstatus", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Id", id);
                SqlDataAdapter dataAdapter = new SqlDataAdapter(cmd);
                DataTable dataTable = new DataTable();
                dataAdapter.Fill(dataTable);
                ProjectKeywordstatus organicstatus = new ProjectKeywordstatus();
                if (dataTable.Rows.Count > 0)
                {
                    //status.Id = Convert.ToInt32(dataTable.Rows[0]["Id"]);

                    organicstatus.Url = dataTable.Rows[0]["Url"].ToString();
                    organicstatus.Processed_Date = Convert.ToDateTime(dataTable.Rows[0]["Processed_Date"].ToString());
                    organicstatus.keyword = dataTable.Rows[0]["keyword"].ToString();
                    organicstatus.Position = Convert.ToInt32(dataTable.Rows[0]["Position"].ToString());
                    organicstatus.for_processed_moth_year = dataTable.Rows[0]["for_processed_moth_year"].ToString();
                    organicstatus.Type = dataTable.Rows[0]["Type"].ToString();
                }
                ViewBag.ProjectName = organicstatus.Url.TrimEnd();

                return View(organicstatus);
            }
            catch (Exception ex)
            {
                return View(ex);
            }
        }
        [HttpPost]
        public ActionResult UpdateOrganicStatus(ProjectKeywordstatus organicstatus)
        {
            try
            {
                if (!ModelState.IsValid)
                    return View(organicstatus);

                SqlConnection con = new SqlConnection(KeywordConnString);
                SqlCommand cmd = new SqlCommand("proc_UpdatetblKeywordOrganicStatus", con);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@Id", organicstatus.Id);
                cmd.Parameters.AddWithValue("@url", organicstatus.Url);

                if (organicstatus.Processed_Date != null)
                    cmd.Parameters.AddWithValue("@Processed_Date", organicstatus.Processed_Date);
                else
                    organicstatus.Processed_Date = DateTime.Now;

                cmd.Parameters.AddWithValue("@keyword", organicstatus.keyword);
                cmd.Parameters.AddWithValue("@Position", organicstatus.Position);
                cmd.Parameters.AddWithValue("@for_processed_moth_year", organicstatus.for_processed_moth_year);
                cmd.Parameters.AddWithValue("@Type", organicstatus.Type);

                con.Open();
                cmd.ExecuteNonQuery();
                con.Close();
                return RedirectToAction("OrganicStatus", new { ProjectName = organicstatus.Url });
            }
            catch (Exception ex)
            {
                return View(ex);
            }
        }

        public ActionResult DeleteOrganicStatus(int Id)
        {
            try
            {
                SqlConnection con = new SqlConnection(KeywordConnString);
                SqlCommand cmd = new SqlCommand("proc_deletetblKeywordOrganicStatus", con);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@Id", Id);
                con.Open();
                cmd.ExecuteNonQuery();
                con.Close();

                return Redirect(Request.UrlReferrer.PathAndQuery);

            }
            catch (Exception ex)

            {
                return View(ex);

            }
        }
    }
}