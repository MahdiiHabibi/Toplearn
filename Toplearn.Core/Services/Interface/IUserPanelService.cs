using Microsoft.AspNetCore.Http;
using Toplearn.DataLayer.Entities.User;

namespace Toplearn.Core.Services.Interface
{

	public interface IUserPanelService
	{
		public Task<bool> IsEmailExist(string email);
		public Task<bool> IsUserNameExist(string username);
		public Task<User> GetUserByEmail(string email);
		public Task<User> GetUserByUserId(int userId);
		public Task<string?> ImageTaskInEditUser(string lastImageUrl,IFormFile? newImageFile);
		public Task<bool> UpdateUser(User user);
	}

}