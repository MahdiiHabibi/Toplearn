using Microsoft.AspNetCore.Mvc;
using Toplearn.Core.Services.Interface;
using Toplearn.DataLayer.Entities.User;

namespace Toplearn.Web.Controllers
{
    public class HomeController(IUserAction userAction) : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
