using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Toplearn.Core.DTOs.UserPanel
{
	public class UserPanelViewModel
	{
		
		public int UserId { get; set; }

		[Display(Name = "نام کاربری")]
		[Required(ErrorMessage = "{0} ضروری است .")]
		[MaxLength(20, ErrorMessage = "{0} شما نمیتواند بیشتر از {1} باشه ")]

		public string UserName { get; set; }

		[Display(Name = "نام و نام خانوادگی")]
		[Required(ErrorMessage = "{0} ضروری است .")]
		[MaxLength(60, ErrorMessage = "{0} شما نمیتواند بیشتر از {1} باشه ")]

		public string FullName { get; set; }

		[Display(Name = "ایمیل")]
		[Required(ErrorMessage = "{0} ضروری است .")]
		[MaxLength(200, ErrorMessage = "{0} شما نمیتواند بیشتر از {1} باشه ")]
		[EmailAddress]
		public string Email { get; set; }

		[Display(Name = "تاریخ ثبت نام ")]
		[Required(ErrorMessage = "{0} ضروری است .")]
		public DateTime DateTime { get; set; }

		[Display(Name = "عکس")]
		[MaxLength(500, ErrorMessage = "{0} شما نمیتواند بیشتر از {1} باشه ")]
		[DataType(DataType.ImageUrl)]
		public string ImageUrl { get; set; }

		public int WalletBalance { get; set; }
	}
}
