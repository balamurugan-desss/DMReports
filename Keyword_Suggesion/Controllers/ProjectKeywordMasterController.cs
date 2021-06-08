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
using Keyword_Suggesion.Models;

namespace Keyword_Suggesion.Controllers
{
    //[LoginCheck]

    public class ProjectKeywordMasterController : Controller
    {
        private string KeywordConnString = ConfigurationManager.ConnectionStrings["KeywordConnection"].ConnectionString;

        // GET: ProjectKeywordMaster
        public ActionResult ProjectKeywordlist()
        
            {
            if (Session["ProjectName"] == null)
            {
                TempData["Message"] = "Please select a Project!";
                return RedirectToAction("ProjectList", "Project_Keyword");

            }
            SqlConnection con = new SqlConnection(KeywordConnString);
            SqlCommand cmd = new SqlCommand("proc_ProjectKeywordmasterlist", con);
            string ProjectName = Session["ProjectName"].ToString();
            cmd.Parameters.AddWithValue("@ProjectName", ProjectName);
            cmd.CommandType = CommandType.StoredProcedure;
            SqlDataAdapter dataAdapter = new SqlDataAdapter(cmd);
            DataTable dataTable = new DataTable();
            dataAdapter.Fill(dataTable);
            List<ProjectKeywordmaster> KeywordtList = new List<ProjectKeywordmaster>();
            for (int j = 0; j < dataTable.Rows.Count; j++)
            {
                ProjectKeywordmaster Keywordmaster = new ProjectKeywordmaster();
                Keywordmaster.Id = Convert.ToInt32(dataTable.Rows[j]["Id"]);
                Keywordmaster.ProjectName = dataTable.Rows[j]["Project_Name"].ToString();
                Keywordmaster.Keyword = dataTable.Rows[j]["Keyword"].ToString();
                Keywordmaster.City = dataTable.Rows[j]["City"].ToString();
                KeywordtList.Add(Keywordmaster);

            }
            return View(KeywordtList);
        }

        [HttpGet]
        public ActionResult CreateMaterKeyword()
        {
            ProjectKeywordmaster master = new ProjectKeywordmaster();
            return View(master);
        }
        [HttpPost]
        public ActionResult CreateMaterKeyword(ProjectKeywordmaster master)
        {
            try
            {
                if (!ModelState.IsValid)
                    return View(master);

                SqlConnection con = new SqlConnection(KeywordConnString);
                SqlCommand cmd = new SqlCommand("proc_CreateProjectKeywordmaster", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Project_Name", master.ProjectName);
                cmd.Parameters.AddWithValue("@Keyword", master.Keyword);
                cmd.Parameters.AddWithValue("@City", master.City);


                con.Open();
                cmd.ExecuteNonQuery();
                con.Close();
                return RedirectToAction("ProjectKeywordlist");
            }
            catch (Exception ex)
            {
                return View(ex);
            }
        }

        [HttpGet]
        public ActionResult UpdateMasterKeyword(int id)
        {
            SqlConnection con = new SqlConnection(KeywordConnString);
            SqlCommand cmd = new SqlCommand("proc_EditProjectKeywordmasterbyid", con);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@Id", id);
            SqlDataAdapter dataAdapter = new SqlDataAdapter(cmd);
            DataTable dataTable = new DataTable();
            dataAdapter.Fill(dataTable);
            ProjectKeywordmaster master = new ProjectKeywordmaster();
            if (dataTable.Rows.Count > 0)
            {
                master.Id = Convert.ToInt32(dataTable.Rows[0]["Id"].ToString());
                master.ProjectName =dataTable.Rows[0]["Project_Name"].ToString();
                master.Keyword = dataTable.Rows[0]["Keyword"].ToString();
                master.City = dataTable.Rows[0]["City"].ToString();
            }
            return View(master);
        }
        [HttpPost]
        public ActionResult UpdateMasterKeyword(ProjectKeywordmaster master)
        {
            try
            {
                if (!ModelState.IsValid)
                    return View(master);
                SqlConnection connection = new SqlConnection(KeywordConnString);
                SqlCommand sqlCommand = new SqlCommand("proc_EditProjectKeywordmaster", connection);
                sqlCommand.CommandType = CommandType.StoredProcedure;

                sqlCommand.Parameters.AddWithValue("@Id", Convert.ToInt32(master.Id));
                if (master.ProjectName != null)
                    sqlCommand.Parameters.AddWithValue("@Project_Name", master.ProjectName);
                else
                    master.ProjectName = string.Empty;
                if (master.Keyword != null)
                    sqlCommand.Parameters.AddWithValue("@Keyword", master.Keyword);
                else
                    master.Keyword = string.Empty;
                if (master.City != null)
                    sqlCommand.Parameters.AddWithValue("@City", master.City);
                else
                    master.City = string.Empty;
                connection.Open();
                sqlCommand.ExecuteNonQuery();
                connection.Close();
                return RedirectToAction("ProjectKeywordlist");
            }
            catch (Exception ex)
            {
                return View(ex);
            }
        }

        public ActionResult DeletekeywordMaster(int Id)
        {
            try
            {
                SqlConnection con = new SqlConnection(KeywordConnString);
                SqlCommand cmd = new SqlCommand("proc_deleteProjectKeywordmaster", con);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@Id", Id);
                con.Open();
                cmd.ExecuteNonQuery();
                con.Close();

                return RedirectToAction("ProjectKeywordlist");

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
                                            sqlBulkCopy.DestinationTableName = "ProjectKeywordmaster";

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
                                        sqlBulkCopy.DestinationTableName = "ProjectKeywordstatus";

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

        public ActionResult ProjectKeywordStatus(string ProjectName)
      
        {
            try
            {
                ViewBag.ProjectName = ProjectName.Trim();
                SqlConnection con = new SqlConnection(KeywordConnString);
                SqlCommand cmd = new SqlCommand("proc_ProjectKeywordstatuslist", con);
                cmd.Parameters.AddWithValue("@url", ProjectName);
                cmd.CommandType = CommandType.StoredProcedure;
                SqlDataAdapter dataAdapter = new SqlDataAdapter(cmd);
                DataTable dataTable = new DataTable();
                dataAdapter.Fill(dataTable);
                List<ProjectKeywordstatus> Keywordstatuslist = new List<ProjectKeywordstatus>();
                for (int j = 0; j < dataTable.Rows.Count; j++)
                {
                    ProjectKeywordstatus Keywordstatus = new ProjectKeywordstatus();
                    Keywordstatus.Id = Convert.ToInt32(dataTable.Rows[j]["Id"]);
                    Keywordstatus.Url = dataTable.Rows[j]["Url"].ToString();
                    Keywordstatus.Processed_Date = Convert.ToDateTime(dataTable.Rows[j]["Processed_Date"].ToString());
                    Keywordstatus.keyword = dataTable.Rows[j]["keyword"].ToString();
                    Keywordstatus.City = dataTable.Rows[j]["City"].ToString();
                    Keywordstatus.Position = Convert.ToInt32(dataTable.Rows[j]["Position"].ToString());
                    Keywordstatus.for_processed_moth_year = dataTable.Rows[j]["for_processed_moth_year"].ToString();
                    Keywordstatus.Type = dataTable.Rows[j]["Type"].ToString();
                    Keywordstatuslist.Add(Keywordstatus);

                }
                return View(Keywordstatuslist);
            }
            catch (Exception ex)
            {
                return View(ex);
            }
        }
        [HttpGet]
        public ActionResult CreateStatusKeyword(string ProjectName)
        {
            ProjectKeywordstatus master = new ProjectKeywordstatus();
            master.Url = ProjectName.Trim();
            return View(master);
        }
        [HttpPost]
        public ActionResult CreateStatusKeyword(ProjectKeywordstatus master)
        {
            try
            {
                if (!ModelState.IsValid)
                    return View(master);

                SqlConnection con = new SqlConnection(KeywordConnString);
                SqlCommand cmd = new SqlCommand("proc_CreateProjectKeywordstatus", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@url", master.Url);
                if (master.Processed_Date != null)
                    cmd.Parameters.AddWithValue("@Processed_Date", master.Processed_Date);
                else
                    master.Processed_Date = DateTime.Now;

                cmd.Parameters.AddWithValue("@keyword", master.keyword);
                cmd.Parameters.AddWithValue("@City", master.City);
                cmd.Parameters.AddWithValue("@Position", master.Position);
                cmd.Parameters.AddWithValue("@for_processed_moth_year", master.for_processed_moth_year);
                cmd.Parameters.AddWithValue("@Type", master.Type);

                con.Open();
                cmd.ExecuteNonQuery();
                con.Close();
                return RedirectToAction("ProjectKeywordStatus", new { ProjectName = master.Url });
            }
            catch (Exception ex)
            {
                return View(ex);
            }
        }

        [HttpGet]
        public ActionResult UpdateStatusKeyword(int id)
        {
            try
            {
                SqlConnection con = new SqlConnection(KeywordConnString);
                SqlCommand cmd = new SqlCommand("proc_GetProjectKeywordstatusbyid", con);
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
                return View(status);
            }
            catch (Exception ex)
            {
                return View(ex);
            }
        }
        [HttpPost]
        public ActionResult UpdateStatusKeyword(ProjectKeywordstatus status)
        {
            try
            {
                if (!ModelState.IsValid)
                    return View(status);

                SqlConnection con = new SqlConnection(KeywordConnString);
                SqlCommand cmd = new SqlCommand("proc_UpdateProjectKeywordstatus", con);
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
                return RedirectToAction("ProjectKeywordStatus", new { ProjectName = status.Url});
            }
            catch (Exception ex)
            {
                return View(ex);
            }
        }

        public ActionResult DeletekeywordStatus(int Id)
        {
            try
            {
                SqlConnection con = new SqlConnection(KeywordConnString);
                SqlCommand cmd = new SqlCommand("proc_deleteProjectKeywordstatus", con);
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