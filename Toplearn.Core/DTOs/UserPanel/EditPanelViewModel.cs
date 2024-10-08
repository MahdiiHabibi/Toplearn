﻿using System;
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
		[MaxLength(40, ErrorMessage = "{0} شما نمیتواند بیشتر از {1} باشه ")]
		[Remote("CheckUserNameIsExist","Home", HttpMethod = "POST", AdditionalFields = "__RequestVerificationToken")]
		public string UserName { get; set; }

		[Display(Name = "نام و نام خانوادگی")]
		[Required(ErrorMessage = "{0} ضروری است .")]
		[MaxLength(60, ErrorMessage = "{0} شما نمیتواند بیشتر از {1} باشه ")]

		public string FullName { get; set; }

		
		[EmailAddress]
		public string Email { get; set; }

		[Display(Name = "توضیحات")]
		[Required(ErrorMessage = "{0} ضروری است .")]
		[MinLength(40, ErrorMessage = "{0} شما نمیتواند کمتر از {1} کاراکتر باشد ")]
		public string? UserDescription { get; set; }


		public IFormFile? ImageFile { get; set; }
	}
}
