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
    public class BusinessCityController : Controller
    {
        // GET: BusinessCity
        private string KeywordConnString = ConfigurationManager.ConnectionStrings["KeywordConnection"].ConnectionString;

        public ActionResult KeywordGMBlist()

        {
            //if (Session["ProjectName"] == null)
            //{
            //    TempData["Message"] = "Please select a Project!";
            //    return RedirectToAction("ProjectList", "Project_Keyword");

            //}
            SqlConnection con = new SqlConnection(KeywordConnString);
            SqlCommand cmd = new SqlCommand("proc_tblKeywordGMBlist", con);
            //string ProjectName = Session["ProjectName"].ToString();
            //cmd.Parameters.AddWithValue("@ProjectName", ProjectName);
            cmd.CommandType = CommandType.StoredProcedure;
            SqlDataAdapter dataAdapter = new SqlDataAdapter(cmd);
            DataTable dataTable = new DataTable();
            dataAdapter.Fill(dataTable);
            List<ProjectKeywordmaster> Gmblist = new List<ProjectKeywordmaster>();
            for (int j = 0; j < dataTable.Rows.Count; j++)
            {
                ProjectKeywordmaster GMB = new ProjectKeywordmaster();
                GMB.Id = Convert.ToInt32(dataTable.Rows[j]["Id"]);
                GMB.ProjectName = dataTable.Rows[j]["Project_Name"].ToString();
                GMB.Keyword = dataTable.Rows[j]["Keyword"].ToString();
                GMB.City = dataTable.Rows[j]["City"].ToString();
                Gmblist.Add(GMB);

            }
            return View(Gmblist);
        }

        [HttpGet]
        public ActionResult CreateBusinessCity()
        {
            ProjectKeywordmaster businesscity = new ProjectKeywordmaster();
            return View(businesscity);
        }
        [HttpPost]
        public ActionResult CreateBusinessCity(ProjectKeywordmaster businesscity)
        {
            try
            {
                if (!ModelState.IsValid)
                    return View(businesscity);
                SqlConnection con = new SqlConnection(KeywordConnString);
                SqlCommand cmd = new SqlCommand("proc_CreatetblKeywordGMB", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Project_Name", businesscity.ProjectName);
                cmd.Parameters.AddWithValue("@Keyword", businesscity.Keyword);
                cmd.Parameters.AddWithValue("@City", businesscity.City);


                con.Open();
                cmd.ExecuteNonQuery();
                con.Close();
                return RedirectToAction("KeywordGMBlist");
            }
            catch (Exception ex)
            {
                return View(ex);
            }
        }

        [HttpGet]
        public ActionResult UpdateBusinessCity(int id)
        {
            SqlConnection con = new SqlConnection(KeywordConnString);
            SqlCommand cmd = new SqlCommand("proc_EditTblKeywordGMB", con);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@Id", id);
            SqlDataAdapter dataAdapter = new SqlDataAdapter(cmd);
            DataTable dataTable = new DataTable();
            dataAdapter.Fill(dataTable);
            ProjectKeywordmaster bc = new ProjectKeywordmaster();
            if (dataTable.Rows.Count > 0)
            {
                bc.Id = Convert.ToInt32(dataTable.Rows[0]["Id"].ToString());
                bc.ProjectName = dataTable.Rows[0]["Project_Name"].ToString();
                bc.Keyword = dataTable.Rows[0]["Keyword"].ToString();
                bc.City = dataTable.Rows[0]["City"].ToString();
            }
            return View(bc);
        }
        [HttpPost]
        public ActionResult UpdateBusinessCity(ProjectKeywordmaster bc)
        {
            try
            {
                if (!ModelState.IsValid)
                    return View(bc);
                SqlConnection connection = new SqlConnection(KeywordConnString);
                SqlCommand sqlCommand = new SqlCommand("proc_UpdateTblKeywordGMB", connection);
                sqlCommand.CommandType = CommandType.StoredProcedure;

                sqlCommand.Parameters.AddWithValue("@Id", Convert.ToInt32(bc.Id));
                if (bc.ProjectName != null)
                    sqlCommand.Parameters.AddWithValue("@Project_Name", bc.ProjectName);
                else
                    bc.ProjectName = string.Empty;
                if (bc.Keyword != null)
                    sqlCommand.Parameters.AddWithValue("@Keyword", bc.Keyword);
                else
                    bc.Keyword = string.Empty;
                if (bc.City != null)
                    sqlCommand.Parameters.AddWithValue("@City", bc.City);
                else
                    bc.City = string.Empty;
                connection.Open();
                sqlCommand.ExecuteNonQuery();
                connection.Close();
                return RedirectToAction("KeywordGMBlist");
            }
            catch (Exception ex)
            {
                return View(ex);
            }
        }

        public ActionResult DeleteBusinessCity(int Id)
        {
            try
            {
                SqlConnection con = new SqlConnection(KeywordConnString);
                SqlCommand cmd = new SqlCommand("proc_deleteTblKeywordGMB", con);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@Id", Id);
                con.Open();
                cmd.ExecuteNonQuery();
                con.Close();

                return RedirectToAction("KeywordGMBlist");

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
                                        sqlBulkCopy.DestinationTableName = "tblKeywordGMB";

                                        //[OPTIONAL]: Map the Excel columns with that of the database table
                                        sqlBulkCopy.ColumnMappings.Add("Project_Name", "Project_Name");
                                        sqlBulkCopy.ColumnMappings.Add("keyword ", "Keyword");
                                        sqlBulkCopy.ColumnMappings.Add("City", "City");
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
                                        sqlBulkCopy.DestinationTableName = "tblKeywordGMBStatus";

                                        //[OPTIONAL]: Map the Excel columns with that of the database table
                                        sqlBulkCopy.ColumnMappings.Add("url", "Url");
                                        sqlBulkCopy.ColumnMappings.Add("Processed Date", "Processed_Date");
                                        sqlBulkCopy.ColumnMappings.Add("keyword ", "keyword");
                                        sqlBulkCopy.ColumnMappings.Add("City", "City");
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

        public ActionResult GMBStatuslist(string ProjectName)

        {
            try
            {
                ViewBag.ProjectName = ProjectName.Trim();
                SqlConnection con = new SqlConnection(KeywordConnString);
                SqlCommand cmd = new SqlCommand("proc_tblKeywordGMBStatuslist", con);
                cmd.Parameters.AddWithValue("@url", ProjectName);
                cmd.CommandType = CommandType.StoredProcedure;
                SqlDataAdapter dataAdapter = new SqlDataAdapter(cmd);
                DataTable dataTable = new DataTable();
                dataAdapter.Fill(dataTable);
                List<ProjectKeywordstatus> statuslist = new List<ProjectKeywordstatus>();
                for (int j = 0; j < dataTable.Rows.Count; j++)
                {
                    ProjectKeywordstatus status = new ProjectKeywordstatus();
                    status.Id = Convert.ToInt32(dataTable.Rows[j]["Id"]);
                    status.Url = dataTable.Rows[j]["Url"].ToString();
                    status.Processed_Date = Convert.ToDateTime(dataTable.Rows[j]["Processed_Date"].ToString());
                    status.keyword = dataTable.Rows[j]["keyword"].ToString();
                    status.City = dataTable.Rows[j]["City"].ToString();
                    status.Position = Convert.ToInt32(dataTable.Rows[j]["Position"].ToString());
                    status.for_processed_moth_year = dataTable.Rows[j]["for_processed_moth_year"].ToString();
                    status.Type = dataTable.Rows[j]["Type"].ToString();
                    statuslist.Add(status);

                }
                return View(statuslist);
            }
            catch (Exception ex)
            {
                return View(ex);
            }
        }
        [HttpGet]
        public ActionResult CreateBusinessStatus(string ProjectName)
        {
            ProjectKeywordstatus status = new ProjectKeywordstatus();
            status.Url = ProjectName.Trim();
            ViewBag.ProjectName = ProjectName.Trim();
            return View(status);
        }
        [HttpPost]
        public ActionResult CreateBusinessStatus(ProjectKeywordstatus status)
        {
            try
            {
                if (!ModelState.IsValid)
                    return View(status);

                SqlConnection con = new SqlConnection(KeywordConnString);
                SqlCommand cmd = new SqlCommand("proc_CreatetblKeywordgmbStatus", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@url", status.Url);
                if (status.Processed_Date != null)
                    cmd.Parameters.AddWithValue("@Processed_Date", status.Processed_Date);
                else
                    status.Processed_Date = DateTime.Now;

                cmd.Parameters.AddWithValue("@keyword", status.keyword);
                cmd.Parameters.AddWithValue("@City", status.City);
                cmd.Parameters.AddWithValue("@Position", status.Position);
                cmd.Parameters.AddWithValue("@for_processed_moth_year", status.for_processed_moth_year);
                cmd.Parameters.AddWithValue("@Type", status.Type);

                con.Open();
                cmd.ExecuteNonQuery();
                con.Close();
                return RedirectToAction("GMBStatuslist", new { ProjectName = status.Url });
            }
            catch (Exception ex)
            {
                return View(ex);
            }
        }

        [HttpGet]
        public ActionResult UpdateBusinessStatus(int id)
        {
            try
            {
                SqlConnection con = new SqlConnection(KeywordConnString);
                SqlCommand cmd = new SqlCommand("proc_GettblKeywordGMBstatus", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Id", id);
                SqlDataAdapter dataAdapter = new SqlDataAdapter(cmd);
                DataTable dataTable = new DataTable();
                dataAdapter.Fill(dataTable);
                ProjectKeywordstatus status = new ProjectKeywordstatus();
                if (dataTable.Rows.Count > 0)
                {
                    //status.Id = Convert.ToInt32(dataTable.Rows[0]["Id"]);

                    status.Url = dataTable.Rows[0]["Url"].ToString();
                    status.Processed_Date = Convert.ToDateTime(dataTable.Rows[0]["Processed_Date"].ToString());
                    status.keyword = dataTable.Rows[0]["keyword"].ToString();
                    status.City = dataTable.Rows[0]["City"].ToString();
                    status.Position = Convert.ToInt32(dataTable.Rows[0]["Position"].ToString());
                    status.for_processed_moth_year = dataTable.Rows[0]["for_processed_moth_year"].ToString();
                    status.Type = dataTable.Rows[0]["Type"].ToString();
                }
                ViewBag.Projectname = status.Url.Trim();
                return View(status);
            }
            catch (Exception ex)
            {
                return View(ex);
            }
        }
        [HttpPost]
        public ActionResult UpdateBusinessStatus(ProjectKeywordstatus status)
        {
            try
            {
                if (!ModelState.IsValid)
                    return View(status);

                SqlConnection con = new SqlConnection(KeywordConnString);
                SqlCommand cmd = new SqlCommand("proc_UpdatetblKeywordGMBStatus", con);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@Id", status.Id);
                cmd.Parameters.AddWithValue("@url", status.Url);

                if (status.Processed_Date != null)
                    cmd.Parameters.AddWithValue("@Processed_Date", status.Processed_Date);
                else
                    status.Processed_Date = DateTime.Now;

                cmd.Parameters.AddWithValue("@keyword", status.keyword);
                cmd.Parameters.AddWithValue("@City", status.City);
                cmd.Parameters.AddWithValue("@Position", status.Position);
                cmd.Parameters.AddWithValue("@for_processed_moth_year", status.for_processed_moth_year);
                cmd.Parameters.AddWithValue("@Type", status.Type);

                con.Open();
                cmd.ExecuteNonQuery();
                con.Close();
                return RedirectToAction("GMBStatuslist", new { ProjectName = status.Url });
            }
            catch (Exception ex)
            {
                return View(ex);
            }
        }

        public ActionResult DeleteBusinessStatus(int Id)
        {
            try
            {
                SqlConnection con = new SqlConnection(KeywordConnString);
                SqlCommand cmd = new SqlCommand("proc_deletetblKeywordGMBStatus", con);
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
