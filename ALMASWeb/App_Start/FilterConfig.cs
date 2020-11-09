using ALMASWeb.Controllers;
using System.Web.Mvc;
using System.Web.Routing;

namespace ALMASWeb
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
            filters.Add(new LoginValidationAttribute());
        }
    }

    public class LoginValidationAttribute : FilterAttribute, IActionFilter
    {
        public void OnActionExecuting(ActionExecutingContext context)
        {
            if(context.ActionDescriptor.ActionName != nameof(OperatorController.Login)) 
            {
                if (!OperatorController.isLoggedIn(context.HttpContext.Session))
                {
                    context.Result = new RedirectToRouteResult(
                        new RouteValueDictionary(new
                        {
                            action = Helper.LOGIN_ACTIONNAME,
                            controller = Helper.LOGIN_CONTROLLERNAME,
                            Area = Helper.LOGIN_AREANAME,
                            returnUrl = context.HttpContext.Request.RawUrl
                        })
                    );
                }
            }
        }

        public void OnActionExecuted(ActionExecutedContext context) { }
    }
}
