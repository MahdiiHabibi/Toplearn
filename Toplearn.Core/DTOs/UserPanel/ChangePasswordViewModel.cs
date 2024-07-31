using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Toplearn.Core.DTOs.UserPanel
{
	public class ChangePasswordViewModel
	{
		
		[Display(Name = "رمز عبور فعلی")]
		[Required(ErrorMessage = "{0} ضروری است .")]
		[MaxLength(20, ErrorMessage = "{0} شما نمیتواند بیشتر از {1} باشه ")]
		[MinLength(8, ErrorMessage = "{0} شما نمیتواند کمتر از {1} باشه ")]
		public string OldPassword { get; set; }

		[Display(Name = "رمز عبور جدید")]
		[Required(ErrorMessage = "{0} ضروری است .")]
		[MaxLength(20, ErrorMessage = "{0} شما نمیتواند بیشتر از {1} باشه ")]
		[MinLength(8, ErrorMessage = "{0} شما نمیتواند کمتر از {1} باشه ")]
		
		public string NewPassword { get; set; }

		[Display(Name = "تکرار رمز عبور")]
		[Required(ErrorMessage = "{0} ضروری است .")]
		[Compare(nameof(NewPassword), ErrorMessage = "رمز عبور وارد با تکرار آن مطابقت ندارد .")]
		public string ReNewPassword { get; set; }
	}
}
