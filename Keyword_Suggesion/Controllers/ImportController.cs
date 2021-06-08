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
    public class ImportController : Controller
    {
        private string KeywordConnString = ConfigurationManager.ConnectionStrings["KeywordConnection"].ConnectionString;

        // GET: Import
        public ActionResult Index()
        {
            return View();
        }
        [HttpGet]
        public ActionResult DM_ReportsImportFile()
        {
            Importfile importfile = new Importfile();
            return View(importfile);
        }
        [HttpPost]
        public ActionResult DM_ReportsImportFile(HttpPostedFileBase postedFile, Importfile importfile)
        {
            //Importfile importfile = new Importfile();
            if (!ModelState.IsValid)
                return View(importfile);

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
                                        if (importfile.ImportType == "1")
                                        {
                                            //Set the database table name.
                                            sqlBulkCopy.DestinationTableName = "TempImportdata";

                                            //[OPTIONAL]: Map the Excel columns with that of the database table
                                            sqlBulkCopy.ColumnMappings.Add("Project_Name", "ProjectName");
                                            sqlBulkCopy.ColumnMappings.Add("keyword ", "KeywordName");
                                            sqlBulkCopy.ColumnMappings.Add("City", "CityName");
                                        }
                                        else
                                        {
                                            //Set the database table name.
                                            sqlBulkCopy.DestinationTableName = "TempImportKeyworddata";

                                            //[OPTIONAL]: Map the Excel columns with that of the database table
                                            sqlBulkCopy.ColumnMappings.Add("Name", "ProjectName");
                                            sqlBulkCopy.ColumnMappings.Add("Keyword", "KeywordName");
                                            sqlBulkCopy.ColumnMappings.Add("City", "CityName");
                                            sqlBulkCopy.ColumnMappings.Add("Date", "Date");
                                            sqlBulkCopy.ColumnMappings.Add("Position", "Position");
                                        }
                                        con.Open();
                                        sqlBulkCopy.WriteToServer(exceldt);
                                        con.Close();
                                    }
                                }

                                SqlConnection con1 = new SqlConnection(KeywordConnString);
                                SqlCommand cmd = new SqlCommand();
                                cmd.Connection = con1;
                                if (importfile.ImportType == "1")
                                {
                                    cmd.CommandText = "Proc_ImportProjectcities";
                                    TempData["Project"] = "City List has been Imported Successfully";
                                }
                                else { 
                                    cmd.CommandText = "Proc_ImportProjectkeywords";
                                    TempData["Project"] = "Keywords List has been Imported Successfully";
                                }
                                cmd.CommandType = CommandType.StoredProcedure;
                                con1.Open();
                                cmd.ExecuteNonQuery();
                                con1.Close();
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

    }
}