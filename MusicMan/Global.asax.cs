﻿using MusicMan.Controllers;
using System;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace MusicMan
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            RouteConfig.RegisterRoutes(RouteTable.Routes);
        }
        protected void Application_Error()
        {
            Exception exception = Server.GetLastError();
            //Response.Clear();

            //HttpException httpException = exception as HttpException;
            //string httpCode = httpException.GetHttpCode().ToString();

            //// clear error on server
            //Server.ClearError();

            //Response.Redirect(String.Format("~/Error/Custom/?httpCode={0}", httpCode));

            //var httpException = exception as HttpException;
            //var httpCode = httpException != null ? httpException.GetHttpCode() : 404;
            //var errMassege = exception.Message.ToString();
            ////AppLogger.Fatal(errMassege, exception);

            //ExecuteError(httpCode, exception.Message.ToString());
        }


        private void ExecuteError(Int32? httpCode, String message)
        {
            var routeData = new RouteData();
            routeData.Values.Add("controller", ErrorController.Name);
            routeData.Values.Add("action", ErrorController.ActionName_Custom);
            if (httpCode.HasValue)
                routeData.Values.Add("httpCode", httpCode.Value);
            if (message != String.Empty)
                routeData.Values.Add("message", message);

            try
            {
                Response.Clear();
                Server.ClearError();
                Response.ContentType = "text/html; charset=utf-8";
                Response.TrySkipIisCustomErrors = true;

                IController errorController = new ErrorController();
                errorController.Execute(new RequestContext(new HttpContextWrapper(Context), routeData));
            }
            catch (Exception exception)
            {
                //SiteLogger.Fatal("Global.ExecuteError", exception, Context.Request.RequestContext.HttpContext);
            }
        }
    }
}
