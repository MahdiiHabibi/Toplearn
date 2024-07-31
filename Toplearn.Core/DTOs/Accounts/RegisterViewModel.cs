using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace Toplearn.Core.DTOs.Accounts
{
    public class RegisterViewModel
    {
        [Display(Name = "نام کاربری")]
        [Required(ErrorMessage = "{0} ضروری است .")]
        [MaxLength(40, ErrorMessage = "{0} شما نمیتواند بیشتر از {1} باشه ")]
		[Remote(routeName: "CheckUserNameIsExist", HttpMethod = "POST", AdditionalFields = "__RequestVerificationToken")]
		public string UserName { get; set; }

        [Display(Name = "نام و نام خانوادگی")]
        [Required(ErrorMessage = "{0} ضروری است .")]
        [MaxLength(50, ErrorMessage = "{0} شما نمیتواند بیشتر از {1} باشه ")]

        public string FullName { get; set; }

        [Display(Name = "ایمیل")]
        [Required(ErrorMessage = "{0} ضروری است .")]
        [MaxLength(200, ErrorMessage = "{0} شما نمیتواند بیشتر از {1} باشه ")]
        [EmailAddress(ErrorMessage = "{0} وارد شده معتبر نمی باشد .")]
		[Remote(routeName: "CheckEmailIsExist", HttpMethod = "POST", AdditionalFields = "__RequestVerificationToken")]
		public string Email { get; set; }

        [Display(Name = "رمز عبور")]
        [Required(ErrorMessage = "{0} ضروری است .")]
        [MaxLength(20, ErrorMessage = "{0} شما نمیتواند بیشتر از {1} باشه ")]
        [MinLength(8, ErrorMessage = "{0} شما نمیتواند کمتر از {1} باشه ")]
        public string Password { get; set; }

        [Display(Name = "تکرار رمز عبور")]
        [Required(ErrorMessage = "{0} ضروری است .")]
        [Compare(nameof(Password), ErrorMessage = "رمز عبور وارد با تکرار آن مطابقت ندارد .")]
        public string RePassword { get; set; }


		
		public string? BackUrl { get; set; }


    }
}
