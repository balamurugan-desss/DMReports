using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Keyword_Suggesion.Models
{
    public class LoginCheck : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            HttpContext ctx = HttpContext.Current;
            if (HttpContext.Current.Session["Access"] == null)
            {
                filterContext.Result = new RedirectResult("~/Login/LoginPage");
                return;
            }
            base.OnActionExecuting(filterContext);
        }
    }
}