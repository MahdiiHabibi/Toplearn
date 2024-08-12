using Microsoft.AspNetCore.Mvc;
using Toplearn.Core.DTOs.Admin;
using Toplearn.Core.Services.Interface;
using Toplearn.Core.Services.Interface.Mapper;
using Toplearn.DataLayer.Entities.User;

namespace Toplearn.Web.ViewComponents
{
	public class ShowRolesOfUserViewComponent(IRoleManager roleManager, IMapperAdmin _mapperAdmin, IUserPanelService _userPanelService) : ViewComponent
	{
		public async Task<IViewComponentResult> InvokeAsync(int userId)
		{
			User user = await _userPanelService.GetUserByUserId(userId);
			var userForShowAddEditRoleViewModel = _mapperAdmin.MapUserForShowAddEditRoleViewModelFromUser(user);
			userForShowAddEditRoleViewModel.ShowAddEditRoleViewModels = await roleManager.GetRolesForShows(userId);
			return View("ShowRolesOfUser", userForShowAddEditRoleViewModel);
		}
	}
}
