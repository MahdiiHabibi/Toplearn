using System.Net;
using Microsoft.AspNetCore.Mvc;
using Quartz.Util;

namespace Toplearn.Web.Controllers
{
    public class ErrorsController : Controller
    {

        [Route("/Error")]
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
		public IActionResult Index()
        {
            var statusCode = HttpContext.Response.StatusCode;
            var link = statusCode switch
            {
	            (int)HttpStatusCode.OK => null,
	            (int)HttpStatusCode.Forbidden => "/AccessDenied",
	            (int)HttpStatusCode.NotFound => "/NotFound",
	            (int)HttpStatusCode.InternalServerError => "/ServerError",
	            (int)HttpStatusCode.BadRequest => "/BadRequest",
	            _ => "/ServerError"
			};

            return Redirect(link??"/");
        }


        #region Access Denied


        [Route("/AccessDenied")]
        public IActionResult AccessDenied() => View();


        #endregion

        #region NotFound

        [Route("/NotFound")]
        public IActionResult NotFound() => View();


        #endregion

        #region InternalServerError

        [Route("/ServerError")]
        public IActionResult InternalServerError() => View();

        #endregion

        #region BadRequest

        [Route("/BadRequest")]
		public IActionResult BadRequest() => View();

		#endregion
    }
}
