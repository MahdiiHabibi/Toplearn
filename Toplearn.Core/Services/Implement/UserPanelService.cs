using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Toplearn.Core.Convertors;
using Toplearn.Core.DTOs.UserPanel;
using Toplearn.Core.Generator;
using Toplearn.Core.Services.Interface;
using Toplearn.DataLayer.Entities.User;

namespace Toplearn.Core.Services.Implement
{
	public class UserPanelService(IContextActions<User> _contextActionsForUser) : IUserPanelService
	{
		public async Task<User> GetUserByEmail(string email) =>
			await _contextActionsForUser.GetOne(x => x.Email == email.FixedEmail());

		public async Task<User> GetUserByUserId(int userId) =>
			await _contextActionsForUser.GetOne(x => x.UserId == userId);

		public async Task<string?> ImageTaskInEditUser(string lastImageUrl, IFormFile? newImageFile)
		{
			var newImageUrl = string.Empty;
			try
			{
				if (newImageFile == null) return @"\images\pic\Default.png";

				if (lastImageUrl != @"\images\pic\Default.png" &&
				    File.Exists($"{Directory.GetCurrentDirectory()}\\wwwroot{lastImageUrl}"))
				{
					File.Delete($"{Directory.GetCurrentDirectory()}\\wwwroot{lastImageUrl}");
				}

				newImageUrl = Path.Combine("\\images", "UsersAvatar",
					StringGenerate.GuidGenerate() + Path.GetExtension(newImageFile.FileName));
				var path = $"{Directory.GetCurrentDirectory()}\\wwwroot{newImageUrl}";

				using (var stream = new FileStream(path, FileMode.Create))
				{
					await newImageFile.CopyToAsync(stream);
				}

				return newImageUrl;
			}
			catch
			{
				return string.Empty;
			}
		}

		public async Task<bool> UpdateUser(User user) =>
			 await _contextActionsForUser.UpdateTblOfContext(user);



		public async Task<bool> IsEmailExist(string email) =>
			await _contextActionsForUser.Exists(x => x.Email == email.FixedEmail());

		public async Task<bool> IsUserNameExist(string username) =>
			await _contextActionsForUser.Exists(x => x.UserName == username);


	}
}
