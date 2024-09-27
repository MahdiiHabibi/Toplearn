using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Toplearn.Core.Convertors;
using Toplearn.DataLayer.Entities.Order;

namespace Toplearn.Core.DTOs.Admin
{
	public class AddEditDiscountViewModel
	{
		public AddEditDiscountViewModel()
		{
			DiscountCode = GetDiscountCode();
		}

		[Display(Name = "کد تخفیف")]
		[Required(ErrorMessage = "{0} ضروری است .")]
		[MaxLength(10, ErrorMessage = "{0} شما نمیتواند بیشتر از {1} باشه ")]
		[MinLength(3, ErrorMessage = "{0} شما نمیتواند کمتر از {1} باشه ")]
		[Remote("CheckDiscountCode","Discount","Admin",AdditionalFields = "__RequestVerificationToken", HttpMethod = "POST")]
		public string DiscountCode { get; set; }


		[Display(Name = "درصد تخیف")]
		[Required(ErrorMessage = "{0} ضروری است .")]
		[Range(1,100,ErrorMessage = "{0} باید بین 0 درصد تا 100 درصد باشد")]
		public int DiscountPercent { get; set; }

		
		public int? UsableCount { get; set; }
		public bool HaveUsableCount { get; set; }


		public DateTime? StartDate { get; set; }
		public bool HaveStartDate { get; set; }


		public DateTime? EndDate { get; set; }
		public bool HaveEndDate { get; set; }


		public string GetDiscountCode()
		{
			var guid = Guid.NewGuid().ToString();
			var model = "";
			for (var i = 0; i < 5; i++)
			{
				model += guid[i];
			}

			return model.CapitalizeFirstLetter();
		}

	}
}
