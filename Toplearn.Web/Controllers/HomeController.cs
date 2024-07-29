using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Toplearn.Core.Convertors;
using Toplearn.Core.Services.Interface;
using Toplearn.DataLayer.Entities.User;

namespace Toplearn.Web.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
			return View();
        }

        public IActionResult p()
        {
	        throw new Exception("سلام سایت قفل است ");
        }
       

    }
}
