using System.Data;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Toplearn.Core.Convertors;
using Toplearn.Core.Services.Interface;
using Toplearn.DataLayer.Context;
using Toplearn.DataLayer.Entities.User;

namespace Toplearn.Web.Controllers
{
	public class HomeController(TopLearnContext _db) : Controller
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
