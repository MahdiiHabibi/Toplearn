using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace Toplearn.Core.DTOs.Admin
{
	public class AddCategoryViewModel
	{

		[Display(Name = "نام دسته بندی")]
		[Required(ErrorMessage = "{0} ضروری است .")]
		[MaxLength(20, ErrorMessage = "{0} شما نمیتواند بیشتر از {1} باشه ")]
		[Remote("CheckCategoryName", "Category",AdditionalFields = "__RequestVerificationToken",HttpMethod = "POST",ErrorMessage = "این نام ورودی قبلا ثبت شده است .")]
		public string CategoryName { get; set; }


		public int? ParentCategoryId { get; set; } 

	}
}
