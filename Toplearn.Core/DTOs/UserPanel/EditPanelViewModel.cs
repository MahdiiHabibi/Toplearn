using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Toplearn.Core.DTOs.UserPanel
{
	public class EditPanelViewModel
	{
		
		[Display(Name = "نام کاربری")]
		[Required(ErrorMessage = "{0} ضروری است .")]
		[MaxLength(20, ErrorMessage = "{0} شما نمیتواند بیشتر از {1} باشه ")]
		[Remote(routeName:"CheckUserInEdit",HttpMethod = "POST",AdditionalFields = "__RequestVerificationToken")]

		public string UserName { get; set; }

		[Display(Name = "نام و نام خانوادگی")]
		[Required(ErrorMessage = "{0} ضروری است .")]
		[MaxLength(60, ErrorMessage = "{0} شما نمیتواند بیشتر از {1} باشه ")]

		public string FullName { get; set; }

		
		[EmailAddress]
		public string Email { get; set; }


		public IFormFile? ImageFile { get; set; }
	}
}
