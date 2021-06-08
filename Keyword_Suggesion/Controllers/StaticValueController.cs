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
    public class StaticValueController : Controller
    {
        // GET: StaticValue
        public ActionResult Index()
        {
            return View();
        }
        public static List<SelectListItem> ListIscreate()
        {

            List<SelectListItem> Iscreate = new List<SelectListItem>()
            {

                new SelectListItem
                {
                    Text = "Yes",
                    Value = "1"
                },
                new SelectListItem
                {
                    Text = "No",
                    Value = "2"
                },

            };
            return Iscreate;


        }
        public static List<SelectListItem> ListSelect()
        {

            List<SelectListItem> Iscreate = new List<SelectListItem>()
            {

                new SelectListItem
                {
                    Text = "Save Keywords",
                    Value = "1"
                },
                new SelectListItem
                {
                    Text = "View Keywords",
                    Value = "2"
                },

            };
            return Iscreate;


        }

        public JsonResult JsonGetStateCode(string id)
        {

            string statecode = string.Empty;
            SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["KeywordConnection"].ConnectionString);


            string query = "select StateName, StateCode from usstates where statename = @statename group by StateName, StateCode";
            SqlCommand cmd = new SqlCommand(query, con);
            cmd.Parameters.AddWithValue("@StateName", id);
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            da.Fill(dt);
            statecode = dt.Rows[0]["StateCode"].ToString();


            return Json(statecode, JsonRequestBehavior.AllowGet);
        }

        public static List<SelectListItem> ListProjectName()
        {

            SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["KeywordConnection"].ConnectionString);

            List<SelectListItem> ProjectList = new List<SelectListItem>();
            {
                string query = "select ProjectName from Tbl_Dynamic_ProjectName group by ProjectName ";
                SqlCommand cmd = new SqlCommand(query, con);
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                da.Fill(dt);
                for (int i = 0; i < dt.Rows.Count; i++)
                {

                    ProjectList.Add(new SelectListItem
                    {

                        Text = dt.Rows[i]["ProjectName"].ToString(),
                        Value = dt.Rows[i]["ProjectName"].ToString()
                    });
                }
            }

            return ProjectList;

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


        public static List<SelectListItem> ListStates()
        {

            SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["KeywordConnection"].ConnectionString);

            List<SelectListItem> statelist = new List<SelectListItem>();
            {
                string query = "select StateName from usstates group by StateName ";
                SqlCommand cmd = new SqlCommand(query, con);
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                da.Fill(dt);
                for (int i = 0; i < dt.Rows.Count; i++)
                {

                    statelist.Add(new SelectListItem
                    {

                        Text = dt.Rows[i]["StateName"].ToString(),
                        Value = dt.Rows[i]["StateName"].ToString()
                    });
                }
            }

            return statelist;

        }

        public JsonResult ListKeywords(string Projectname)
        {

            SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["KeywordConnection"].ConnectionString);

            List<SelectListItem> MainKeywordList = new List<SelectListItem>();
            {
                string query = "select MainKeyword from tbl"+ Projectname.Replace(" ",string.Empty) + "  group by MainKeyword ";
                SqlCommand cmd = new SqlCommand(query, con);
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                da.Fill(dt);
                for (int i = 0; i < dt.Rows.Count; i++)
                {

                    MainKeywordList.Add(new SelectListItem
                    {

                        Text = dt.Rows[i]["MainKeyword"].ToString(),
                        Value = dt.Rows[i]["MainKeyword"].ToString()
                    });
                }
            }

            return Json(MainKeywordList, JsonRequestBehavior.AllowGet);
        }
        public static List<SelectListItem> ListImportType()
        {

            List<SelectListItem> Iscreate = new List<SelectListItem>()
            {

                new SelectListItem
                {
                    Text = "Project Cities",
                    Value = "1"
                },
                new SelectListItem
                {
                    Text = "Project Keywords",
                    Value = "2"
                },

            };
            return Iscreate;


        }

    }
}