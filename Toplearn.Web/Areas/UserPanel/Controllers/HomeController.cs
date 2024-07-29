using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Toplearn.Core.Convertors;
using Toplearn.Core.Services.Interface.Mapper;
using Toplearn.DataLayer.Entities.User;

namespace Toplearn.Web.Areas.UserPanel.Controllers
{
	[Area("UserPanel")]
	[Authorize]
	public class HomeController(IMapperUserPanel _mapperUserPanel) : Controller
	{
		
		public IActionResult Index()
        {
            var model = _mapperUserPanel.MapTheUserPanelViewModelFromClaims(User.Claims.ToList());
			ViewBag.ImageUrl = model.ImageUrl;
			ViewBag.RegisterDate = model.DateTime.ToShamsi();
            return View(model);
		}


        
	}
}
