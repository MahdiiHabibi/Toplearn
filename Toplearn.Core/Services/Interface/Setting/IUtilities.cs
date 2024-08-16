using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Toplearn.Core.DTOs.Setting;

namespace IdentitySample.Repositories
{
	public interface IUtilities
	{
		public IList<ActionAndControllerAndAreaViewModel> AreaAndActionAndControllerNamesList();
		public IList<string> GetAllAreasNames();
		public Task<string?> RoleValidationGuid();
		public Task<string> CreateAndSaveNewValidationCode();
		public System.Threading.Tasks.Task<bool> SendIVG(int userId);


	}
}
